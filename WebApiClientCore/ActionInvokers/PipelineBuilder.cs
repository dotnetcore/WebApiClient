using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示所有中间件执行委托
    /// </summary>
    /// <typeparam name="TContext">中间件上下文类型</typeparam>
    /// <param name="context">中间件上下文</param>
    /// <returns></returns>
    delegate Task InvokeDelegate<TContext>(TContext context);

    /// <summary>
    /// 表示中间件创建者
    /// </summary>
    class PipelineBuilder<TContext>
    {
        private readonly InvokeDelegate<TContext> completedHandler;
        private readonly List<Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>>> middlewares = new List<Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>>>();

        /// <summary>
        /// 中间件创建者
        /// </summary>
        public PipelineBuilder()
            : this(context => Task.CompletedTask)
        {
        }

        /// <summary>
        /// 中间件创建者
        /// </summary>
        /// <param name="completedHandler">完成执行内容处理者</param>
        public PipelineBuilder(InvokeDelegate<TContext> completedHandler)
        {
            this.completedHandler = completedHandler;
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="middleware"></param>
        public void Use(Func<TContext, Func<Task>, Task> middleware)
        {
            this.Use(next => context => middleware(context, () => next(context)));
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="middleware"></param>
        public void Use(Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>> middleware)
        {
            this.middlewares.Add(middleware);
        }

        /// <summary>
        /// 创建所有中间件执行处理者
        /// </summary>
        /// <returns></returns>
        public InvokeDelegate<TContext> Build()
        {
            var handler = this.completedHandler;
            for (var i = this.middlewares.Count - 1; i >= 0; i--)
            {
                handler = this.middlewares[i](handler);
            }
            return handler;
        }
    }
}
