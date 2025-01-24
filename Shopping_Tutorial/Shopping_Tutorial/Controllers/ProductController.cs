using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Controllers
{

	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		public ProductController(DataContext Context)
		{
			_dataContext = Context;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Details(int Id)
		{
			if(Id == null) return RedirectToAction("Index");
			var productsById = _dataContext.Products.Include(p=>p.Ratings).Where(p => p.Id == Id).FirstOrDefault();


			//san pham lien quan khi xem chi tiet san pham
			var relatedProducts = await _dataContext.Products
				.Where(p=>p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
				.Take(4) //lay 4 san pham
				.ToListAsync();
			ViewBag.RelatedProducts = relatedProducts;

			//Danh gia
			var viewModel = new ProductDetailsViewModel
			{
				ProductDetails = productsById				
			};

			return View(viewModel);	
		}
		public async Task<IActionResult> Search(string searchTerm)
		{
			var products = await _dataContext.Products
				.Where(p=>p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
				.ToListAsync();
			ViewBag.Keyword = searchTerm;
			return View(products);
		}

		public async Task<IActionResult> CommentProduct(RatingModel rating)
		{
			if (ModelState.IsValid) 
			{
				var ratingEntity = new RatingModel 
				{
					ProductId = rating.ProductId,
					Name = rating.Name,
					Email = rating.Email,
					Comment = rating.Comment,
					Star = rating.Star
				};

				_dataContext.Ratings.Add(ratingEntity);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm đánh giá thành công";

				return Redirect(Request.Headers["Referer"]); //tra ve trang truoc đó
			}
			/*else
			{
				TempData["error"] = "Model đang có một vài thứ bị lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);

				return RedirectToAction("Details", new { id = rating.ProductId });
			}*/
			return Redirect(Request.Headers["Referer"]);
		}
	}
}
