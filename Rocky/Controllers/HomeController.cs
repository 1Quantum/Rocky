﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky_DataAccess;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
	 public class HomeController : Controller
	 {

		private readonly ILogger<HomeController> _logger; 
		private readonly ApplicationDbContext _db;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
		{
			_logger = logger;
			_db = db;
		}

		public IActionResult Index()
		{
			HomeVM homeVM = new HomeVM()
			{
				Products = _db.Product.Include(x => x.Category).Include(x => x.ApplicationType),
				Categories = _db.Category
			};

			return View(homeVM);
		}
		 
		public IActionResult Details(int id)
		{
			List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
			if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
			{
				shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
			}



			DetailsVM detailsVM = new DetailsVM()
			{
				Product = _db.Product.Include(x => x.Category).Include(x => x.ApplicationType).Where(x => x.Id == id).FirstOrDefault(),
				ExistsInCart = false,
			};

			foreach (var items in shoppingCartList)
			{
				if (items.ProductId == id)
				{
					detailsVM.ExistsInCart = true;
				}
			}

			return View(detailsVM);
		}

		[HttpPost, ActionName("Details")]
		public IActionResult DetailsPost(int id)
		{
			List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
			if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
			{
				shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
			}
			shoppingCartList.Add(new ShoppingCart { ProductId = id });
			HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
			return RedirectToAction(nameof(Index));
		}
		
		public IActionResult RemoveFromCart(int id)
		{
			List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
			if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
			{
				shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
			}

			var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == id);
			if (itemToRemove != null)
         {
				shoppingCartList.Remove(itemToRemove);
         }

			HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	 }
}
