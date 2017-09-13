using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes; 

namespace Demo
{
    [Logger] // 记录请求日志
    [HttpHost("http://www.mywebapi.com")] // 可以在Implement传Url覆盖
    public interface MyWebApi
    {
        // GET webapi/user/id001
        // Return HttpResponseMessage
        [HttpGet("/webapi/user/{id}")]
        Task<HttpResponseMessage> GetUserByIdAsync(string id);

        // GET webapi/user?account=laojiu
        // Return 原始string内容
        [HttpGet("/webapi/user")]
        Task<string> GetUserByAccountAsync(string account);


        // POST webapi/user  
        // Body:Account=laojiu&Password=123456
        // Return json或xml内容
        [HttpPost("/webapi/user")]
        Task<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);

        // POST webapi/user   
        // Body:{"Account":"laojiu","Password":"123456"}
        // Return json或xml内容
        [HttpPost("/webapi/user")]
        Task<UserInfo> UpdateUserWithJsonAsync([JsonContent] UserInfo user);

        // POST webapi/user   
        // Body:﻿<?xml version="1.0" encoding="utf-8"?><UserInfo><Account>laojiu</Account><Password>123456</Password></UserInfo>
        // Return xml内容
        [XmlReturn]
        [HttpPost("/webapi/user")]
        Task<UserInfo> UpdateUserWithXmlAsync([XmlContent] UserInfo user);
    }
}
