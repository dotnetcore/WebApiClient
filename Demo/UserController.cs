using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSocket;
using NetworkSocket.Http;

namespace Demo
{
    /// <summary>
    /// User控制器
    /// </summary>
    [Route("/webapi/user/{action}")]
    public class UserController : HttpController
    {
        [HttpGet]
        public ActionResult About(UserInfo user, string something)
        {
            var about = new StringBuilder()
                .Append("Cookie:").AppendLine(Request.Headers.TryGet<string>("Cookie"))
                .Append("Authorization:").AppendLine(Request.Headers.TryGet<string>("Authorization"))
                .Append("UserInfo:").AppendLine(user.ToString())
                .Append("Something:").Append(something)
                .ToString();

            return Content(about);
        }

        [HttpGet]
        public JsonResult GetById(string id)
        {
            var model = new UserInfo { Account = CurrentContext.Action.ApiName, Password = "123456" };
            return Json(model);
        }

        [HttpGet]
        public JsonResult GetByAccount(string account)
        {
            var model = new UserInfo { Account = CurrentContext.Action.ApiName, Password = "123456" };
            return Json(model);
        }

        [HttpPost]
        public JsonResult UpdateWithForm(UserInfo user)
        {
            user.Account = CurrentContext.Action.ApiName;
            return Json(user);
        }

        [HttpPost]
        public JsonResult UpdateWithJson([Body] UserInfo user)
        {
            user.Account = CurrentContext.Action.ApiName;
            return Json(user);
        }

        [HttpPost]
        public ActionResult UpdateWithXml()
        {
            var xml = Encoding.UTF8.GetString(Request.Body);
            return Content(xml);
        }

        [HttpPost]
        public ActionResult UpdateWithMulitpart(UserInfo user)
        {
            user.Account = CurrentContext.Action.ApiName;
            user.Password = string.Join(";", Request.Files.Select(item => item.FileName));
            return Json(user);
        }

        protected override void OnExecuting(ActionContext filterContext)
        {
            Console.WriteLine("{0} HttpServer收到http请求：{1}", DateTime.Now.ToString("HH:mm:ss.fff"), filterContext.Action.Route);
        }
    }
}
