using System.Collections.Generic;
using WebApiClientCore.Internals;
using Xunit;

namespace WebApiClientCore.Test.Internals
{
    public class LambdaTest
    {
        [Fact]
        public void GetTest()
        {
            var model = new Model { name = "laojiu" };
            var p = model.GetType().GetProperty("name");
            Assert.NotNull(p);
            var getter = LambdaUtil.CreateGetFunc<Model, string>(p);
            var name = getter.Invoke(model);
            Assert.True(name == model.name);


            var getter2 = LambdaUtil.CreateGetFunc<object, string>(p);
            var name2 = getter2.Invoke(model);
            Assert.True(name2 == model.name);

            Assert.NotNull(p.DeclaringType);
            var getter3 = LambdaUtil.CreateGetFunc<object, object>(p.DeclaringType, p.Name);
            var name3 = getter2.Invoke(model).ToString();
            Assert.True(name3 == model.name);

            var kv = new KeyValuePair<string, int>("k", 10);
            var getter4 = LambdaUtil.CreateGetFunc<object, object>(kv.GetType(), "Value");
            var value = (int)getter4.Invoke(kv);
            Assert.True(value == kv.Value);

            var getter5 = LambdaUtil.CreateGetFunc<object, int>(kv.GetType(), "Value");
            Assert.True(getter5.Invoke(kv) == kv.Value);

            var getter6 = LambdaUtil.CreateGetFunc<object, long>(kv.GetType(), "Value");
            Assert.True(getter6.Invoke(kv) == kv.Value);
        }

        [Fact]
        public void SetTest()
        {
            var model = new Model { name = "laojiu" };
            var setter = LambdaUtil.CreateSetAction<object, object>(model.GetType().GetProperty("name")!);
            setter.Invoke(model, "ee");
            Assert.Equal("ee", model.name);
        }

        [Fact]
        public void CtorTest()
        {
            var value = "name";
            var name = LambdaUtil.CreateCtorFunc<string, Model>(typeof(Model))(value).name;
            Assert.True(name == value);

            var func = LambdaUtil.CreateCtorFunc<string, object>(typeof(Model));
            var model = (Model)func.Invoke(value);
            Assert.True(model.name == value);
        }


        class Model
        {
            public string? name { get; set; }

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
