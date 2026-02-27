using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tcs.api.Controllers
{
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// Lớp trả về kết quả khi gọi một API.
        /// Luôn trả về dữ liệu có dạng { StatusCode: int, Type: int, Value: string }
        /// StatusCode: HttpCodeResponse
        /// Type: quy định theo từng chức năng
        /// Value: string - dữ liệu json được parse từ một object
        /// </summary>
        public class APIResponseMessage
        {            
            public static HttpResponseMessage Success(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.OK, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.OK
                };
            }
            
            public static HttpResponseMessage Unauthorized(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.Unauthorized, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }
            
            public static HttpResponseMessage BadRequest(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.BadRequest, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            
            public static HttpResponseMessage InternalServerError(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.InternalServerError, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            
            public static HttpResponseMessage NotModified(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.NotModified, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.NotModified
                };
            }
            
            public static HttpResponseMessage RequestTimeout(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.RequestTimeout, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.RequestTimeout
                };
            }
            
            public static HttpResponseMessage NullData(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.NoContent, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            
            public static HttpResponseMessage FunctionBusy(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.RequestTimeout, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.RequestTimeout
                };
            }
            
            public static HttpResponseMessage Duplicate(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.RequestTimeout, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.RequestTimeout
                };
            }
            
            public static HttpResponseMessage NotImplemented(object _object = null, int _type = 1)
            {
                return new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new { StatusCode = HttpStatusCode.NotImplemented, Type = _type, Value = JsonConvert.SerializeObject(_object) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.NotImplemented
                };
            }
        }
    }
}
