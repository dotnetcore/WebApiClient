namespace WebApiClient.Analyzers
{
    /// <summary>
    /// HttpApi诊断器接口
    /// </summary>
    interface IHttpApiDiagnostic
    {
        /// <summary>
        /// 报告诊断结果
        /// </summary>
        void Report();
    }
}
