//using System.Web;
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
  public enum MessageCryptoErrorCode
  {
    OK = 0,
    ValidateSignatureError = -40001,
    ParseXmlError = -40002,
    ComputeSignatureError = -40003,
    IllegalAesKeyError = -40004,
    ValidateCorpIdError = -40005,
    EncryptAesError = -40006,
    DecryptAesError = -40007,
    IllegalBufferError = -40008,
    EncodeBase64Error = -40009,
    DecodeBase64Error = -40010
  };
}