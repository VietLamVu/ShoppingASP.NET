using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Controllers
{
	public class CartController : Controller
	{
		private readonly DataContext _dataContext;
		public CartController(DataContext context) 
		{
			_dataContext = context;
		}
		public IActionResult Index()
		{
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

			//nhận giá shipping từ cookie
			var shippingPriceCookie = Request.Cookies["ShippingPrice"]; //ShippingPrice ở Response.Cookies.Append("ShippingPrice",shippingPriceJson,cookieOptions);
			decimal shippingPrice = 0;
			if(shippingPriceCookie != null)
			{
				var shippingPriceJson = shippingPriceCookie;
				shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
			}


			CartItemViewModel cartVM = new()
			{
				CartItems = cartItems,
				GrandTotal = cartItems.Sum(x => x.Quantity*x.Price),
				ShippingCost = shippingPrice

			};
			return View(cartVM);
		}
		public IActionResult Checkout()
		{
			return View("~/Views/Checkout/Index.cshtml");
		}
		
		
		public async Task<IActionResult> Add(long Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);		
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();

			if(cartItems == null) 
			{
				cart.Add(new CartItemModel(product));
			}
			else
			{
				cartItems.Quantity += 1;
			}

			//luu tru du lieu cart vao session cart
			HttpContext.Session.SetJson("Cart", cart);



			TempData["success"] = "Thêm sản phẩm thành công";
			return Redirect(Request.Headers["Referer"].ToString());

		}

		public async Task<IActionResult> Decrease(long Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
			if(cartItems.Quantity > 1)
			{
				--cartItems.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.ProductId == Id);
			}
			if(cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart",cart);
			}

			TempData["success"] = "Giảm số lượng thành công";
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Increase(long Id)
		{
			ProductModel product = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();

			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
			if (cartItems.Quantity >= 1 && product.Quantity > cartItems.Quantity)
			{
				++cartItems.Quantity;
				TempData["success"] = "Tăng số lượng thành công";
			}
			else
			{
				cartItems.Quantity = product.Quantity;
				TempData["success"] = "Số lượng sản phẩm đạt tối đa";
			}
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Increase Item quantity to cart Successfully";
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Remove(long Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			cart.RemoveAll(p => p.ProductId == Id);
			if(cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Remove Item of cart Successfully";
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Clear()
		{
			HttpContext.Session.Remove("Cart");

			TempData["success"] = "Clear all Item of cart Successfully";
			return RedirectToAction("Index");
		}

		//tính phí ship
		[Route("GetShipping")]
		[HttpPost]
		public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string phuong, string tinh)
		{
			var existingShipping = await _dataContext.Shippings.FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);
			decimal shippingPrice = 0;

			if(existingShipping != null) //neu tim thay du lieu thi shippingPrice = gia ship o trong csdl
			{
				shippingPrice = existingShipping.Price;
			}
			else
			{
				shippingPrice = 15000; //set giá tiền mặc định nếu chưa create giá tiền ở admin
			}
			var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
			try
			{
				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					Expires = DateTimeOffset.UtcNow.AddMinutes(30),
					Secure = true
				};
				Response.Cookies.Append("ShippingPrice",shippingPriceJson,cookieOptions); //đẩy shippingPriceJson vào shippingPriceJson với tên là ShippingPrice
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return Json(new { shippingPrice });
		}

		[Route("DeleteShipping")]
		[HttpGet]
		public IActionResult DeleteShipping()
		{
			Response.Cookies.Delete("ShippingPrice");
			return RedirectToAction("Index","Cart");
		}
	}
}
