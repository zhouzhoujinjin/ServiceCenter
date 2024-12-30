using PureCode.DataTransfer.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PureCode.DataTransfer.Providers
{
  public interface ISheetProvider
  {
    /// <summary>
    /// 文件扩展名
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// 加载模板文件
    /// </summary>
    /// <param name="fileStream">模板文档流</param>
    /// <param name="fieldTitles">解析的属性标题</param>
    /// <param name="addonInfos">其他由模板得到的附加数据</param>
    /// <returns>模板加载结果</returns>
    VerifyStatus LoadTemplate(Stream fileStream, out IEnumerable<string> fieldTitles, out Dictionary<string, object> addonInfos);

    /// <summary>
    /// 从文件流中得到数据信息
    /// </summary>
    /// <param name="input">输入文档流</param>
    /// <param name="hasTitleRow">是否包含标题列</param>
    /// <param name="fields">属性列表</param>
    /// <returns>解析的数据列表</returns>
    ICollection<Dictionary<string, string>> LoadData(Stream input, bool hasTitleRow = true, IEnumerable<Field> fields = null);

    /// <summary>
    /// 从文件流中得到数据信息
    /// </summary>
    /// <param name="input">输入文档流</param>
    /// <param name="dataType">数据的类型</param>
    /// <param name="fields">属性列表</param>
    /// <param name="addonInfos">模板附加数据</param>
    /// <returns>解析的数据列表</returns>
    ICollection<object> LoadData(Stream input, Type dataType, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null);

    /// <summary>
    /// 填充数据到文件流
    /// </summary>
    /// <param name="templateStream">模板文档流</param>
    /// <param name="dataType">数据的类型</param>
    /// <param name="data">数据内容</param>
    /// <param name="fields">属性列表</param>
    /// <param name="addonInfos">模板附加信息</param>
    /// <returns>输出文件流</returns>
    Stream FillData(Stream templateStream, Type dataType, IEnumerable<object> data, IEnumerable<Field> fields, Dictionary<string, object> addonInfos = null);

    /// <summary>
    /// 获得导入导出类的域列表
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    IEnumerable<Field> GetFields(Type type, Field parent = null);
  }
}