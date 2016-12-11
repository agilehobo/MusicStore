// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Features.Store
{
    public class StoreController : Controller
    {
        private readonly IMediator _mediator;

        public StoreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _mediator.SendAsync(new Index.Query());

            return View(viewModel);
        }

        public async Task<IActionResult> Browse(string genre)
        {
            var viewModel = await _mediator.SendAsync(new Browse.Query(genre));

            return View(viewModel);
        }
    }
}
