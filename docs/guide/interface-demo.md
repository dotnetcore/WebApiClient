# 接口声明示例

这个OpenApi文档在[petstore.swagger.io](https://petstore.swagger.io/)，代码为使用WebApiClientCore.OpenApi.SourceGenerator工具将其OpenApi文档反向生成得到

```csharp
/// <summary>
/// Everything about your Pets
/// </summary>
[LoggingFilter]
[HttpHost("https://petstore.swagger.io/v2/")]
public interface IPetApi : IHttpApi
{
    /// <summary>
    /// Add a new pet to the store
    /// </summary>
    /// <param name="body">Pet object that needs to be added to the store</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpPost("pet")]
    Task AddPetAsync([Required] [JsonContent] Pet body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing pet
    /// </summary>
    /// <param name="body">Pet object that needs to be added to the store</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpPut("pet")]
    Task UpdatePetAsync([Required] [JsonContent] Pet body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds Pets by status
    /// </summary>
    /// <param name="status">Status values that need to be considered for filter</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [HttpGet("pet/findByStatus")]
    ITask<List<Pet>> FindPetsByStatusAsync([Required] IEnumerable<Anonymous> status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds Pets by tags
    /// </summary>
    /// <param name="tags">Tags to filter by</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [Obsolete]
    [HttpGet("pet/findByTags")]
    ITask<List<Pet>> FindPetsByTagsAsync([Required] IEnumerable<string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find pet by ID
    /// </summary>
    /// <param name="petId">ID of pet to return</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [HttpGet("pet/{petId}")]
    ITask<Pet> GetPetByIdAsync([Required] long petId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a pet in the store with form data
    /// </summary>
    /// <param name="petId">ID of pet that needs to be updated</param>
    /// <param name="name">Updated name of the pet</param>
    /// <param name="status">Updated status of the pet</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpPost("pet/{petId}")]
    Task UpdatePetWithFormAsync([Required] long petId, [FormField] string name, [FormField] string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a pet
    /// </summary>
    /// <param name="api_key"></param>
    /// <param name="petId">Pet id to delete</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpDelete("pet/{petId}")]
    Task DeletePetAsync([Header("api_key")] string api_key, [Required] long petId, CancellationToken cancellationToken = default);

    /// <summary>
    /// uploads an image
    /// </summary>
    /// <param name="petId">ID of pet to update</param>
    /// <param name="additionalMetadata">Additional data to pass to server</param>
    /// <param name="file">file to upload</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [HttpPost("pet/{petId}/uploadImage")]
    ITask<ApiResponse> UploadFileAsync([Required] long petId, [FormDataText] string additionalMetadata, FormDataFile file, CancellationToken cancellationToken = default);
}
```
