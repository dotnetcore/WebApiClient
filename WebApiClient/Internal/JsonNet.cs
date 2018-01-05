using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供Json.net无引用调用
    /// </summary>
    static class JsonNet
    {
        /// <summary>
        /// 程序集名称
        /// </summary>
        private static readonly string assemblyName = "Newtonsoft.Json";

        /// <summary>
        /// 程序集文件
        /// </summary>
        private static readonly string assemblyFile = assemblyName + ".dll";

        /// <summary>
        /// 序列化方法
        /// </summary>
        private static readonly Method serializeMethod = null;

        /// <summary>
        /// 反序列化方法
        /// </summary>
        private static readonly Method deserializeMethod = null;

        /// <summary>
        /// 获取是否得到支持
        /// </summary>
        public static readonly bool IsSupported = false;

        /// <summary>
        /// Json.net
        /// </summary>
        static JsonNet()
        {
            var type = FindJsonConvertType();
            if (type != null)
            {
                serializeMethod = FindSerializeObjectMethod(type);
                deserializeMethod = FindDeserializeObjectMethod(type);
            }
            IsSupported = serializeMethod != null && deserializeMethod != null;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            return serializeMethod.Invoke(null, obj) as string;
        }

        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <param name="json">json文本</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object DeserializeObject(string json, Type type)
        {
            return deserializeMethod.Invoke(null, json, type);
        }

        /// <summary>
        /// 查找类型JsonConvert
        /// </summary>
        /// <returns></returns>
        private static Type FindJsonConvertType()
        {
            var assembly = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(item => item.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

            if (assembly == null && File.Exists(assemblyFile))
            {
                assembly = Assembly.LoadFrom(assemblyFile);
            }

            if (assembly == null)
            {
                return null;
            }
            return assembly.GetType("Newtonsoft.Json.JsonConvert", false);
        }

        /// <summary>
        /// 查找SerializeObject方法
        /// </summary>
        /// <param name="type">JsonConvert</param>
        /// <returns></returns>
        private static Method FindSerializeObjectMethod(Type type)
        {
            var method = type.GetMethod("SerializeObject", new[] { typeof(object) });
            if (method == null)
            {
                return null;
            }
            return new Method(method);
        }

        /// <summary>
        /// 查找DeserializeObject方法
        /// </summary>
        /// <param name="type">JsonConvert</param>
        /// <returns></returns>
        private static Method FindDeserializeObjectMethod(Type type)
        {
            var method = type.GetMethod("DeserializeObject", new[] { typeof(string), typeof(Type) });
            if (method == null)
            {
                return null;
            }
            return new Method(method);
        }
    }
}
