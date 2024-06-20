using Microsoft.Extensions.Hosting;
using Serilog;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Serilog扩展
    /// </summary>
    public static class SerilogExtensions
    {
        private const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{NewLine}{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

        /// <summary>
        /// 使用Serilog
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="writeToProviders">是否同时写入到非Serilog的日志提供者</param>
        public static IHostBuilder UseSerilog(this IHostBuilder builder, bool writeToProviders)
        {
            return builder.UseSerilog((context, logger) =>
            {
                logger.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: template);
            }, writeToProviders);
        }
    }
}
