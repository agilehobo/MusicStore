// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;

namespace MusicStore.Features.Home
{
    public class Index
    {
        public class Query : IAsyncRequest<Result>
        {
        }

        public class Result
        {
            public List<Album> Albums { get; set; }
        }

        public class Handler : IAsyncRequestHandler<Query, Result>
        {
            private readonly MusicStoreContext _dbContext;
            private readonly IMemoryCache _cache;
            private readonly IOptions<AppSettings> _options;

            public Handler(MusicStoreContext dbContext, IMemoryCache cache, IOptions<AppSettings> options)
            {
                _dbContext = dbContext;
                _cache = cache;
                _options = options;
            }

            public async Task<Result> Handle(Query message)
            {
                // Get most popular albums
                var cacheKey = "topselling";
                List<Album> albums;
                if (!_cache.TryGetValue(cacheKey, out albums))
                {
                    albums = await GetTopSellingAlbumsAsync(_dbContext, 6);

                    if (albums != null
                        && albums.Count > 0)
                    {
                        if (_options.Value.CacheDbResults)
                        {
                            // Refresh it every 10 minutes.
                            // Let this be the last item to be removed by cache if cache GC kicks in.
                            _cache.Set(
                                cacheKey,
                                albums,
                                new MemoryCacheEntryOptions()
                                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                                    .SetPriority(CacheItemPriority.High));
                        }
                    }
                }

                return new Result { Albums = albums };
            }

            private Task<List<Album>> GetTopSellingAlbumsAsync(MusicStoreContext dbContext, int count)
            {
                // Group the order details by album and return
                // the albums with the highest count

                return dbContext.Albums
                    .OrderByDescending(a => a.OrderDetails.Count)
                    .Take(count)
                    .ToListAsync();
            }
        }
    }
}
