#if NET45

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebApiClient
{
    /// <summary>
    /// 提供Json解析
    /// </summary>
    static class JSON
    {
        /// <summary>
        /// 序列化得到Json
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="datetimeFomat">时期时间格式</param>
        /// <returns></returns>
        public static string Serialize(object model, string datetimeFomat)
        {
            if (model == null)
            {
                return null;
            }

            var serializer = new JavaScriptSerializer();
            var dateTimeConverter = new DateTimeConverter(datetimeFomat);
            serializer.MaxJsonLength = int.MaxValue;
            serializer.RegisterConverters(new JavaScriptConverter[] { dateTimeConverter });

            var json = serializer.Serialize(model);
            return dateTimeConverter.Decode(json);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public static object Deserialize(string json, Type objType)
        {
            if (json == null)
            {
                return null;
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            return serializer.Deserialize(json, objType);
        }

        /// <summary>
        /// 时间转换器
        /// </summary>
        private class DateTimeConverter : JavaScriptConverter
        {
            /// <summary>
            /// 获取转换器是否已使用过
            /// </summary>              
            private bool isUsed = false;

            /// <summary>
            /// 时期时间格式化
            /// </summary>
            private readonly string datetimeFomat;

            /// <summary>
            /// 时间转换
            /// </summary>
            /// <param name="datetimeFomat">时期时间格式</param>
            public DateTimeConverter(string datetimeFomat)
            {
                this.datetimeFomat = datetimeFomat;
            }

            /// <summary>
            /// 时间解码
            /// </summary>
            /// <param name="json">json内容</param>
            /// <returns></returns>
            public string Decode(string json)
            {
                return this.isUsed ? UriEscapeValue.Decode(json) : json;
            }

            /// <summary>
            /// 反序化
            /// </summary>
            /// <param name="dictionary"></param>
            /// <param name="type"></param>
            /// <param name="serializer"></param>
            /// <returns></returns>
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 序列化对象
            /// </summary>
            /// <param name="obj">对象</param>
            /// <param name="serializer">序列化实例</param>
            /// <returns></returns>
            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                var dateTime = (DateTime?)obj;
                if (dateTime.HasValue == false)
                {
                    return null;
                }

                this.isUsed = true;
                var dateTimeString = dateTime.Value.ToString(this.datetimeFomat, DateTimeFormatInfo.InvariantInfo);
                return new UriEscapeValue(dateTimeString);
            }

            /// <summary>
            /// 支持的类型
            /// </summary>
            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    yield return typeof(DateTime);
                    yield return typeof(Nullable<DateTime>);
                }
            }

            /// <summary>
            /// 表示将值进行Uri转义              
            /// </summary>
            private class UriEscapeValue : Uri, IDictionary<string, object>
            {
                /// <summary>
                /// 标记
                /// </summary>
                private static readonly string Mask = "UriEscaped_";

                /// <summary>
                /// 表达式
                /// </summary>
                private static readonly string Pattern = Mask + ".+?(?=\")";

                /// <summary>
                /// 将值进行Uri转义  
                /// </summary>
                /// <param name="value">值</param>
                public UriEscapeValue(string value)
                    : base(UriEscapeValue.Mask + value, UriKind.Relative)
                {
                }

                /// <summary>
                /// URI解码
                /// </summary>
                /// <param name="content">内容</param>
                /// <returns></returns>
                public static string Decode(string content)
                {
                    return Regex.Replace(content, Pattern, m =>
                    {
                        var vlaue = m.Value.Substring(UriEscapeValue.Mask.Length);
                        return HttpUtility.UrlDecode(vlaue, Encoding.UTF8);
                    });
                }

                #region IDictionary<string, object>
                void IDictionary<string, object>.Add(string key, object value)
                {
                    throw new NotImplementedException();
                }

                bool IDictionary<string, object>.ContainsKey(string key)
                {
                    throw new NotImplementedException();
                }

                ICollection<string> IDictionary<string, object>.Keys
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                bool IDictionary<string, object>.Remove(string key)
                {
                    throw new NotImplementedException();
                }

                bool IDictionary<string, object>.TryGetValue(string key, out object value)
                {
                    throw new NotImplementedException();
                }

                ICollection<object> IDictionary<string, object>.Values
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                object IDictionary<string, object>.this[string key]
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                void ICollection<KeyValuePair<string, object>>.Clear()
                {
                    throw new NotImplementedException();
                }

                bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
                {
                    throw new NotImplementedException();
                }

                int ICollection<KeyValuePair<string, object>>.Count
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                bool ICollection<KeyValuePair<string, object>>.IsReadOnly
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
                {
                    throw new NotImplementedException();
                }

                IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
                {
                    throw new NotImplementedException();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    throw new NotImplementedException();
                }
                #endregion
            }
        }
    }
}

#endif