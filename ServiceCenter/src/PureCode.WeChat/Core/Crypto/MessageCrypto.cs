using System;
using System.Collections;

//using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

//-40001 ： 签名验证错误
//-40002 :  xml解析失败
//-40003 :  sha加密生成签名失败
//-40004 :  AESKey 非法
//-40005 :  corpid 校验错误
//-40006 :  AES 加密失败
//-40007 ： AES 解密失败
//-40008 ： 解密后得到的buffer非法
//-40009 :  base64加密异常
//-40010 :  base64解密异常
namespace PureCode.WeChat.Core
{
  public class MessageCrypto
  {
    private string token;
    private string encodingAesKey;
    private string receiveId;

    //构造函数
    // @param sToken: 企业微信后台，开发者设置的Token
    // @param sEncodingAESKey: 企业微信后台，开发者设置的EncodingAESKey
    // @param sReceiveId: 不同场景含义不同，详见文档说明
    public MessageCrypto(string token, string encodingAesKey, string receiveId)
    {
      this.token = token;
      this.receiveId = receiveId;
      this.encodingAesKey = encodingAesKey;
    }

    //验证URL
    // @param sMsgSignature: 签名串，对应URL参数的msg_signature
    // @param sTimeStamp: 时间戳，对应URL参数的timestamp
    // @param sNonce: 随机串，对应URL参数的nonce
    // @param sEchoStr: 随机串，对应URL参数的echostr
    // @param sReplyEchoStr: 解密之后的echostr，当return返回0时有效
    // @return：成功0，失败返回对应的错误码
    public int VerifyURL(string signature, string timestamp, string nonce, string echoStr, out string replyEcho)
    {
      replyEcho = "";
      if (encodingAesKey.Length != 43)
      {
        return (int)MessageCryptoErrorCode.IllegalAesKeyError;
      }
      int ret = VerifySignature(token, timestamp, nonce, echoStr, signature);
      if (0 != ret)
      {
        return ret;
      }
      try
      {
        replyEcho = Cryptography.AesDecrypt(echoStr, encodingAesKey, out var cpid);

        if (cpid != receiveId)
        {
          replyEcho = "";
          return (int)MessageCryptoErrorCode.ValidateCorpIdError;
        }
      }
      catch (Exception)
      {
        replyEcho = "";
        return (int)MessageCryptoErrorCode.DecryptAesError;
      }
      return 0;
    }

    // 检验消息的真实性，并且获取解密后的明文
    // @param sMsgSignature: 签名串，对应URL参数的msg_signature
    // @param sTimeStamp: 时间戳，对应URL参数的timestamp
    // @param sNonce: 随机串，对应URL参数的nonce
    // @param sPostData: 密文，对应POST请求的数据
    // @param sMsg: 解密后的原文，当return返回0时有效
    // @return: 成功0，失败返回对应的错误码
    public int DecryptMsg(string signature, string timestamp, string nonce, string postData, out string? result)
    {
      result = null;
      if (encodingAesKey.Length != 43)
      {
        return (int)MessageCryptoErrorCode.IllegalAesKeyError;
      }
      XmlDocument doc = new XmlDocument();
      XmlNode root;
      string sEncryptMsg;
      try
      {
        doc.LoadXml(postData);
        root = doc.FirstChild!;
        sEncryptMsg = root["Encrypt"]!.InnerText;
      }
      catch (Exception)
      {
        return (int)MessageCryptoErrorCode.ParseXmlError;
      }
      //verify signature
      var ret = VerifySignature(token, timestamp, nonce, sEncryptMsg, signature);
      if (ret != 0)
        return ret;

      var cpid = "";
      try
      {
        result = Cryptography.AesDecrypt(sEncryptMsg, encodingAesKey, out cpid);
      }
      catch (FormatException)
      {
        result = "";
        return (int)MessageCryptoErrorCode.DecodeBase64Error;
      }
      catch (Exception)
      {
        result = "";
        return (int)MessageCryptoErrorCode.DecryptAesError;
      }
      if (cpid != receiveId)
        return (int)MessageCryptoErrorCode.ValidateCorpIdError;
      return 0;
    }

