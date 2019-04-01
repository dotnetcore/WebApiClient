﻿using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义Api响应结果缓存提供者的接口
    /// </summary>
    public interface IResponseCacheProvider
    {
        /// <summary>
        /// 获取提供者的友好名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 从缓存中获取响应实体
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<ResponseCacheResult> GetAsync(ApiActionContext context,string key);

        /// <summary>
        /// 设置响应实体到缓存
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="key">键</param>
        /// <param name="entry">缓存实体</param>
        /// <param name="expiration">有效时间</param>
        /// <returns></returns>
        Task SetAsync(ApiActionContext context, string key, ResponseCacheEntry entry, TimeSpan expiration);
    }
}
