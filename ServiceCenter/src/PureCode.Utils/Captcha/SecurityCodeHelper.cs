﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PureCode.Utils.Captcha
{
  public class SecurityCodeHelper
  {
    /// <summary>
    /// 验证码文本池
    /// </summary>
    private static readonly string[] CnTextArr =
    [
      "的", "一", "国", "在", "人", "了", "有", "中", "是", "年", "和", "大", "业", "不", "为", "发", "会", "工", "经", "上", "地", "市", "要",
      "个", "产", "这", "出", "行", "作", "生", "家", "以", "成", "到", "日", "民", "来", "我", "部", "对", "进", "多", "全", "建", "他", "公",
      "开", "们", "场", "展", "时", "理", "新", "方", "主", "企", "资", "实", "学", "报", "制", "政", "济", "用", "同", "于", "法", "高", "长",
      "现", "本", "月", "定", "化", "加", "动", "合", "品", "重", "关", "机", "分", "力", "自", "外", "者", "区", "能", "设", "后", "就", "等",
      "体", "下", "万", "元", "社", "过", "前", "面", "农", "也", "得", "与", "说", "之", "员", "而", "务", "利", "电", "文", "事", "可", "种",
      "总", "改", "三", "各", "好", "金", "第", "司", "其", "从", "平", "代", "当", "天", "水", "省", "提", "商", "十", "管", "内", "小", "技",
      "位", "目", "起", "海", "所", "立", "已", "通", "入", "量", "子", "问", "度", "北", "保", "心", "还", "科", "委", "都", "术", "使", "明",
      "着", "次", "将", "增", "基", "名", "向", "门", "应", "里", "美", "由", "规", "今", "题", "记", "点", "计", "去", "强", "两", "些", "表",
      "系", "办", "教", "正", "条", "最", "达", "特", "革", "收", "二", "期", "并", "程", "厂", "如", "道", "际", "及", "西", "口", "京", "华",
      "任", "调", "性", "导", "组", "东", "路", "活", "广", "意", "比", "投", "决", "交", "统", "党", "南", "安", "此", "领", "结", "营", "项",
      "情", "解", "议", "义", "山", "先", "车", "然", "价", "放", "世", "间", "因", "共", "院", "步", "物", "界", "集", "把", "持", "无", "但",
      "城", "相", "书", "村", "求", "治", "取", "原", "处", "府", "研", "质", "信", "四", "运", "县", "军", "件", "育", "局", "干", "队", "团",
      "又", "造", "形", "级", "标", "联", "专", "少", "费", "效", "据", "手", "施", "权", "江", "近", "深", "更", "认", "果", "格", "几", "看",
      "没", "职", "服", "台", "式", "益", "想", "数", "单", "样", "只", "被", "亿", "老", "受", "优", "常", "销", "志", "战", "流", "很", "接",
      "乡", "头", "给", "至", "难", "观", "指", "创", "证", "织", "论", "别", "五", "协", "变", "风", "批", "见", "究", "支", "那", "查", "张",
      "精", "每", "林", "转", "划", "准", "做", "需", "传", "争", "税", "构", "具", "百", "或", "才", "积", "势", "举", "必", "型", "易", "视",
      "快", "李", "参", "回", "引", "镇", "首", "推", "思", "完", "消", "值", "该", "走", "装", "众", "责", "备", "州", "供", "包", "副", "极",
      "整", "确", "知", "贸", "己", "环", "话", "反", "身", "选", "亚", "么", "带", "采", "王", "策", "真", "女", "谈", "严", "斯", "况", "色",
      "打", "德", "告", "仅", "它", "气", "料", "神", "率", "识", "劳", "境", "源", "青", "护", "列", "兴", "许", "户", "马", "港", "则", "节",
      "款", "拉", "直", "案", "股", "光", "较", "河", "花", "根", "布", "线", "土", "克", "再", "群", "医", "清", "速", "律", "她", "族", "历",
      "非", "感", "占", "续", "师", "何", "影", "功", "负", "验", "望", "财", "类", "货", "约", "艺", "售", "连", "纪", "按", "讯", "史", "示",
      "象", "养", "获", "石", "食", "抓", "富", "模", "始", "住", "赛", "客", "越", "闻", "央", "席", "坚"
    ];

    private static readonly string[] EnTextArr =
    {
      "a", "b", "c", "d", "e", "f", "g", "h", "k", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A",
      "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };

    /// <summary>
    /// 验证码图片宽高
    /// </summary>
    private const int ImageWidth = 160;

    private const int ImageHeight = 50;

    /// <summary>
    /// 泡泡数量
    /// </summary>
    private int _circleCount = 10;

    /// <summary>
    /// 泡泡半径范围
    /// </summary>
    private const int MiniCircleR = 2;

    private const int MaxCircleR = 8;

    /// <summary>
    /// 颜色池,较深的颜色
    /// https://tool.oschina.net/commons?type=3
    /// </summary>
    private static readonly string[] ColorHexArr = new string[] { "#00E5EE", "#000000", "#2F4F4F", "#000000", "#43CD80", "#191970", "#006400", "#458B00", "#8B7765", "#CD5B45" };

    ///较浅的颜色
    private static readonly string[] LightColorHexArr = new string[] { "#FFFACD", "#FDF5E6", "#F0FFFF", "#BBFFFF", "#FAFAD2", "#FFE4E1", "#DCDCDC", "#F0E68C" };

    private static readonly Random Random = new();

    /// <summary>
    /// 字体池
    /// </summary>
    private static readonly Dictionary<int, Font[]> CachedFonts = [];

    public SecurityCodeHelper()
    {
    
    }

    /// <summary>
    /// 生成随机中文字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public string GetRandomCnText(int length)
    {
      StringBuilder sb = new();
      if (length > 0)
      {
        do
        {
          sb.Append(CnTextArr[Random.Next(0, CnTextArr.Length)]);
        }
        while (--length > 0);
      }
      return sb.ToString();
    }

    /// <summary>
    /// 生成随机英文字母/数字组合字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public string GetRandomEnDigitalText(int length)
    {
      StringBuilder sb = new();
      if (length <= 0) return sb.ToString();
      do
      {
        if (Random.Next(0, 2) > 0)
        {
          sb.Append(Random.Next(2, 10));
        }
        else
        {
          sb.Append(EnTextArr[Random.Next(0, EnTextArr.Length)]);
        }
      }
      while (--length > 0);
      return sb.ToString();
    }

    /// <summary>
    /// 获取泡泡样式验证码
    /// </summary>
    /// <param name="text">2-3个文字，中文效果较好</param>
    /// <param name="width">验证码宽度，默认宽度100px，可根据传入</param>
    /// <param name="format"></param>
    /// <returns>验证码图片字节数组</returns>
    public byte[] GetBubbleCodeByte(string text)
    {
      using var img = new Image<Rgba32>(ImageWidth, ImageHeight);
      var fonts = GetFonts(ImageHeight);
      var textFont = fonts.FirstOrDefault(c => "Xiaolai SC".Equals(c.Name, StringComparison.CurrentCultureIgnoreCase)) ??
                     fonts[Random.Next(0, fonts.Length)];

      var colorCircleHex = ColorHexArr[Random.Next(0, ColorHexArr.Length)];
      var colorTextHex = colorCircleHex;

      if (Random.Next(0, 6) == 3)
      {
        colorCircleHex = "#FFFFFF";//白色
        _circleCount = (int)(_circleCount * 2.65);
      }

      img.Mutate(ctx => ctx
        .DrawingCnText(ImageWidth, ImageHeight, text, Rgba32.ParseHex(colorTextHex), textFont)
        .DrawingCircles(ImageWidth, ImageHeight, _circleCount, MiniCircleR, MaxCircleR, Rgba32.ParseHex(colorCircleHex))
      );

      using var ms = new MemoryStream();
      img.Save(ms, PngFormat.Instance);
      return ms.ToArray();
    }

    /// <summary>
    /// 获取动态(gif)泡泡验证码
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public byte[] GetGifBubbleCodeByte(string text)
    {
      var gifCircleCount = (int)(_circleCount * 1.5);
      var color = Rgba32.ParseHex(ColorHexArr[Random.Next(0, ColorHexArr.Length)]);
      var fonts = GetFonts(ImageHeight);
      var textFont = fonts.FirstOrDefault(c => "Xiaolai SC".Equals(c.Name, StringComparison.CurrentCultureIgnoreCase)) ??
                     fonts[Random.Next(0, fonts.Length)];

      var img = new Image<Rgba32>(ImageWidth, ImageHeight);

      img.Mutate(ctx => ctx
                    .Fill(Rgba32.ParseHex("#FFF"))
                    .DrawingCircles(ImageWidth, ImageHeight, gifCircleCount, MiniCircleR, MaxCircleR, color)
                 );

      for (int i = 0; i < 5; i++)
      {
        using var tempImg = new Image<Rgba32>(ImageWidth, ImageHeight);
        tempImg.Frames[0].Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay = Random.Next(20, 50);
        tempImg.Mutate(ctx => ctx
          .Fill(Rgba32.ParseHex("#FFF"))
          .DrawingCircles(ImageWidth, ImageHeight, gifCircleCount, MiniCircleR, MaxCircleR, color)
        );
        img.Frames.AddFrame(tempImg.Frames[0]);
      }
      img.Frames[0].Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay = Random.Next(20, 50);
      img.Mutate(ctx => ctx.DrawingCnText(ImageWidth, ImageHeight, text, color, textFont));
      return img.ToGifArray();
    }

    /// <summary>
    /// 英文字母+数字组合验证码
    /// </summary>
    /// <param name="text"></param>
    /// <returns>验证码图片字节数组</returns>
    public byte[] GetEnDigitalCodeByte(string text)
    {
      using var img = GetEnDigitalCodeImage(text);
      return img.ToGifArray();
    }

    /// <summary>
    /// 动态(gif)数字字母组合验证码
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public byte[] GetGifEnDigitalCodeByte(string text)
    {
      using var img = GetEnDigitalCodeImage(text);
      for (int i = 0; i < 5; i++)
      {
        using var tempImg = GetEnDigitalCodeImage(text);
        tempImg.Frames[0].Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay = Random.Next(80, 150);
        img.Frames.AddFrame(tempImg.Frames[0]);
      }
      img.Frames[0].Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay = Random.Next(200, 400);
      return img.ToGifArray();
    }

    /// <summary>
    /// 生成一个数组组合验证码素材（Image）
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static Image GetEnDigitalCodeImage(string text)
    {
      var img = new Image<Rgba32>(ImageWidth, ImageHeight);
      var colorTextHex = ColorHexArr[Random.Next(0, ColorHexArr.Length)];
      var lightenColorHex = LightColorHexArr[Random.Next(0, LightColorHexArr.Length)];

      img.Mutate(ctx => ctx.BackgroundColor(Rgba32.ParseHex(LightColorHexArr[Random.Next(0, LightColorHexArr.Length)]))
                  .Glow(Rgba32.ParseHex(lightenColorHex))
                  .DrawGrid(ImageWidth, ImageHeight, Rgba32.ParseHex(lightenColorHex), 8, 1)
                  .DrawingEnText(ImageWidth, ImageHeight, text, ColorHexArr, GetFonts(ImageHeight))
                  .GaussianBlur(0.4f)
                  .DrawingCircles(ImageWidth, ImageHeight, 15, MiniCircleR, MaxCircleR, Rgba32.ParseHex("#FFF"))
              );
      return img;
    }

    /// <summary>
    /// 初始化字体池
    /// </summary>
    /// <param name="fontSize">一个初始大小</param>
    private static Font[] GetFonts(int fontSize)
    {
      fontSize = (int)(fontSize / 1.5);
      if (CachedFonts.TryGetValue(fontSize, out var fonts))
      {
        return fonts;
      }

      string fontDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "fonts");

      if (!Directory.Exists(fontDir)) throw new Exception($"绘制验证码字体文件不存在，请将字体文件(.ttf)复制到目录：{fontDir}");
      var fontFiles = Directory.GetFiles(fontDir, "*.ttf");

      var temp = CachedFonts[fontSize] = new Font[fontFiles.Length];

      var fontCollection = new FontCollection();
      var i = 0;
      foreach (var fontFile in fontFiles)
      {
        temp[i] = new Font(fontCollection.Add(fontFile), fontSize);
        i++;
      }

      return CachedFonts[fontSize];

    }
  }

}