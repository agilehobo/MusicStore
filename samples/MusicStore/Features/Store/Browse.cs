// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;

namespace MusicStore.Features.Store
{
    public class Browse
    {
        public class Query : IAsyncRequest<Result>
        {
            public string Genre { get; }

            public Query(string genre)
            {
                Genre = genre;
            }
        }

        public class Result
        {
            public Genre Genre { get; set; }
        }

        public class Handler : IAsyncRequestHandler<Query, Result>
        {
            private readonly MusicStoreContext _dbContext;

            public Handler(MusicStoreContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result> Handle(Query message)
            {
                var genre = await _dbContext.Genres
                    .Include(g => g.Albums)
                    .Where(g => g.Name == message.Genre)
                    .FirstOrDefaultAsync();

                return new Result { Genre = genre };
            }
        }
    }
}
