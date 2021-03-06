﻿using System;
using System.Web.Mvc;
using MyMovies.DomainModel;
using MyMovies.DomainModel.Services;

namespace MyMovies.WebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMoviesService _moviesService;

        public MoviesController(IMoviesService moviesService)
        {
            _moviesService = moviesService;
        }

        //
        // GET: /Movies/
        public ActionResult Index()
        {
            return View(_moviesService.GetAllMovies());
        }


        public ActionResult Details(int id)
        {

            Movie movie = _moviesService.Get(id);
            if(movie == null)
            {
                //return RedirectToAction("Index");
                return View("NotFound", id);
            }
            return View(movie);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string Title, string  fill)
        {
            Movie newMovie;
            if(fill != null)
            {
                newMovie = _moviesService.Search(Title);
                ModelState.Remove("Title");
                return View(newMovie);
            }

            TryUpdateModel(newMovie  = new Movie());
            if (ModelState.IsValid)
            {
                _moviesService.Add(newMovie);
                return RedirectToAction("Details", new { id = newMovie.ID });
            }
            else
            {
                return View(newMovie);
            }
        }

        [HttpGet]
        public JsonResult TitleInfo(string Title)
        {
            Title = Server.UrlDecode(Title);
            return Json(_moviesService.Search(Title), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(int id)
        {
            return View(_moviesService.Get(id));
        }

        [HttpPost]
        [ActionName("Edit")]
        public ActionResult EditPost(int id)
        {
            Movie movie = _moviesService.Get(id);
            try
            {
                UpdateModel(movie);
                if (ModelState.IsValid)
                {
                    _moviesService.Update(movie);
                    return RedirectToAction("Details", new {id});
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", String.Format("Edit Failure, inner exception: {0}", e));
            }

            return View(movie);
        }


        public ActionResult Delete(int id)
        {
            return View(_moviesService.Get(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeletePost(int id)
        {
            try
            {
                _moviesService.Delete(id);
                TempData["Message"] = String.Format("Product with id {0} successfully removed!", id);
            }
            catch (ArgumentException e)
            {
                TempData["Message"] = e.Message;
            }

            return RedirectToAction("Index");
        }


        public ActionResult CreateComment(int movieId)
        {
            Comment c = new Comment {MovieID = movieId};
            return View(c);
        }

        [HttpPost]
        public ActionResult AddComment(int movieId, string comment, int rating)
        {
            Comment c = new Comment() { Description = comment, Rating = rating};
            Movie movie = _moviesService.Get(movieId);
            movie.Comments.Add(c);
            _moviesService.Update(movie);
            return Json( true );
        }

        [HttpPost]
        public ActionResult CreateComment(Comment c)
        {
            try
            {
                if (ModelState.IsValid) {
                    Movie movie = _moviesService.Get(c.MovieID);
                    movie.Comments.Add(c);
                    _moviesService.Update(movie);
                    return RedirectToRoute("Default", new { action = "Details", id = c.MovieID });
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", String.Format("Edit Failure, inner exception: {0}", e));
            }

            return View(c);
        }
    }
}
