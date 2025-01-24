using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Shipping")]
    [Authorize(Roles = "Admin")]
    public class ShippingController : Controller
	{
        private readonly DataContext _dataContext;
        public ShippingController(DataContext Context)
        {
            _dataContext = Context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
		{
            var shippingList = await _dataContext.Shippings.ToListAsync();
            ViewBag.Shippings = shippingList;
			return View();
		}
        [Route("StoreShipping")]
        [HttpPost]
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, decimal price)
        {
            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var existingShipping = await _dataContext.Shippings.AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);
                if(existingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp" });
                }
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, messgae = "Thêm giá vận chuyển thành công" });
            }catch(Exception ex)
            {
                return StatusCode(500, "Xảy ra lỗi trong khi thêm shipping");
            }
        }
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shipping = await _dataContext.Shippings.FindAsync(Id);
            _dataContext.Shippings.Remove(shipping);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thành công giá vận chuyển";
            return RedirectToAction("Index","Shipping");
        }
    }
}
