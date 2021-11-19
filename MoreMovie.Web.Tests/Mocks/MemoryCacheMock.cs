using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;

namespace MoreMovie.Web.Tests.Mocks
{
    public static class MemoryCacheMock
    {

        public static IMemoryCache Instanse
        {
            get
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                var cache = new Mock<IMemoryCache>();

                return cache.Object;
            }
        }
    }
}
