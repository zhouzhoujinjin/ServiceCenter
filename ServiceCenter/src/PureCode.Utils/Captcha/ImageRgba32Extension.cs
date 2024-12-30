﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace PureCode.Utils.Captcha
{
  public static class ImageRgba32Extension
  {
    public static byte[] ToPngArray(this Image img)
    {
      using var ms = new MemoryStream();
      img.Save(ms, PngFormat.Instance);
      return ms.ToArray();
    }

    public static byte[] ToGifArray(this Image img)
    {
      using var ms = new MemoryStream();
      img.Save(ms, new GifEncoder());
      return ms.ToArray();
    }
  }
}