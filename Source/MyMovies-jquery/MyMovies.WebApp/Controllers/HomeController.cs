using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMovies.DomainModel.Services;

namespace MyMovies.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMoviesService _moviesService;

        public HomeController(IMoviesService moviesService)
        {
            _moviesService = moviesService;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Reset()
        {
            // Remove all movies
            _moviesService
                .GetAllMovies()
                .ToList()
                .ForEach(movie => _moviesService.Delete(movie.ID));

            // Add default movies...
            new [] { "Shrek 1", "Shrek 2", "Shrek 3",
                     "Back to future 1", "Back to future 2", "Back to future 3",
                     "Lord of the rings", "Lord of the rings tower",
                     "The ring"
                    }
                .ToList()
                .ForEach( title => _moviesService.Add(_moviesService.Search(title)) );

            return View();
        }


    }
}
