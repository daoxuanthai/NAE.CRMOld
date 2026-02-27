using System;
using System.IO;
using System.Web.Caching;
using System.Web.Mvc;

namespace tcs.lib
{
    public static class JsExtension
    {
        public static MvcHtmlString IncludeVersionedJs(this HtmlHelper helper, string filename)
        {
            if(!helper.ViewContext.RequestContext.HttpContext.Request.IsLocal || ConfigMgr.IsLiveEnv)
            {
                if (!filename.StartsWith("/"))
                    filename = "/" + filename;

                filename = filename.Replace(".js", ".min.js");

                string version = GetVersion(helper, filename);

                filename = filename.Replace(".js", version + ".js");
            }
            return MvcHtmlString.Create("<script type='text/javascript' defer async src='" + ConfigMgr.Cdn + filename + "'></script>");
        }

        private static string GetVersion(this HtmlHelper helper, string filename)
        {
            var clearCache = helper.ViewContext.RequestContext.HttpContext.Request["clearcache"] != null;
            var context = helper.ViewContext.RequestContext.HttpContext;

            if (context.Cache[filename] == null || clearCache)
            {
                var cdnPath = ConfigMgr.ScriptCdnPath;

                var physicalPath = cdnPath + filename.Replace("/","\\");
                var version = ".v" +
                  new FileInfo(physicalPath).LastWriteTime
                    .ToString("yyyyMMddhhmmss");

                var vfile = physicalPath.Replace(".js", version + ".js");
                if(!File.Exists(vfile))
                {
                    File.Copy(physicalPath, vfile);
                }

                context.Cache.Add(physicalPath, version, null,
                  DateTime.Now.AddMonths(12), TimeSpan.Zero,
                  CacheItemPriority.Normal, null);
                context.Cache[physicalPath] = version;
                return version;
            }

            return context.Cache[filename] as string;
        }
    }
}