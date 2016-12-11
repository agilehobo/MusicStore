// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicStore.Models;

namespace MusicStore.Features.Store
{
    public class Details
    {
        public class Query : IAsyncRequest<Result>
        {
            public Query(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        public class Result
        {
            public Album Album { get; set; }
        }

        public class Handler : IAsyncRequestHandler<Query, Result>
        {
            private readonly IMemoryCache _cache;
            private readonly MusicStoreContext _dbContext;
            private readonly IOptions<AppSettings> _options;

            public Handler(MusicStoreContext dbContext, IMemoryCache cache, IOptions<AppSettings> options)
            {
                _dbContext = dbContext;
                _cache = cache;
                _options = options;
            }

            public async Task<Result> Handle(Query message)
            {
                var cacheKey = string.Format("album_{0}", message.Id);
                Album album;

                if (!_cache.TryGetValue(cacheKey, out album))
                {
                    album = await _dbContext.Albums
                        .Where(a => a.AlbumId == message.Id)
                        .Include(a => a.Artist)
                        .Include(a => a.Genre)
                        .FirstOrDefaultAsync();

                    if (album != null)
                        if (_options.Value.CacheDbResults)
                            _cache.Set(
                                cacheKey,
                                album,
                                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                }

                return new Result { Album = album };
            }
        }
    }
}
