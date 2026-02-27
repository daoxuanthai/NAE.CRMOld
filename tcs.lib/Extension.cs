using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace tcs.lib
{
    public static class Extension
    {
        #region Enumerable

        /// <summary>
        /// Chuyển một chuỗi dạng , về mảng int
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string val)
        {
            try
            {
                if (string.IsNullOrEmpty(val))
                    return new int[0];
                val = val.Trim(',').Replace(" ", "").Trim();
                if (!val.IsValidatedCommaList())
                    return new int[0];
                var a = val.Split(',');
                var l = new List<int>();
                foreach (var x in a)
                {
                    int y;
                    if (int.TryParse(x.Trim(), out y))
                        l.Add(y);
                }
                return l.ToArray();
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#else
                return new int[0];
#endif
            }
        }
        
        /// <summary>
        /// Nhân bản một List bất kỳ
        /// </summary>
        /// <typeparam name="T">Class của object được chứa trong list</typeparam>
        /// <param name="listToClone">List cần nhân bản</param>
        /// <returns>List nhân bản</returns>
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        #endregion

        #region Json & Data

        /// <summary>
        /// Kiểu JSON result để sử dụng cho các AJAX request cần trả về dữ liệu JSON 
        /// </summary>
        [Serializable]
        public class JsonResult : System.Web.Mvc.JsonResult
        {
            private readonly HttpStatusCode _code;
            private readonly string _msg;

            /// <summary>
            /// Trạng thái của dữ liệu trả về. Ở đây tận dụng HttpStatusCode để xác định giá trị.
            /// Ví dụ: 200: Xử lý dữ liệu thàng công, 404: Không tìm thấy dữ liệu, 500: Có lỗi trong quá trình xử lý dữ liệu...
            /// </summary>
            public HttpStatusCode Code
            {
                get { return _code; }
            }

            /// <summary>
            /// Nội dung phản hồi về cho client luôn là một String (nếu là object sẽ được serialize thành JSON)
            /// </summary>
            public string Msg
            {
                get { return _msg; }
            }

            /// <summary>
            /// Khởi tạo với nội dung trả về client là văn bản
            /// </summary>
            /// <param name="code">Mã phản hồi</param>
            /// <param name="msg">Nội dung văn bản</param>
            public JsonResult(HttpStatusCode code, string msg)
            {
                _code = code;
                _msg = msg;

                ContentEncoding = Encoding.UTF8;
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                Data = new
                {
                    Code = code,
                    Msg = msg
                };
            }

            /// <summary>
            /// Khởi tạo với nội dung trả về client là JSON. Truyền vào object nó sẽ tự động được serialize.
            /// </summary>
            /// <param name="code"></param>
            /// <param name="data"></param>
            public JsonResult(HttpStatusCode code, object data)
            {
                _code = code;
                _msg = JsonConvert.SerializeObject(data);

                ContentEncoding = Encoding.UTF8;
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                Data = new
                {
                    Code = code,
                    Msg = _msg
                };
            }

            /// <summary>
            /// Khởi tạo, chỉ trả về client trạng thái không có nội dung nào cả
            /// </summary>
            /// <param name="code"></param>
            public JsonResult(HttpStatusCode code)
            {
                _code = code;

                ContentEncoding = Encoding.UTF8;
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                Data = new { Code = code };
            }
        }

        #endregion
        
        #region String

        /// <summary>
        /// Kiểm tra xem chuỗi có phải ở dạng ID,ID,ID... hay không?
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidatedCommaList(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            // Không chứa dấu , thì hiển nhiên tách được
            if (!s.Contains(','))
                return true;
            s = s.Trim(',').Trim();
            return Regex.IsMatch(s, "^([0-9]+,)*[0-9]+$");
        }

        /// <summary>
        /// Chuyển một danh sách số nguyên về dạng chuỗi cách nhau bởi dấu phẩy
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string ToCommaList(this IEnumerable<int> l)
        {
            if (l.Any())
                return l.Aggregate(string.Empty, (current, i) => current + ("," + i)).Trim(',');
            return string.Empty;
        }

        /// <summary>
        /// Lấy dạng URL của một chuỗi bất kỳ
        /// </summary>
        /// <param name="phrase">Chuỗi cần chuyển thành URl</param>
        /// <returns>Chuối dạng URL</returns>
        public static string ToUrl(this string phrase)
        {
            string str = phrase.Replace(",", " ").ToUnsignedVietnamese().RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-/?:]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 150 ? str.Length : 150).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            str = str.Replace("?", "");

            return str;
        }

        /// <summary>
        /// Chuyển mãi về chuẩn ASCII để đảm bảo loại tất cả các dấu đặc biệt
        /// </summary>
        /// <param name="txt">Chuỗi cần chuyển</param>
        /// <returns>Chuỗi đã mã hóa</returns>
        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes).Replace("-", " ").Replace("/", " ");
        }

        /// <summary>
        /// Bỏ dấu tiếng Việt của một chuỗi bất kỳ
        /// </summary>
        /// <param name="vietnamese">Chuỗi tiếng Việt cần bỏ dấu</param>
        /// <returns>Chuỗi đã khử dấu</returns>
        public static string ToUnsignedVietnamese(this string vietnamese)
        {
            if (vietnamese == null) return string.Empty;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = vietnamese.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /// <summary>
        /// Chuyển chuỗi ký tự thành thông tin để tìm kiếm trên elasticsearch
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSearchInfo(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input.Trim().Replace("  ", " ").Replace(" ", "_").Replace("@", "_").Replace(".", "_").ToLower();
        }

        /// <summary>
        /// Viết hoa các chữ cái đầu của mỗi từ trong một văn bản
        /// </summary>
        /// <param name="value">Chuỗi cần viết hoa</param>
        /// <returns>Chuỗi đã viết hoa</returns>
        public static string ToUpperWords(this string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        /// <summary>
        /// Chuyển một số về dạng tiền tệ
        /// </summary>
        /// <param name="number">Giá trị cần chuyển (int, decimal, double...)</param>
        /// <param name="unit">Đơn vị tính</param>
        /// <returns>String dạng tiền tệ</returns>
        public static string ToCurrency(this object number, string unit = "")
        {
            if (string.IsNullOrEmpty(unit))
                return number.FormatNumber("{0:c}");
            return number.FormatNumber("{0:c}") + "/" + unit.ToLower();
        }

        private static string FormatNumber(this object value, string format)
        {
            if (!value.IsNumeric())
                throw new ArgumentException("\"" + value + "\" is not a number.");

            var result = string.Format(ConfigMgr.DefaultCultureInfo, format, value);
            if (result.StartsWith("0"))
                return string.Empty;
            return result;
        }

        /// <summary>
        /// Chuyển một số về dạng hiển thị có định dạng
        /// </summary>
        /// <param name="number">Giá trị cần chuyển (int, decimal, double...)</param>
        /// <returns>String của số đã được định dạng</returns>
        public static string ToNumberString(this object number)
        {
            return number.FormatNumber("{0:##,#}");
        }

        public static string ToHexString(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return ToHexString(valueBytes);
        }

        public static string ToHexString(this byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "");
        }

        public static bool IsPhoneNumber(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return false;
            phone = phone.Replace("-", "");
            const string phonePattern = @"^((\d){10})$";
            return Regex.IsMatch(phone.Trim(), phonePattern);
        }

        public static bool IsBirthdayYear(this string year)
        {
            var tmp = int.Parse(year);
            return tmp > 1950 && tmp < 2100;
        }

        public static bool IsValidDate(this string date, string format = "dd/MM/yyyy")
        {
            try
            {
                DateTime.ParseExact(date, format, DateTimeFormatInfo.InvariantInfo);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static DateTime ToDateTime(this string date, string format = "dd/MM/yyyy")
        {
            var myDtfi = new DateTimeFormatInfo { ShortDatePattern = format };
            return DateTime.Parse(date, myDtfi);
        }

        public static string ToDateString(this DateTime date, string format = "dd/MM/yyyy")
        {
            return date.ToString(format);
        }

        public static string ToDateTimeString(this DateTime date, string format = "dd/MM/yyyy HH:mm:ss", bool wrap = false)
        {
            var val = date.ToString(format);
            return wrap ? val.Replace(" ", "<br>") : val;
        }

        public static bool IsEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            const string sMailPattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            return Regex.IsMatch(email.Trim(), sMailPattern);
        }

        public static bool IsTaxCode(this string taxtCode)
        {
            try
            {
                taxtCode = taxtCode.Trim();
                if (taxtCode.Trim() == "")
                {
                    return true;
                }
                if (taxtCode.Contains(" "))
                {
                    return false;
                }
                if (taxtCode.Length != 10 &&
                    taxtCode.Length != 14)
                {
                    return false;
                }
                string str = taxtCode.Substring(0, 10);
                if (str == "0000000000")
                {
                    return false;
                }
                Hashtable hashtable = new Hashtable
                {
                    {0, 0x1f},
                    {1, 0x1d},
                    {2, 0x17},
                    {3, 0x13},
                    {4, 0x11},
                    {5, 13},
                    {6, 7},
                    {7, 5},
                    {8, 3}
                };
                decimal num = 0M;
                int num2 = 0;
                foreach (char ch in str)
                {
                    if (num2 == 9)
                    {
                        break;
                    }
                    num += Convert.ToInt32(ch.ToString()) * Convert.ToDecimal(hashtable[num2]);
                    num2++;
                }
                bool bolResult = (Convert.ToDecimal(str.Substring(9, 1)) == (10M - (num % 11M)));
                if (bolResult)
                {
                    if (taxtCode.Length == 14)
                    {
                        str = taxtCode.Substring(10, 4);
                        // Nếu là 13 số thì phải có dấu - sau 10 số
                        bolResult = str[0] == '-' && IsNumber(str.Substring(1, 3));
                    }
                }
                return bolResult;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidCustomerName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            name = name.Trim();
            var requirePattern = @"^[\p{L}][\p{L}\s().'-]*$";
            if (!Regex.IsMatch(name, requirePattern))
                return false;

            var pattern = "^([^\\x00-\\x7F]|[\\w_\\ \\.\\+\\-]){2,50}$";
            return Regex.IsMatch(name, pattern);
        }

        public static bool IsNumber(this string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber))
            {
                return false;
            }
            string strExp = "[0-9]{" + strNumber.Length + "}";
            return Regex.IsMatch(strNumber, strExp);
        }

        public static bool IsNumeric(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetFileName(this string o)
        {
            if (string.IsNullOrEmpty(o))
                return string.Empty;

            if (o.IndexOf("/") >= 0)
                return o.Substring(o.LastIndexOf("/") + 1);

            return string.Empty;
        }

        public static string[] SplitTotArray(this string list, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(list))
                return new string[] { };
            return list.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static List<int> SplitToIntList(this string list, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(list) || list == "-1")
                return new List<int>();
            var tmp = list.Split(new char[] { separator}, StringSplitOptions.RemoveEmptyEntries);
            return tmp.Select(Int32.Parse).ToList();
        }

        public static int[] SplitToIntArr(this string list, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(list) || list == "-1")
                return new int[] { };
            var lst = SplitToIntList(list, separator);
            if(lst == null || !lst.Any())
                return new int[] { };

            return lst.ToArray();
        }

        public static JsonResult ToJsonResult(this Dictionary<string, string> dic)
        {
            return new JsonResult(HttpStatusCode.BadRequest, new
            {
                Error = dic.Select(x => new
                {
                    key = x.Key,
                    msg = x.Value
                })
            });
        }

        public static string TrimSomeSpecialChar(this string mainKeyword)
        {
            if (string.IsNullOrEmpty(mainKeyword))
                return string.Empty;

            if (mainKeyword.Contains(",") || mainKeyword.Contains(".") || mainKeyword.Contains("-") || mainKeyword.Contains("(") ||
                mainKeyword.Contains(")") || mainKeyword.Contains("'") || mainKeyword.Contains("\"") || mainKeyword.Contains("/") ||
                mainKeyword.Contains("+") || mainKeyword.Contains("&") || mainKeyword.Contains(" x ") || mainKeyword.Contains("*") ||
                mainKeyword.Contains(" X "))
            {
                return mainKeyword.Replace(".", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("/", "").Replace("+", "").Replace("&", "").Replace(" x ", "").Replace(" X ", "").Replace("*", "");
            }
            return mainKeyword;
        }

        public static string FilterVietkey(this string strSource)
        {
            if (string.IsNullOrEmpty(strSource)) return "";
            if (string.IsNullOrEmpty(strSource)) return "";
            strSource = ConvertISOToUnicode(strSource);
            strSource = strSource.Replace("\0", "");
            if (strSource.Trim().Length == 0)
                return "";
            return ConvertToUnsign3(strSource);
        }

        private static string ConvertISOToUnicode(this string strSource)
        {
            String strUni = "á à ả ã ạ Á À Ả Ã Ạ ă ắ ằ ẳ ẵ ặ Ă Ắ Ằ Ẳ Ẵ Ặ â ấ ầ ẩ ẫ ậ Â Ấ Ầ Ẩ Ẫ Ậ đ Đ é è ẻ ẽ ẹ É È Ẻ Ẽ Ẹ ê ế ề ể ễ ệ Ê Ế Ề Ể Ễ Ệ í ì ỉ ĩ ị Í Ì Ỉ Ĩ Ị ó ò ỏ õ ọ Ó Ò Ỏ Õ Ọ ô ố ồ ổ ỗ ộ Ô Ố Ồ Ổ Ỗ Ộ ơ ớ ờ ở ỡ ợ Ơ Ớ Ờ Ở Ỡ Ợ ú ù ủ ũ ụ Ú Ù Ủ Ũ Ụ ư ứ ừ ử ữ ự Ư Ứ Ừ Ử Ữ Ự ý ỳ ỷ ỹ ỵ Ý Ỳ Ỷ Ỹ Ỵ";
            String strISO = "á à &#7843; ã &#7841; Á À &#7842; Ã &#7840; &#259; &#7855; &#7857; &#7859; &#7861; &#7863; &#258; &#7854; &#7856; &#7858; &#7860; &#7862; â &#7845; &#7847; &#7849; &#7851; &#7853; Â &#7844; &#7846; &#7848; &#7850; &#7852; &#273; &#272; é è &#7867; "
                            + "&#7869; &#7865; É È &#7866; &#7868; &#7864; ê &#7871; &#7873; &#7875; &#7877; &#7879; Ê &#7870; &#7872; &#7874; &#7876; &#7878; í ì &#7881; &#297; &#7883; Í Ì &#7880; &#296; &#7882; ó ò &#7887; õ &#7885; Ó Ò &#7886; Õ &#7884; ô "
                            + "&#7889; &#7891; &#7893; &#7895; &#7897; Ô &#7888; &#7890; &#7892; &#7894; &#7896; &#417; &#7899; &#7901; &#7903; &#7905; &#7907; &#416; &#7898; &#7900; &#7902; &#7904; &#7906; ú ù &#7911; &#361; &#7909; Ú Ù &#7910; &#360; &#7908; &#432; &#7913; &#7915; &#7917; &#7919; &#7921; &#431; "
                            + "&#7912; &#7914; &#7916; &#7918; &#7920; ý &#7923; &#7927; &#7929; &#7925; Ý &#7922; &#7926; &#7928; &#7924;";

            String[] arrCharUni = strUni.Split(" ".ToCharArray());
            String[] arrCharISO = strISO.Split(" ".ToCharArray());

            String strResult = strSource;
            for (int i = 0; i < arrCharUni.Length; i++)
                strResult = strResult.Replace(arrCharISO[i], arrCharUni[i]);

            strUni = "À Á Â Ã Ä Å Æ Ç È É Ê Ë Ì Í Î Ï Ð Ñ Ò Ó Ô Õ Ö Ø Ù Ú Û Ü Ý Þ ß à á â ã ä å æ ç è é ê ë ì í î ï ð ñ ò ó ô õ ö ø ù ú û ü ý þ ÿ";
            strISO = "&#192; &#193; &#194; &#195; &#196; &#197; &#198; &#199; &#200; &#201; &#202; &#203; &#204; &#205; &#206; "
                + "&#207; &#208; &#209; &#210; &#211; &#212; &#213; &#214; &#216; &#217; &#218; &#219; &#220; &#221; &#222; "
                + "&#223; &#224; &#225; &#226; &#227; &#228; &#229; &#230; &#231; &#232; &#233; &#234; &#235; &#236; &#237; &#238; &#239; "
                + "&#240; &#241; &#242; &#243; &#244; &#245; &#246; &#248; &#249; &#250; &#251; &#252; &#253; &#254; &#255;";

            String[] arrCharUni1 = strUni.Split(" ".ToCharArray());
            String[] arrCharISO1 = strISO.Split(" ".ToCharArray());

            for (int i = 0; i < arrCharUni1.Length; i++)
                strResult = strResult.Replace(arrCharISO1[i], arrCharUni1[i]);

            strResult = strResult.Replace("\0", "");
            return strResult;
        }

        private static string ConvertToUnsign3(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty)
                        .Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string ToKeywordSearch(this string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            input = Regex.Replace(input, @"[\W]", " ");
            Regex rgx = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            string rsl = rgx.Replace(WebUtility.HtmlDecode(input), "");
            rgx = new Regex(@"[,;""!`\n\t“”]+");
            return rgx.Replace(rsl, " , ").ToLower().Replace(" ", "_");
        }

        public static string ToElasticDate(this DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static DateTime ToEndDay(this DateTime value)
        {
            return value.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        #endregion

        #region Style

        private static string StyleList = "client-style-list";

        public static MvcHtmlString AddCss(this HtmlHelper helper, string filePath, string storeKey = "")
        {
            try
            {
                if (string.IsNullOrEmpty(storeKey))
                {
                    storeKey = StyleList + "-" + ConfigMgr.StaticVersionFeed;
                }

                var styleList = helper.ViewContext.HttpContext.Items[storeKey] as List<string>;
                if (styleList == null)
                {
                    styleList = new List<string>();
                    helper.ViewContext.HttpContext.Items[storeKey] = styleList;
                }
                styleList.Add(filePath);

                return null;
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }

            return null;
        }

        public static MvcHtmlString RenderCss(this HtmlHelper helper, string storeKey = "")
        {
            if (string.IsNullOrEmpty(storeKey))
            {
                storeKey = StyleList + "-" + ConfigMgr.StaticVersionFeed;
            }

            var styleList = helper.ViewContext.HttpContext.Items[storeKey] as List<string>;
            if (styleList == null || styleList.Count == 0)
                return null;

            var cacheKey = Cacher.CreateCacheKey(styleList, ConfigMgr.GetStaticVersion());
            var result = ConfigMgr.IsLiveEnv ? Cacher.Get<string>(cacheKey) : null;
            if (result != null)
            {
                return MvcHtmlString.Create(result);
            }

            // Xóa các file không tồn tại
            for (var i = 0; i < styleList.Count; i++)
            {
                var path = HttpContext.Current.Server.MapPath(ConfigMgr.IsLiveEnv ? styleList[i].Replace(".css", ".min.css") : styleList[i]);
                if (!File.Exists(path))
                {
                    styleList.RemoveAt(i);
                }
            }

            result = BuildCss(styleList.ToArray());
            Cacher.Add(cacheKey, result, new Cacher.CacheInfo(Cacher.CacheGroup.Js));
            return MvcHtmlString.Create(result);
        }

        public static string BuildCss(params string[] filePaths)
        {
            // Đọc nội dung file và ráp lại
            var result = string.Empty;
            foreach (var file in filePaths)
            {
                var path = HttpContext.Current.Server.MapPath(ConfigMgr.IsLiveEnv
                    ? file.Replace(".css", ".min.css")
                    : file);
                if (!File.Exists(path))
                {
                    if (HttpContext.Current != null && HttpContext.Current.Request.IsLocal)
                        throw new FileNotFoundException(path);
                    else
                        continue;
                }
                var css = File.ReadAllText(path);
                if (string.IsNullOrEmpty(css))
                    if (HttpContext.Current != null && HttpContext.Current.Request.IsLocal)
                        throw new FileNotFoundException(path);
                    else
                        continue;

                css = css.Replace("url({cdn}/Content/", $"url({ConfigMgr.Cdn}/Content/");
                css = css.Replace("url('{cdn}/Content/", $"url('{ConfigMgr.Cdn}/Content/");
                css = css.Replace("url(/{cdn}/Content/", $"url({ConfigMgr.Cdn}/Content/");
                css = css.Replace("url('/{cdn}/Content/", $"url('{ConfigMgr.Cdn}/Content/");
                css = css.Replace("url({cdn}/Scripts/", $"url({ConfigMgr.Cdn}/Scripts/");
                css = css.Replace("url('{cdn}/Scripts/", $"url('{ConfigMgr.Cdn}/Scripts/");
                result += css;
            }

            if (HttpContext.Current != null && HttpContext.Current.Request.IsLocal)
                result += ".profiler-results{display: block !important;}";

            return result;
        }

        /// <summary>
        /// Lấy description của một properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }

        public static class HtmlRemoval
        {
            public static bool StripHtmlAndCheckVisible(string htmlText)
            {
                if (string.IsNullOrEmpty(htmlText))
                    return false;
                else
                {
                    var regJs = new Regex(@"(?s)<\s?script.*?(/\s?>|<\s?/\s?script\s?>)", RegexOptions.IgnoreCase);
                    htmlText = regJs.Replace(htmlText, "");
                    var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
                    htmlText = reg.Replace(htmlText, "");
                    return !string.IsNullOrEmpty(htmlText);
                }
            }

            public static bool IsHtml(string htmlText)
            {
                var tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
                var tagRegexSingleTag = new Regex(@"<[^>]+>");

                return tagRegex.IsMatch(htmlText) || tagRegexSingleTag.IsMatch(htmlText);
            }

            /// <summary>
            /// Remove HTML from string with Regex.
            /// </summary>
            public static string StripTagsRegex(string source)
            {
                return Regex.Replace(source, "<.*?>", string.Empty);
            }

            /// <summary>
            /// Compiled regular expression for performance.
            /// </summary>
            private static readonly Regex HtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

            /// <summary>
            /// Remove HTML from string with compiled Regex.
            /// </summary>
            public static string StripTagsRegexCompiled(string source)
            {
                return HtmlRegex.Replace(source, string.Empty);
            }

            /// <summary>
            /// Remove HTML tags from string using char array.
            /// </summary>
            public static string StripTagsCharArray(string source)
            {
                var array = new char[source.Length];
                int arrayIndex = 0;
                bool inside = false;

                foreach (char @let in source)
                {
                    if (@let == '<')
                    {
                        inside = true;
                        continue;
                    }
                    if (@let == '>')
                    {
                        inside = false;
                        continue;
                    }
                    if (!inside)
                    {
                        array[arrayIndex] = @let;
                        arrayIndex++;
                    }
                }
                return new string(array, 0, arrayIndex);
            }

        }
        #endregion

        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }
}