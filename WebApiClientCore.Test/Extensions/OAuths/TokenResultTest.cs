using System;
using System.Threading;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.Exceptions;
using Xunit;

namespace WebApiClientCore.Test.Extensions.OAuths
{
    /// <summary>
    /// TokenResult 过期判断测试
    /// </summary>
    public class TokenResultTest
    {
        #region 基础过期判断测试

        [Fact]
        [Trait("Category", "BasicExpiration")]
        public void IsExpired_WithValidToken_ReturnsFalse()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 3600 };

            // Act
            var result = token.IsExpired();

            // Assert
            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "BasicExpiration")]
        public void IsExpired_WithExpiredToken_ReturnsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 0 };
            // 直接使用 expires_in=0，避免等待

            // Act
            var result = token.IsExpired();

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "BasicExpiration")]
        public void IsExpired_AtExactExpirationTime_ReturnsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 0 };

            // Act
            var result = token.IsExpired();

            // Assert
            Assert.True(result, "Token with expires_in=0 should be immediately expired");
        }

        #endregion

        #region 刷新窗口过期判断测试

        [Fact]
        [Trait("Category", "RefreshWindow")]
        public void IsExpired_OutsideRefreshWindow_ReturnsFalse()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 300 }; // 5 minutes
            var refreshWindow = TimeSpan.FromSeconds(60); // 1 minute

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.False(result, "Token with 5 minutes remaining should not trigger refresh with 1 minute window");
        }

        [Fact]
        [Trait("Category", "RefreshWindow")]
        public void IsExpired_JustEnteredRefreshWindow_ReturnsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 2 }; // 2 seconds
            var refreshWindow = TimeSpan.FromSeconds(60); // 1 minute

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.True(result, "Token with 2 seconds lifetime should be in refresh window immediately with 60 second window");
        }

        [Fact]
        [Trait("Category", "RefreshWindow")]
        public void IsExpired_AtRefreshWindowBoundary_ReturnsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 60 }; // Exactly 60 seconds
            var refreshWindow = TimeSpan.FromSeconds(60);

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.True(result, "Token at exact refresh window boundary should trigger refresh");
        }

        [Theory]
        [InlineData(30, false)]  // 120秒有效期，30秒窗口，剩余120秒 > 30秒，不在窗口内
        [InlineData(60, false)]  // 120秒有效期，60秒窗口，剩余120秒 > 60秒，不在窗口内
        [InlineData(90, false)]  // 120秒有效期，90秒窗口，剩余120秒 > 90秒，不在窗口内
        [InlineData(120, true)]  // 120秒有效期，120秒窗口，剩余120秒 = 120秒，在窗口内（临界点）
        [Trait("Category", "RefreshWindow")]
        public void IsExpired_WithDifferentRefreshWindows_BehavesCorrectly(int windowSeconds, bool expectedExpired)
        {
            // Arrange
            var token = new TokenResult { Expires_in = 120 }; // 2 minutes
            var refreshWindow = TimeSpan.FromSeconds(windowSeconds);

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.Equal(expectedExpired, result);
        }

        [Fact]
        [Trait("Category", "RefreshWindow")]
        public void IsExpired_AfterActualExpiration_ReturnsTrueRegardlessOfWindow()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 0 };
            // 使用 expires_in=0 模拟过期
            var refreshWindow = TimeSpan.Zero;

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.True(result, "Expired token should return true even with zero refresh window");
        }

        #endregion

        #region 边界情况测试

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void IsExpired_WithZeroExpiresIn_ReturnsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 0 };

            // Act
            var result = token.IsExpired();

            // Assert
            Assert.True(result, "Token with expires_in=0 should be immediately expired");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void IsExpired_WithNegativeExpiresIn_ReturnsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = -100 };

            // Act
            var result = token.IsExpired();

            // Assert
            Assert.True(result, "Token with negative expires_in should be expired");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void IsExpired_RefreshWindowLargerThanExpiresIn_ReturnsTrueImmediately()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 60 }; // 1 minute
            var refreshWindow = TimeSpan.FromSeconds(120); // 2 minutes

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.True(result, "When refresh window is larger than expires_in, token should be in refresh window immediately");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public void IsExpired_VeryShortExpiresIn_BehavesCorrectly()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 2 }; // 2 seconds
            var refreshWindow = TimeSpan.FromSeconds(60);

            // Act
            var result = token.IsExpired(refreshWindow);

            // Assert
            Assert.True(result, "Very short lifetime token should not cause exceptions");
        }

        #endregion

        #region 时间流逝模拟测试

        [Fact]
        [Trait("Category", "TimeProgression")]
        public void IsExpired_ProgressionFromValidToRefreshWindow_TransitionsCorrectly()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 1 }; // 1 second
            var refreshWindow = TimeSpan.FromSeconds(0.5);

            // Act & Assert - Initially outside refresh window
            Assert.False(token.IsExpired(refreshWindow));

            // Wait a small amount of time to enter refresh window
            Thread.Sleep(600); // After 0.6 seconds, remaining time < 0.5 seconds (refresh window)
            Assert.True(token.IsExpired(refreshWindow), "Should be in refresh window after some time");
        }

        [Fact]
        [Trait("Category", "TimeProgression")]
        public void IsExpired_ProgressionFromRefreshWindowToExpired_RemainsTrue()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 1 }; // 1 second
            var refreshWindow = TimeSpan.FromSeconds(0.5);

            // Act & Assert - Check at different points
            Thread.Sleep(600); // After 0.6 seconds, in refresh window
            Assert.True(token.IsExpired(refreshWindow));

            Thread.Sleep(500); // After 1.1 seconds, actually expired
            Assert.True(token.IsExpired(refreshWindow));
        }

        [Fact]
        [Trait("Category", "TimeProgression")]
        public void IsExpired_MultipleChecks_ConsistentResults()
        {
            // Arrange
            var token = new TokenResult { Expires_in = 3600 };
            bool? firstResult = null;

            // Act - Multiple rapid checks
            for (int i = 0; i < 100; i++)
            {
                var result = token.IsExpired();
                if (!firstResult.HasValue)
                {
                    firstResult = result;
                }
                else
                {
                    // Assert
                    Assert.Equal(firstResult.Value, result);
                }
            }
        }

        #endregion

        #region IsSuccess 和 EnsureSuccess 方法测试

        [Theory]
        [InlineData("valid_token", true)]
        [InlineData(null, false)]
        [InlineData("", false)]
        [Trait("Category", "IsSuccess")]
        public void IsSuccess_WithDifferentAccessTokens_ReturnsCorrectly(string? accessToken, bool expectedResult)
        {
            // Arrange
            var token = new TokenResult { Access_token = accessToken, Expires_in = 3600 };

            // Act
            var result = token.IsSuccess();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        [Trait("Category", "EnsureSuccess")]
        public void EnsureSuccess_WithValidToken_ReturnsSameToken()
        {
            // Arrange
            var token = new TokenResult { Access_token = "valid_token", Expires_in = 3600 };

            // Act
            var result = token.EnsureSuccess();

            // Assert
            Assert.Same(token, result);
        }

        [Fact]
        [Trait("Category", "EnsureSuccess")]
        public void EnsureSuccess_WithInvalidToken_ThrowsTokenException()
        {
            // Arrange
            var token = new TokenResult { Access_token = null, Expires_in = 3600, Error = "invalid_token" };

            // Act & Assert
            var exception = Assert.Throws<TokenException>(() => token.EnsureSuccess());
            Assert.Equal("invalid_token", exception.Message);
        }

        #endregion

        #region CanRefresh 方法测试

        [Theory]
        [InlineData("valid_refresh_token", true)]
        [InlineData(null, false)]
        [InlineData("", false)]
        [Trait("Category", "CanRefresh")]
        public void CanRefresh_WithDifferentRefreshTokens_ReturnsCorrectly(string? refreshToken, bool expectedResult)
        {
            // Arrange
            var token = new TokenResult { Access_token = "valid_token", Refresh_token = refreshToken, Expires_in = 3600 };

            // Act
            var result = token.CanRefresh();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion
    }
}
