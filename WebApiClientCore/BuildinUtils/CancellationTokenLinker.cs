using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示CancellationToken链接器
    /// </summary>
    struct CancellationTokenLinker : IDisposable
    {
        /// <summary>
        /// 链接产生的tokenSource
        /// </summary>
        private readonly CancellationTokenSource? tokenSource;

        /// <summary>
        /// 获取token
        /// </summary>
        public CancellationToken Token { get; }

        /// <summary>
        /// CancellationToken链接器
        /// </summary>
        /// <param name="tokenList"></param>
        public CancellationTokenLinker(IList<CancellationToken> tokenList)
        {
            if (IsNoneCancellationToken(tokenList) == true)
            {
                this.tokenSource = null;
                this.Token = CancellationToken.None;
            }
            else
            {
                this.tokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokenList.ToArray());
                this.Token = this.tokenSource.Token;
            }
        }

        /// <summary>
        /// 是否为None的CancellationToken
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        private static bool IsNoneCancellationToken(IList<CancellationToken> tokenList)
        {
            if (tokenList.Count == 0)
            {
                return true;
            }
            if (tokenList.Count == 1 && tokenList[0] == CancellationToken.None)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.tokenSource != null)
            {
                this.tokenSource.Dispose();
            }
        }
    }
}
