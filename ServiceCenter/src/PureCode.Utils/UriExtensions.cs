using System;
using System.Collections.Generic;
using System.Web;

namespace PureCode.Utils
{
  public static class UriExtensions
  {
    public static Uri AddQueryString(this Uri uri, IDictionary<string, string>? args = null)
    {
      if (args == null)
      {
        return uri;
      }

      var uriBuilder = new UriBuilder(uri);
      var query = HttpUtility.ParseQueryString(uriBuilder.Query);
      foreach (var kv in args)
      {
        query[kv.Key] = kv.Value;
        uriBuilder.Query = query.ToString();
      }

      return uriBuilder.Uri;
    }
  }
}