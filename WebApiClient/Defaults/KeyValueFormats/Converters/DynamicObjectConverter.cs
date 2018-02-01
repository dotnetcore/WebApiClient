using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示动态类型转换器
    /// </summary>
    public class DynamicObjectConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            if (context.Data is DynamicObject dynamicObject)
            {
                return from name in dynamicObject.GetDynamicMemberNames()
                       let value = this.GetValue(dynamicObject, name)
                       let ctx = new ConvertContext(name, value, context.Depths, context.Options)
                       select ctx.ToKeyValuePair();
            }

            return this.Next.Invoke(context);
        }

        /// <summary>
        /// 获取动态类型的值
        /// </summary>
        /// <param name="dynamicObject">实例</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        private object GetValue(DynamicObject dynamicObject, string name)
        {
            var binder = new MemberBinder(name);
            dynamicObject.TryGetMember(binder, out object value);
            return value;
        }

        /// <summary>
        /// 表示成员值的获取绑定
        /// </summary>
        private class MemberBinder : GetMemberBinder
        {
            /// <summary>
            /// 键的信息获取绑定
            /// </summary>
            /// <param name="key">键名</param>
            public MemberBinder(string key)
                : base(key, false)
            {
            }

            /// <summary>
            /// 在派生类中重写时，如果无法绑定目标动态对象，则执行动态获取成员操作的绑定
            /// </summary>
            /// <param name="target"></param>
            /// <param name="errorSuggestion"></param>
            /// <returns></returns>
            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }
    }
}
