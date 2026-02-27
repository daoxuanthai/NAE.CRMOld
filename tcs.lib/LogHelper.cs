using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace tcs.lib
{
    public static class LogHelper
    {
        public static void Error(string msg = null, object data = null, string logName = "", bool sendNotify = true)
        {
            WriteLog("Error." + msg, data);
            if (sendNotify)
                PostNotify("Error: " + msg + " - data: " + (data != null ? Newtonsoft.Json.JsonConvert.SerializeObject(data) : "null"));
        }

        public static void Warning(string msg = null, object data = null, string logName = "", bool sendNotify = false)
        {
            WriteLog("Warning." + msg, data);
            if (sendNotify)
                PostNotify("Warning: " + msg + " - data: " + (data != null ? Newtonsoft.Json.JsonConvert.SerializeObject(data) : "null"));
        }

        #region LINE Notify

        /// <summary>
        /// Gửi một notify lên LINE
        /// </summary>
        /// <param name="content"></param>
        /// <param name="chanel"></param>
        public static void PostNotify(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            PostDiscordNotify(message, ConfigMgr.DiscordWebhooks);
        }

        public static void PostNotify(string message, string chatId)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(chatId))
                return;

            PostDiscordNotify(message, chatId);
        }

        #endregion

        private static async Task PostDiscordNotify(string message, string chatId)
        {
            var url = $"https://discord.com/api/webhooks/{chatId}"; // Thay bằng webhook của bạn
            var json = $"{{\"content\":\"{message}\"}}";
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(url, content);
            }
        }

        public static void WriteLog(string message, object data)
        {
            if (data == null)
                data = new object();

            Type type = data.GetType();
            Exception exp;

            string errorDate = string.Concat(DateTime.Now.ToString("dd-MM-yyyy"));
            string errorPath = AppDomain.CurrentDomain.BaseDirectory;
            errorPath = Path.Combine(errorPath, "ErrorLog");
            errorPath = Path.Combine(errorPath, errorDate);
            if (!Directory.Exists(errorPath))
            {
                Directory.CreateDirectory(errorPath);
            }
            string errorFilename = string.Concat(DateTime.Now.ToString("dd-MM-yyyy"), ".txt");
            string fullLogPath = Path.Combine(errorPath, errorFilename);

            if (type.Name.Contains("Exception"))
            {
                exp = (Exception)data;
                try
                {
                    StackTrace stackTrace = new StackTrace();
                    string methodName = stackTrace.GetFrame(1).GetMethod().Name;
                    string content = string.Concat(DateTime.Now.ToString(), " -- ", message, "----Object[Method] : ", methodName, "-----Message : ", exp.Message, "-----Source : ", exp.StackTrace, Environment.NewLine);
                    File.AppendAllText(fullLogPath, content, System.Text.Encoding.UTF8);

                    PostNotify(content);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                try
                {
                    string content = string.Concat(DateTime.Now.ToString(), "-----Message : ", data, Environment.NewLine);
                    File.AppendAllText(fullLogPath, content, System.Text.Encoding.UTF8);

                    PostNotify(content);
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
