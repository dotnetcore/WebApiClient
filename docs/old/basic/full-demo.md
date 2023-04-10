# 8、完整接口声明示例

本示例的接口为swagger官方的v2/swagger.json，参见swagger官网接口，对于swagger文档，可以使用WebApiClient.Tools.Swagger工具生成客户端代码。

## 8.1 IPetApi

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.DataAnnotations;
using WebApiClient.Parameterables;
namespace petstore.swagger
{
    /// <summary>
    /// Everything about your Pets
    /// </summary>
    [TraceFilter]
    [HttpHost("https://petstore.swagger.io/v2/")]
    public interface IPetApi : IHttpApi
    {
        /// <summary>
        /// Add a new pet to the store
        /// </summary>
        /// <param name="body">Pet object that needs to be added to the store</param>
        [HttpPost("pet")]
        ITask<HttpResponseMessage> AddPetAsync([Required] [JsonContent] Pet body);

        /// <summary>
        /// Update an existing pet
        /// </summary>
        /// <param name="body">Pet object that needs to be added to the store</param>
        [HttpPut("pet")]
        ITask<HttpResponseMessage> UpdatePetAsync([Required] [JsonContent] Pet body);

        /// <summary>
        /// Finds Pets by status
        /// </summary>
        /// <param name="status">Status values that need to be considered for filter</param>
        /// <returns>successful operation</returns>
        [HttpGet("pet/findByStatus")]
        ITask<List<Pet>> FindPetsByStatusAsync([Required] IEnumerable<Anonymous> status);

        /// <summary>
        /// Finds Pets by tags
        /// </summary>
        /// <param name="tags">Tags to filter by</param>
        /// <returns>successful operation</returns>
        [Obsolete]
        [HttpGet("pet/findByTags")]
        ITask<List<Pet>> FindPetsByTagsAsync([Required] IEnumerable<string> tags);

        /// <summary>
        /// Find pet by ID
        /// </summary>
        /// <param name="petId">ID of pet to return</param>
        /// <returns>successful operation</returns>
        [HttpGet("pet/{petId}")]
        ITask<Pet> GetPetByIdAsync([Required] long petId);

        /// <summary>
        /// Updates a pet in the store with form data
        /// </summary>
        /// <param name="petId">ID of pet that needs to be updated</param>
        /// <param name="name">Updated name of the pet</param>
        /// <param name="status">Updated status of the pet</param>
        [HttpPost("pet/{petId}")]
        ITask<HttpResponseMessage> UpdatePetWithFormAsync([Required] long petId, [FormContent] string name, [FormContent] string status);

        /// <summary>
        /// Deletes a pet
        /// </summary>
        /// <param name="api_key"></param>
        /// <param name="petId">Pet id to delete</param>
        [HttpDelete("pet/{petId}")]
        ITask<HttpResponseMessage> DeletePetAsync([Header("api_key")] string api_key, [Required] long petId);

        /// <summary>
        /// uploads an image
        /// </summary>
        /// <param name="petId">ID of pet to update</param>
        /// <param name="additionalMetadata">Additional data to pass to server</param>
        /// <param name="file">file to upload</param>
        /// <returns>successful operation</returns>
        [TraceFilter(Enable = false)]
        [HttpPost("pet/{petId}/uploadImage")]
        ITask<ApiResponse> UploadFileAsync([Required] long petId, [MulitpartContent] string additionalMetadata, MulitpartFile file);

    }
}
```

## 8.2 IUserApi

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.DataAnnotations;
using WebApiClient.Parameterables;
namespace petstore.swagger
{
    /// <summary>
    /// Operations about user
    /// </summary>
    [TraceFilter]
    [HttpHost("https://petstore.swagger.io/v2/")]
    public interface IUserApi : IHttpApi
    {
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="body">Created user object</param>
        /// <returns>successful operation</returns>
        [HttpPost("user")]
        ITask<HttpResponseMessage> CreateUserAsync([Required] [JsonContent] User body);

        /// <summary>
        /// Creates list of users with given input array
        /// </summary>
        /// <param name="body">List of user object</param>
        /// <returns>successful operation</returns>
        [HttpPost("user/createWithArray")]
        ITask<HttpResponseMessage> CreateUsersWithArrayInputAsync([Required] [JsonContent] IEnumerable<User> body);

        /// <summary>
        /// Creates list of users with given input array
        /// </summary>
        /// <param name="body">List of user object</param>
        /// <returns>successful operation</returns>
        [HttpPost("user/createWithList")]
        ITask<HttpResponseMessage> CreateUsersWithListInputAsync([Required] [JsonContent] IEnumerable<User> body);

        /// <summary>
        /// Logs user into the system
        /// </summary>
        /// <param name="username">The user name for login</param>
        /// <param name="password">The password for login in clear text</param>
        /// <returns>successful operation</returns>
        [HttpGet("user/login")]
        ITask<string> LoginUserAsync([Required] string username, [Required] string password);

        /// <summary>
        /// Logs out current logged in user session
        /// </summary>
        /// <returns>successful operation</returns>
        [HttpGet("user/logout")]
        ITask<HttpResponseMessage> LogoutUserAsync();

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="username">The name that needs to be fetched. Use user1 for testing.</param>
        /// <returns>successful operation</returns>
        [HttpGet("user/{username}")]
        ITask<User> GetUserByNameAsync([Required] string username);

        /// <summary>
        /// Updated user
        /// </summary>
        /// <param name="username">name that need to be updated</param>
        /// <param name="body">Updated user object</param>
        [HttpPut("user/{username}")]
        ITask<HttpResponseMessage> UpdateUserAsync([Required] string username, [Required] [JsonContent] User body);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="username">The name that needs to be deleted</param>
        [HttpDelete("user/{username}")]
        ITask<HttpResponseMessage> DeleteUserAsync([Required] string username);

    }
}
```
