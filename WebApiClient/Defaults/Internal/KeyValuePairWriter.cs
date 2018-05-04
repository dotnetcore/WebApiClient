using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示KeyValuePair写入对象
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    class KeyValuePairWriter : JsonWriter, IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// 当前属性名称
        /// </summary>
        private string properyName;

        /// <summary>
        /// 保存KeyValuePair的列表
        /// </summary>
        private readonly List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// KeyValuePair写入对象
        /// </summary>
        /// <param name="name">对象名称</param>
        public KeyValuePairWriter(string name)
        {
            this.properyName = name;
        }

        /// <summary>
        /// 转换为KeyValuePair并添加到列表
        /// </summary>
        /// <param name="value">值</param>
        private void AddStringItem(string value)
        {
            var item = new KeyValuePair<string, string>(this.properyName, value);
            this.keyValues.Add(item);
        }


        /// <summary>
        /// 转换为KeyValuePair并添加到列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        private void AddNullableItem<T>(T value)
        {
            if (value == null)
            {
                this.AddStringItem(null);
            }
            else
            {
                this.AddStringItem(value.ToString());
            }
        }

        /// <summary>
        /// 写入属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="escape"></param>
        public override void WritePropertyName(string name, bool escape)
        {
            this.properyName = name;
        }

        /// <summary>
        /// 写入属性
        /// </summary>
        /// <param name="name"></param>
        public override void WritePropertyName(string name)
        {
            this.properyName = name;
        }

        /// <summary>
        /// 写入bool
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(bool value)
        {
            var val = value ? "true" : "false";
            this.AddStringItem(val);
        }

        /// <summary>
        /// 写入bool?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(bool? value)
        {
            if (value == null)
            {
                this.AddStringItem(null);
            }
            else
            {
                this.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// 写入byte
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(byte value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入byte?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(byte? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入byte[]
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(byte[] value)
        {
            if (value == null)
            {
                this.AddStringItem(null);
            }
            else
            {
                throw new NotSupportedException($"不支持序列化{this.properyName} {value}");
            }
        }

        /// <summary>
        /// 写入char
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(char value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入char?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(char? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入DateTime
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(DateTime value)
        {
            var dateTimeString = value.ToString(this.DateFormatString, CultureInfo.InvariantCulture);
            this.AddStringItem(dateTimeString);
        }

        /// <summary>
        /// 写入DateTime?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(DateTime? value)
        {
            if (value == null)
            {
                this.AddStringItem(null);
            }
            else
            {
                this.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// 写入DateTimeOffset
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(DateTimeOffset value)
        {
            var dateTimeOffsetString = value.ToString(this.DateFormatString, CultureInfo.InvariantCulture);
            this.AddStringItem(dateTimeOffsetString);
        }

        /// <summary>
        /// 写入DateTimeOffset?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(DateTimeOffset? value)
        {
            if (value == null)
            {
                this.AddStringItem(null);
            }
            else
            {
                this.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// 写入decimal
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(decimal value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入decimal?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(decimal? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入double
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(double value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入double?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(double? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入float
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(float value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入float?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(float? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入Guid
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(Guid value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入Guid?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(Guid? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入int
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(int value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入int?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(int? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入long
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(long value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入long?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(long? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入object
        /// 使用基类自动分析类型
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(object value)
        {
            if (value == null)
            {
                this.AddStringItem(null);
            }
            else
            {
                base.WriteValue(value);
            }
        }

        /// <summary>
        /// 写入sbyte
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(sbyte value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入sbyte?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(sbyte? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入short
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(short value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入short?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(short? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入string
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(string value)
        {
            this.AddStringItem(value);
        }

        /// <summary>
        /// 写入TimeSpan
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(TimeSpan value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入TimeSpan?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(TimeSpan? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入uint
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(uint value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入uint?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(uint? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入ulong
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(ulong value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入ulong
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(ulong? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入uri
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(Uri value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入ushort
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(ushort value)
        {
            this.AddStringItem(value.ToString());
        }

        /// <summary>
        /// 写入ushort?
        /// </summary>
        /// <param name="value"></param>
        public override void WriteValue(ushort? value)
        {
            this.AddNullableItem(value);
        }

        /// <summary>
        /// 写入null值
        /// </summary>
        public override void WriteNull()
        {
            this.AddStringItem(null);
        }

        /// <summary>
        /// 转换为文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return null;
        }

        #region empty methods
        public override void Close()
        {
        }
        public override void Flush()
        {
        }
        public override void WriteComment(string text)
        {
        }
        public override void WriteEnd()
        {
        }
        protected override void WriteEnd(JsonToken token)
        {
        }
        public override void WriteEndArray()
        {
        }
        public override void WriteEndConstructor()
        {
        }
        public override void WriteEndObject()
        {
        }
        protected override void WriteIndent()
        {
        }
        protected override void WriteIndentSpace()
        {
        }
        public override void WriteRaw(string json)
        {
        }
        public override void WriteRawValue(string json)
        {
        }
        public override void WriteStartArray()
        {
        }
        public override void WriteStartConstructor(string name)
        {
        }
        public override void WriteStartObject()
        {
        }
        public override void WriteUndefined()
        {
        }
        protected override void WriteValueDelimiter()
        {
        }
        public override void WriteWhitespace(string ws)
        {
        }
        #endregion

        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.keyValues.GetEnumerator();
        }

        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// 查看的对象
            /// </summary>
            private KeyValuePairWriter target;

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(KeyValuePairWriter target)
            {
                this.target = target;
            }

            /// <summary>
            /// 查看的内容
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public List<KeyValuePair<string, string>> Values
            {
                get
                {
                    return this.target.keyValues;
                }
            }
        }
    }
}