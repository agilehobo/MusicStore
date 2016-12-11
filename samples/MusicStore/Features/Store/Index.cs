// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Features.Store
{
    public class Index
    {
        public class Query : IAsyncRequest<List<Genre>>
        {
        }

        public class Handler : IAsyncRequestHandler<Query, List<Genre>>
        {
            private readonly MusicStoreContext _dbContext;

            public Handler(MusicStoreContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<List<Genre>> Handle(Query message)
            {
                return await _dbContext.Genres.ToListAsync();
            }
        }
    }
}
