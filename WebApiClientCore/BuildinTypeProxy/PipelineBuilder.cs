using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义Action执行的委托
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns></returns>
    delegate Task ExecutionDelegate(ApiActionContext context);

    /// <summary>
    /// 表示中间件创建者
    /// </summary>
    class PipelineBuilder
    {
        private readonly ExecutionDelegate completedHandler;
        private readonly List<Func<ExecutionDelegate, ExecutionDelegate>> middlewares = new List<Func<ExecutionDelegate, ExecutionDelegate>>();

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
        public PipelineBuilder(ExecutionDelegate completedHandler)
        {
            this.completedHandler = completedHandler;
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        public PipelineBuilder Use(Func<ExecutionDelegate, ExecutionDelegate> middleware)
        {
            this.middlewares.Add(middleware);
            return this;
        }

        /// <summary>
        /// 使用中间件
        /// </summary>  
        /// <param name="middleware"></param>
        /// <returns></returns>
        public PipelineBuilder Use(Func<ApiActionContext, Func<Task>, Task> middleware)
        {
            return this.Use(next => context => middleware(context, () => next(context)));
        }

        /// <summary>
        /// 创建所有中间件执行处理者
        /// </summary>
        /// <returns></returns>
        public ExecutionDelegate Build()
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
