using System.Net;
using System.Web.Http;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Text;
using tcs.lib;
using SmartFormat;

namespace tcs.api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// LINE Notify: https://notify-bot.line.me/doc/en/
    /// </summary>
    public class LINENotifyController : ApiController
    {
        protected string access_token;
        protected dynamic msg_lib;

        public LINENotifyController()
        {
            access_token = ConfigMgr.Get<string>("general_access_token");
            msg_lib = JsonConvert.DeserializeObject<dynamic>(
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/general_msg.json"), Encoding.UTF8));
        }

        /// <summary>
        /// POST a notify
        /// </summary>
        /// <param name="msg">The notified message</param>
        /// <returns></returns>
        public IHttpActionResult Post(string msg, string access_token)
        {
            try
            {
                if (string.IsNullOrEmpty(msg))
                    return Ok("The notified message is NOT empty");

                var client = new RestClient(ConfigMgr.Get<string>("notify_uri"))
                {
                    Authenticator =
                        new OAuth2AuthorizationRequestHeaderAuthenticator(access_token, "Bearer")
                };
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.Parameters.Clear();
                request.AddParameter("message", msg);
                IRestResponse response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    LogHelper.Error("Lỗi LINE Notify", response, "webapi");
                }
                return Ok(new { res = JsonConvert.DeserializeObject(response.Content), msg });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message, ex);
                throw ex;
            }
        }

        public async Task<IHttpActionResult> Post()
        {
            try
            {
                if (string.IsNullOrEmpty(access_token))
                    return Ok("access_token is NULL");

                var result = await Request.Content.ReadAsStringAsync();
                var req = JsonConvert.DeserializeObject<dynamic>(result);
                var msg = GetMsg(req);
                if (string.IsNullOrEmpty(msg))
                    return Ok("The message is NULL");

                return Post(msg, access_token);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message, ex);
                throw;
            }
            
        }

        protected virtual string GetMsg(dynamic data)
        {
            if (data is string)
            {
                return Convert.ToString(data);
            }
            return null;
        }

        public static string GetMsg(dynamic messages, string msgId, dynamic args, string lang = "en")
        {
            try
            {
                foreach (var m in messages.messages)
                {
                    if (m.id != msgId)
                        continue;

                    var msg = lang == "vi" ? m.msg.vi.ToString() : m.msg.en.ToString();
                    var matches = Regex.Matches(msg, "0[xX][0-9a-fA-F]+");
                    foreach (var match in matches)
                    {
                        int intCode = Convert.ToInt32(match.ToString(), 16);
                        var code = char.ConvertFromUtf32(intCode);
                        msg = msg.Replace(match.ToString(), code);
                    }

                    return Smart.Format(msg, args);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message, ex);
                throw;
            }
            
            return null;
        }
    }
}
