using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Brand")]
    [Authorize(Roles ="Admin,Publisher,Author")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext Context)
        {
            _dataContext = Context;
        }
        [HttpGet("Index")]
        //public async Task<IActionResult> Index()
        // {
        //     return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
        // }
        public async Task<IActionResult> Index(int page = 1)
        {
            List<BrandModel> Brands = _dataContext.Brands.ToList();
            const int pageSize = 10;
            if (page < 1)
            {
                page = 1;
            }
            int recsCount = Brands.Count();
            var paper = new Paginate(recsCount, page, pageSize);
            int recSkip = (page - 1) * pageSize; //VD: (trang 3 - 1) * 10 = 20
            //category.Skip(20).Take(10).ToList(); data bat dau chay tu 20 va chay 10 item (chay den 30)
            var data = Brands.Skip(recSkip).Take(paper.PageSize).ToList();
            ViewBag.Paper = paper;
            return View(data);
        }

        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {

            //lay du lieu tu form vao csdl
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong database");
                    return View(brand);
                }

                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction("Index");
            }
            else
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
                return BadRequest(errorMessage);

            }
            return View(brand);
        }
        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }
		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(BrandModel brand)
		{
			// Kiểm tra tính hợp lệ của model
			if (ModelState.IsValid)
			{
				// Kiểm tra trùng lặp Slug
				brand.Slug = brand.Name.Replace(" ", "-");
				var existingBrand = await _dataContext.Brands
													   .FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);
				if (existingBrand != null)
				{
					ModelState.AddModelError("", "Thương hiệu đã có trong database.");
					return View(brand);
				}

				// Cập nhật thương hiệu vào cơ sở dữ liệu
				_dataContext.Update(brand);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật thương hiệu thành công.";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "Model không hợp lệ.";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}
		}
		[Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            if (brand == null)
            {
                return NotFound(); //handle product not found
            }

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thương hiệu đã xóa";
            return RedirectToAction("Index");
        }
    }
}
