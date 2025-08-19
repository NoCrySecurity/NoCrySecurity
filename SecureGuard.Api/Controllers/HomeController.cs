using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecureGuard.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Rota padrão: / ou /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // Página de erro (usada no modo Production)
        public IActionResult Error()
        {
            _logger.LogError("Aconteceu um erro inesperado no sistema.");
            return View(); // procura Views/Shared/Error.cshtml
        }
    }
}
