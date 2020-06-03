using System.Collections.Generic;
using Xunit;

namespace WebApiClientCore.Test.ResponseCaches
{
    public class LambdaTest
    {
        [Fact]
        public void GetTest()
        {
            var model = new Model { name = "laojiu" };
            var p = model.GetType().GetProperty("name");

            var getter = Lambda.CreateGetFunc<Model, string>(p);
            var name = getter.Invoke(model);
            Assert.True(name == model.name);


            var getter2 = Lambda.CreateGetFunc<object, string>(p);
            var name2 = getter2.Invoke(model);
            Assert.True(name2 == model.name);

            var getter3 = Lambda.CreateGetFunc<object, object>(p.DeclaringType, p.Name);
            var name3 = getter2.Invoke(model).ToString();
            Assert.True(name3 == model.name);

            var kv = new KeyValuePair<string, int>("k", 10);
            var getter4 = Lambda.CreateGetFunc<object, object>(kv.GetType(), "Value");
            var value = (int)getter4.Invoke(kv);
            Assert.True(value == kv.Value);

            var getter5 = Lambda.CreateGetFunc<object, int>(kv.GetType(), "Value");
            Assert.True(getter5.Invoke(kv) == kv.Value);

            var getter6 = Lambda.CreateGetFunc<object, long>(kv.GetType(), "Value");
            Assert.True(getter6.Invoke(kv) == kv.Value);
        }

        [Fact]
        public void SetTest()
        {
            var model = new Model { name = "laojiu" };
            var setter = Lambda.CreateSetAction<object, object>(model.GetType().GetProperty("name"));
            setter.Invoke(model, "ee");
            Assert.True("ee" == model.name);
        }

        [Fact]
        public void CtorTest()
        {
            var value = "name";
            var name = Lambda.CreateCtorFunc<string, Model>(typeof(Model))(value).name;
            Assert.True(name == value);

            var func = Lambda.CreateCtorFunc<string, object>(typeof(Model));
            var model = func.Invoke(value) as Model;
            Assert.True(model.name == value);
        }


        class Model
        {
            public string name { get; set; }

            public Model()
            {
            }

            public Model(string name)
            {
                this.name = name;
            }
        }
    }
}
