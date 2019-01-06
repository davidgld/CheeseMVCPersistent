﻿using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private readonly CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //List<Cheese> cheeses = context.Cheeses.ToList();
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();

            ViewBag.Title = "Cheeses";

            return View(cheeses);
        }

        public IActionResult Add()
        {
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory = context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);

                // Add the new cheese to my existing cheeses
                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }
            else
            {
                addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
                return View(addCheeseViewModel);
            }
        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        // .NET CORE default routing. /Controller/Action/id
        public IActionResult Category(int id)
        {
            if (id==0)
            {
                return Redirect("/Category");
            }

            CheeseCategory theCategory = context.Categories.Include(cat => cat.Cheeses).Single(cat => cat.ID == id);

           

            ViewBag.title = "Cheeses in category: " + theCategory.Name;
            return View("Index", theCategory.Cheeses);
        }

        public IActionResult Edit(int id)
        {
            Cheese theCheese = context.Cheeses.Single(c => c.ID == id);
            EditCheeseViewModel editCheeseViewModel = new EditCheeseViewModel(theCheese, context.Categories.ToList());
            return View(editCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditCheeseViewModel editCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory = context.Categories.Single(c => c.ID == editCheeseViewModel.CategoryID);

                Cheese theCheese = context.Cheeses.Single(c => c.ID == editCheeseViewModel.ID);

                theCheese.Name = editCheeseViewModel.Name;
                theCheese.Description = editCheeseViewModel.Description;
                theCheese.CategoryID = editCheeseViewModel.CategoryID;
                theCheese.Category = newCheeseCategory;

                

                context.Cheeses.Update(theCheese);
                context.SaveChanges();
                return Redirect("/Cheese");
            }
            else
            {
                return View(editCheeseViewModel);
            }
        }
    }
}
