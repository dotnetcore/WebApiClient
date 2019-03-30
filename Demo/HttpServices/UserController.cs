using Demo.HttpClients;
using NetworkSocket.Http;
using System.IO;
using System.Xml.Linq;
using WebApiClient;
using WebApiClient.Defaults;

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
            System.Threading.Thread.Sleep(100);
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
            var stream = new MemoryStream(Request.Body);
            var xml = XDocument.Load(stream).ToString();
            var user = XmlFormatter.Default.Deserialize(xml, typeof(UserInfo)) as UserInfo;
            user.Email = "laojiu@qq.com";
            return new XmlResult(user);
        }

        [HttpPost]
        public ActionResult UpdateWithMulitpart(UserInfo user, string nickName, int? age)
        {
            return Json(user);
        }
    }
}
