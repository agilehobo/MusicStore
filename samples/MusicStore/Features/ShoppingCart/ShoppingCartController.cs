// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Features.ShoppingCart
{
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
            var cmd = new AddToCart.Command(cartId, id);
            await _mediator.SendAsync(cmd, requestAborted);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var cartId = Models.ShoppingCart.GetCartId(HttpContext);
            var viewModel = await _mediator.SendAsync(new Index.Query(cartId));

            // Return the view
            return View(viewModel);
        }
    }
}
