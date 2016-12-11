using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Features.Home
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _mediator.SendAsync(new Index.Query());

            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View("~/Features/Shared/Error.cshtml");
        }

        public IActionResult StatusCodePage()
        {
            return View("~/Features/Shared/StatusCodePage.cshtml");
        }

        public IActionResult AccessDenied()
        {
            return View("~/Features/Shared/AccessDenied.cshtml");
        }
    }
}
