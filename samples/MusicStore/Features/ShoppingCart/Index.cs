// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using MusicStore.Models;

namespace MusicStore.Features.ShoppingCart
{
    public class Index
    {
        public class Result
        {
            public List<CartItem> CartItems { get; set; }
            public decimal CartTotal { get; set; }
        }

        public class Query : IAsyncRequest<Result>
        {
            public Query(string cartId)
            {
                CartId = cartId;
            }

            public string CartId { get; }
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
                var cart = Models.ShoppingCart.GetCart(_dbContext, message.CartId);

                return new Result
                {
                    CartItems = await cart.GetCartItems(),
                    CartTotal = await cart.GetTotal()
                };
            }
        }
    }
}
