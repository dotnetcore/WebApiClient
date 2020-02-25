namespace WebApiClient
{
    /// <summary>
    /// Defines the behavior of the ApiAction filter decoration feature
    /// </summary>
    public interface IApiActionFilterAttribute : IApiActionFilter, IAttributeMultiplable
    {
    }
}
