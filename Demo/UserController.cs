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
        public JsonResult GetById(string id)
        {
            var model = new UserInfo { Account = "GetUserById", Password = "123456" };
            return Json(model);
        }

        [HttpGet]
        public JsonResult GetByAccount(string account)
        {
            var model = new UserInfo { Account = account, Password = "123456" };
            return Json(model);
        }

        [HttpPost]
        public JsonResult UpdateWithForm(UserInfo user)
        {
            return Json(user);
        }

        [HttpPost]
        public JsonResult UpdateWithJson([Body] UserInfo user)
        {
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
            var file = Request.Files;
            return Json(user);
        }

        protected override void OnExecuting(ActionContext filterContext)
        {
            Console.WriteLine("{0} HttpServer收到http请求：{1}", DateTime.Now.ToString("HH:mm:ss.fff"), filterContext.Action.Route);
        }
    }
}
