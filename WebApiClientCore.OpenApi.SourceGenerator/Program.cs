using CommandLine;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<OpenApiDocOptions>(args)
                .WithParsed(options =>
                {
                    var doc = new OpenApiDoc(options);
                    doc.GenerateFiles();
                })
                .WithNotParsed(errors =>
                {
                    errors.Output();
                });
        }
    }
}
