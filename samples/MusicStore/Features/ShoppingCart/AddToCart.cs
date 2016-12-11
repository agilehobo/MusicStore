// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Models;

namespace MusicStore.Features.ShoppingCart
{
    public class AddToCart
    {
        public class Command : ICancellableAsyncRequest
        {
            public Command(string cartId, int albumId)
            {
                CartId = cartId;
                AlbumId = albumId;
            }

            public string CartId { get; }
            public int AlbumId { get; }
        }

        public class Handler : CancellableAsyncRequestHandler<Command>
        {
            private readonly MusicStoreContext _dbContext;
            private readonly ILogger<ShoppingCartController> _logger;

            public Handler(MusicStoreContext dbContext, ILogger<ShoppingCartController> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            protected override async Task HandleCore(Command message, CancellationToken cancellationToken)
            {
                // Retrieve the album from the database
                var addedAlbum = await _dbContext.Albums.SingleAsync(album => album.AlbumId == message.AlbumId, cancellationToken);

                // Add it to the shopping cart
                var cart = Models.ShoppingCart.GetCart(_dbContext, message.CartId);

                await cart.AddToCart(addedAlbum);

                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Album {albumId} was added to the cart.", addedAlbum.AlbumId);
            }
        }
    }
}
