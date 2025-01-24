using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Order")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext Context)
        {
            _dataContext = Context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            List<OrderModel> order = _dataContext.Orders.ToList();
            const int pageSize = 10;
            if (page < 1)
            {
                page = 1;
            }
            int recsCount = order.Count();
            var paper = new Paginate(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize; //VD: (trang 3 - 1) * 10 = 20
            //category.Skip(20).Take(10).ToList(); data bat dau chay tu 20 va chay 10 item (chay den 30)
            var data = order.Skip(recSkip).Take(paper.PageSize).ToList();
            ViewBag.Paper = paper;
            return View(data);
        }
        [Route("ViewOrder")]
        public async Task<IActionResult> ViewOrder(string Ordercode)
        {
            var DetailsOrder = await _dataContext.OrderDetails.Include(od=>od.Product).Where(od=>od.OrderCode==Ordercode).ToListAsync();

            //Lay shipping cost
            var ShippingCost = _dataContext.Orders.Where(o => o.OrderCode == Ordercode).First();
            ViewBag.ShippingCost = ShippingCost.ShippingCost;

            return View(DetailsOrder);
        }
        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o=>o.OrderCode==ordercode);
            if(order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công" });
            }catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi trong khi cập nhật trạng thái đơn hàng");
            }
        }
    }
}
