using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net.Http;
using System.Web.Http;
using tcs.lib;

namespace tcs.api.Controllers
{
    public class LINEOAuth2Controller : BaseApiController
    {
        public IHttpActionResult Get()
        {
            var content = Request.Content.ReadAsStringAsync();
            content.Wait();
            LogHelper.Error(content.Result, logName: "webapi");
            return Ok(content.Result);
        }

        public HttpResponseMessage Get(string code, string state)
        {
            try
            {
                var visa_state = ConfigMgr.Get<string>("visa_state");
                var general_state = ConfigMgr.Get<string>("general_state");

                if (state != visa_state &&
                    state != general_state)
                    return APIResponseMessage.BadRequest();

                var client = new RestClient(ConfigMgr.Get<string>("token_uri"));
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.Parameters.Clear();
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", ConfigMgr.Get<string>("WebAPI") + "/lineoauth2");

                //if (state == visa_state)
                //{
                //    request.AddParameter("client_id", ConfigMgr.Get<string>("visa_client_id"));
                //    request.AddParameter("client_secret", ConfigMgr.Get<string>("visa_client_secret"));
                //}
                //if (state == general_state)
                //{
                    request.AddParameter("client_id", ConfigMgr.Get<string>("general_client_id"));
                    request.AddParameter("client_secret", ConfigMgr.Get<string>("general_client_secret"));
                //}

                IRestResponse response = client.Execute(request);
                LogHelper.Error(response.Content);
                return APIResponseMessage.Success(JsonConvert.DeserializeObject(response.Content));
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message, e, logName: "webapi");
                return APIResponseMessage.InternalServerError(e);
            }
        }

        public IHttpActionResult Get(string error, string state, string error_description)
        {
            var content = Request.Content.ReadAsStringAsync();
            content.Wait();
            LogHelper.Error(content.Result, logName: "webapi");
            return Ok(new { content, error, state, error_description });
        }
    }
}
