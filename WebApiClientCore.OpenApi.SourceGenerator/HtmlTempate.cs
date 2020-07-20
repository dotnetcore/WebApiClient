using RazorEngineCore;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// html模板
    /// </summary>
    public class HtmlTempate : RazorEngineTemplateBase
    {
        /// <summary>
        /// html标签转换
        /// </summary>
        /// <param name="obj"></param>
        public override void Write(object obj = null)
        {
            var text = obj?.ToString();
            if (text != null)
            {
                text = text.Replace("<", "&lt;").Replace(">", "&gt;");
            }
            base.Write(text);
        }
    }

    /// <summary>
    /// html模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HtmlTempate<T> : HtmlTempate
    {
        public new T Model { get; set; }
    }
}
