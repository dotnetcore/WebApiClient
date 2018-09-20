using Demo.HttpClients;
using NetworkSocket.Http;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Demo.HttpServices
{
    /// <summary>
    /// User控制器
    /// </summary>
    [Route("/webapi/{controller}/{action}")]
    public class UserController : HttpController
    {
        [HttpGet]
        public ActionResult About(UserInfo user, string something)
        {
            return Content("This is from NetworkSocket.Http");
        }

        [HttpGet]
        [Route("/webapi/{controller}/{action}/{id}")]
        public JsonResult GetById()
        {
            var id = this.CurrentContext.Action.RouteDatas["id"];
            var model = new UserInfo { Account = id };
            return Json(model);
        }

        [HttpGet]
        public JsonResult GetByAccount(string account)
        {
            var model = new UserInfo { Account = account };
            return Json(model);
        }

        [HttpPost]
        public JsonResult UpdateWithForm(UserInfo user, string name, string nickName, int? age)
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
        public ActionResult UpdateWithMulitpart(UserInfo user, string nickName, int? age)
        {
            return Json(user);
        }
    }
}