    //将企业号回复用户的消息加密打包
    // @param sReplyMsg: 企业号待回复用户的消息，xml格式的字符串
    // @param sTimeStamp: 时间戳，可以自己生成，也可以用URL参数的timestamp
    // @param sNonce: 随机串，可以自己生成，也可以用URL参数的nonce
    // @param sEncryptMsg: 加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串,
    //						当return返回0时有效
    // return：成功0，失败返回对应的错误码
    public int EncryptMsg(string reply, string timestamp, string nonce, out string encryptedMsg)
    {
      encryptedMsg = "";
      if (encodingAesKey.Length != 43)
      {
        return (int)MessageCryptoErrorCode.IllegalAesKeyError;
      }
      string raw = "";
      try
      {
        raw = Cryptography.AesEncrypt(reply, encodingAesKey, receiveId);
      }
      catch (Exception)
      {
        return (int)MessageCryptoErrorCode.EncryptAesError;
      }
      int ret = GenarateSignature(token, timestamp, nonce, raw, out var signature);
      if (0 != ret)
        return ret;

      string EncryptLabelHead = "<Encrypt><![CDATA[";
      string EncryptLabelTail = "]]></Encrypt>";
      string MsgSigLabelHead = "<MsgSignature><![CDATA[";
      string MsgSigLabelTail = "]]></MsgSignature>";
      string TimeStampLabelHead = "<TimeStamp><![CDATA[";
      string TimeStampLabelTail = "]]></TimeStamp>";
      string NonceLabelHead = "<Nonce><![CDATA[";
      string NonceLabelTail = "]]></Nonce>";
      encryptedMsg = encryptedMsg + "<xml>" + EncryptLabelHead + raw + EncryptLabelTail;
      encryptedMsg = encryptedMsg + MsgSigLabelHead + signature + MsgSigLabelTail;
      encryptedMsg = encryptedMsg + TimeStampLabelHead + timestamp + TimeStampLabelTail;
      encryptedMsg = encryptedMsg + NonceLabelHead + nonce + NonceLabelTail;
      encryptedMsg += "</xml>";
      return 0;
    }

    public class DictionarySort : IComparer
    {
      public int Compare(object? left, object? right)
      {
        var l = left as string;
        var r = right as string;
        var ll = string.IsNullOrEmpty(l) ? 0 : l.Length;
        var rl = string.IsNullOrEmpty(r) ? 0 : r.Length;
        int index = 0;
        while (index < ll && index < rl)
        {
          if (l![index] < r![index])
            return -1;
          else if (l[index] > r[index])
            return 1;
          else
            index++;
        }
        return ll - rl;
      }
    }

    //Verify Signature
    private static int VerifySignature(string token, string timestamp, string nonce, string encrypted, string signature)
    {
      int ret = GenarateSignature(token, timestamp, nonce, encrypted, out var hash);
      if (ret != 0)
        return ret;
      if (hash == signature)
        return 0;
      else
      {
        return (int)MessageCryptoErrorCode.ValidateSignatureError;
      }
    }

    public static int GenarateSignature(string token, string timestamp, string nonce, string encrypted, out string signature)
    {
      var al = new ArrayList
      {
        token,
        timestamp,
        nonce,
        encrypted
      };
      al.Sort(new DictionarySort());
      string raw = string.Join("", al);

      signature = "";
      try
      {
        var sha = HashAlgorithm.Create("SHA1")!;
        var enc = new ASCIIEncoding();
        var dataToHash = enc.GetBytes(raw);
        var dataHashed = sha.ComputeHash(dataToHash);
        signature = BitConverter.ToString(dataHashed).Replace("-", "");
        signature = signature.ToLower();
      }
      catch (Exception)
      {
        return (int)MessageCryptoErrorCode.ComputeSignatureError;
      }
      return 0;
    }
  }
}