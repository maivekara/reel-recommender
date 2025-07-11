using Microsoft.AspNetCore.Mvc;
using FilmOneriProjesi.Models;

namespace FilmOneriProjesi.Controllers
{
    public class DefaultController : Controller
    {
        private readonly OpenAiService _openAiService;
        public DefaultController(OpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpGet]
        public IActionResult CreateRecipe()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(string ingredients)
            {
                var result = await _openAiService.GetMovieSuggestionAsync(ingredients);
                ViewBag.recipe = result;
                return View();
            }

    }
}
