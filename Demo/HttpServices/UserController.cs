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
            Debug.Assert(this.Request.Query.ContainsKey("BirthDay") == true);
            Debug.Assert(this.Request.Query.ContainsKey("Gender") == false);
            Debug.Assert(this.Request.Query.ContainsKey("Email") == false);

            var about = new StringBuilder()
                .AppendLine()
                .Append("UserInfo:").AppendLine(user.ToString())
                .Append("Something:").Append(something);

            var keys = Request.Headers.Keys;
            foreach (var key in keys.Cast<string>().Reverse())
            {
                var value = Request.Headers.TryGet<string>(key, null);
                about.Insert(0, key + ": " + value + "\r\n");
            }

            return Content(about.ToString());
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
            Debug.Assert(this.Request.Form.ContainsKey("BirthDay") == true);
            Debug.Assert(this.Request.Form.ContainsKey("Gender") == false);
            Debug.Assert(this.Request.Form.ContainsKey("Email") == false);
            Debug.Assert(this.Request.Form.ContainsKey("age") == false);

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
            Debug.Assert(this.Request.Form.ContainsKey("BirthDay") == true);
            Debug.Assert(this.Request.Form.ContainsKey("Gender") == false);
            Debug.Assert(this.Request.Form.ContainsKey("Email") == false);
            Debug.Assert(this.Request.Form.ContainsKey("age") == true);

            return Json(user);
        }


        protected override void OnExecuting(ActionContext filterContext)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} HttpServer->收到http请求：{1}", DateTime.Now.ToString("HH:mm:ss.fff"), filterContext.Action.Route);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
