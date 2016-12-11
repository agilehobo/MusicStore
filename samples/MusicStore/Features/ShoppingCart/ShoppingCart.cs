// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Models;

namespace MusicStore.Features.ShoppingCart
{
    public class AddToCart : ICancellableAsyncRequest<Unit>
    {
        public string CartId { get; private set; }
        public int AlbumId { get; private set; }

        public AddToCart(string cartId, int albumId)
        {
            CartId = cartId;
            AlbumId = albumId;
        }
    }

    public class AddToCartHandler : ICancellableAsyncRequestHandler<AddToCart, Unit>
    {
        private readonly MusicStoreContext _dbContext;
        private readonly ILogger<ShoppingCartController> _logger;

        public AddToCartHandler(MusicStoreContext dbContext, ILogger<ShoppingCartController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddToCart message, CancellationToken cancellationToken)
        {
            // Retrieve the album from the database
            var addedAlbum = await _dbContext.Albums.SingleAsync(album => album.AlbumId == message.AlbumId, cancellationToken);

            // Add it to the shopping cart
            var cart = Models.ShoppingCart.GetCart(_dbContext, message.CartId);

            await cart.AddToCart(addedAlbum);

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Album {albumId} was added to the cart.", addedAlbum.AlbumId);

            return Unit.Value;
        }
    }

    public class ShoppingCartController : Controller
    {
        private readonly IMediator _mediator;

        public ShoppingCartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> AddToCart(int id, CancellationToken requestAborted)
        {
            var cartId = Models.ShoppingCart.GetCartId(HttpContext);
            var cmd = new AddToCart(cartId, id);
            await _mediator.SendAsync(cmd, requestAborted);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }
    }
}
