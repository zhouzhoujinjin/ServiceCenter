using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PureCode.WeChat.Core
{
  public class Cryptography
  {
    public static uint HostToNetworkOrder(uint inval)
    {
      var outval = 0u;
      for (int i = 0; i < 4; i++)
        outval = (outval << 8) + ((inval >> (i * 8)) & 255);
      return outval;
    }

    public static int HostToNetworkOrder(int inval)
    {
      var outval = 0;
      for (int i = 0; i < 4; i++)
        outval = (outval << 8) + ((inval >> (i * 8)) & 255);
      return outval;
    }

    /// <summary>
    /// 解密方法
    /// </summary>
    /// <param name="input">密文</param>
    /// <param name="encodingAesKey"></param>
    /// <returns></returns>
    ///
    public static string AesDecrypt(string input, string encodingAesKey, out string corpId)
    {
      var key = Convert.FromBase64String(encodingAesKey + "=");
      var iv = new byte[16];
      Array.Copy(key, iv, 16);
      byte[] tmpMsg = AesDecrypt(input, iv, key);

      int len = BitConverter.ToInt32(tmpMsg, 16);
      len = IPAddress.NetworkToHostOrder(len);

      var bMsg = new byte[len];
      var bCorpid = new byte[tmpMsg.Length - 20 - len];
      Array.Copy(tmpMsg, 20, bMsg, 0, len);
      Array.Copy(tmpMsg, 20 + len, bCorpid, 0, tmpMsg.Length - 20 - len);
      var oriMsg = Encoding.UTF8.GetString(bMsg);
      corpId = Encoding.UTF8.GetString(bCorpid);

      return oriMsg;
    }

    public static string AesEncrypt(string input, string encodingAesKey, string corpId)
    {
      var key = Convert.FromBase64String(encodingAesKey + "=");
      var iv = new byte[16];
      Array.Copy(key, iv, 16);
      var randcode = CreateRandCode(16);
      var rand = Encoding.UTF8.GetBytes(randcode);
      var corpIdBytes = Encoding.UTF8.GetBytes(corpId);
      var tmpMsg = Encoding.UTF8.GetBytes(input);
      var msgLen = BitConverter.GetBytes(HostToNetworkOrder(tmpMsg.Length));
      var msg = new byte[rand.Length + msgLen.Length + corpIdBytes.Length + tmpMsg.Length];

      Array.Copy(rand, msg, rand.Length);
      Array.Copy(msgLen, 0, msg, rand.Length, msgLen.Length);
      Array.Copy(tmpMsg, 0, msg, rand.Length + msgLen.Length, tmpMsg.Length);
      Array.Copy(corpIdBytes, 0, msg, rand.Length + msgLen.Length + tmpMsg.Length, corpIdBytes.Length);

      return AesEncrypt(msg, iv, key);
    }

    private static string CreateRandCode(int codeLen)
    {
      var codeSerial = "2,3,4,5,6,7,a,c,d,e,f,h,i,j,k,m,n,p,r,s,t,A,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,U,V,W,X,Y,Z";
      if (codeLen == 0)
      {
        codeLen = 16;
      }
      var arr = codeSerial.Split(',');
      var code = "";
      var rand = new Random(unchecked((int)DateTime.Now.Ticks));
      for (int i = 0; i < codeLen; i++)
      {
        int randValue = rand.Next(0, arr.Length - 1);
        code += arr[randValue];
      }
      return code;
    }

    private static string AesEncrypt(string input, byte[] iv, byte[] key)
    {
      var aes = Aes.Create("AesManaged")!;
      //秘钥的大小，以位为单位
      aes.KeySize = 256;
      //支持的块大小
      aes.BlockSize = 128;
      //填充模式
      aes.Padding = PaddingMode.PKCS7;
      aes.Mode = CipherMode.CBC;
      aes.Key = key;
      aes.IV = iv;
      var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
      var buff = default(byte[]);

      using (var ms = new MemoryStream())
      {
        using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
        {
          var xml = Encoding.UTF8.GetBytes(input);
          cs.Write(xml, 0, xml.Length);
        }
        buff = ms.ToArray();
      }
      return Convert.ToBase64String(buff);
    }

    private static string AesEncrypt(byte[] input, byte[] iv, byte[] key)
    {
      var aes = Aes.Create("AesManaged")!;
      //秘钥的大小，以位为单位
      aes.KeySize = 256;
      //支持的块大小
      aes.BlockSize = 128;
      //填充模式
      //aes.Padding = PaddingMode.PKCS7;
      aes.Padding = PaddingMode.None;
      aes.Mode = CipherMode.CBC;
      aes.Key = key;
      aes.IV = iv;
      var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
      var buff = default(byte[]);

      #region 自己进行PKCS7补位，用系统自己带的不行

      var msg = new byte[input.Length + 32 - input.Length % 32];
      Array.Copy(input, msg, input.Length);
      var pad = KCS7Encoder(input.Length);
      Array.Copy(pad, 0, msg, input.Length, pad.Length);

      #endregion 自己进行PKCS7补位，用系统自己带的不行

      using (var ms = new MemoryStream())
      {
        using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
        {
          cs.Write(msg, 0, msg.Length);
        }
        buff = ms.ToArray();
      }

      return Convert.ToBase64String(buff);
    }

    private static byte[] KCS7Encoder(int textLength)
    {
      var block_size = 32;
      // 计算需要填充的位数
      var amount_to_pad = block_size - (textLength % block_size);
      if (amount_to_pad == 0)
      {
        amount_to_pad = block_size;
      }
      // 获得补位所用的字符
      var pad_chr = chr(amount_to_pad);
      var tmp = "";
      for (int index = 0; index < amount_to_pad; index++)
      {
        tmp += pad_chr;
      }
      return Encoding.UTF8.GetBytes(tmp);
    }

    /**
     * 将数字转化成ASCII码对应的字符，用于对明文进行补码
     *
     * @param a 需要转化的数字
     * @return 转化得到的字符
     */

    private static char chr(int a)
    {
      var target = (byte)(a & 0xFF);
      return (char)target;
    }

    private static byte[] AesDecrypt(string input, byte[] iv, byte[] key)
    {
      var aes = Aes.Create("AesManaged")!;
      aes.KeySize = 256;
      aes.BlockSize = 128;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.None;
      aes.Key = key;
      aes.IV = iv;
      var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
      var buff = default(byte[]);
      using (var ms = new MemoryStream())
      {
        using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
        {
          var xml = Convert.FromBase64String(input);
          var msg = new byte[xml.Length + 32 - xml.Length % 32];
          Array.Copy(xml, msg, xml.Length);
          cs.Write(xml, 0, xml.Length);
        }
        buff = decode2(ms.ToArray());
      }
      return buff;
    }

    private static byte[] decode2(byte[] decrypted)
    {
      var pad = (int)decrypted[decrypted.Length - 1];
      if (pad < 1 || pad > 32)
      {
        pad = 0;
      }
      var res = new byte[decrypted.Length - pad];
      Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
      return res;
    }
  }
}