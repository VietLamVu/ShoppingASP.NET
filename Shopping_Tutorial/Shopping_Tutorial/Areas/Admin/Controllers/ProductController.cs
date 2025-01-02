using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Route("Admin/Product")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(DataContext Context, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = Context;
			_webHostEnvironment = webHostEnvironment;
		}
        [Route("Index")]
        public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
		}
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
			ViewBag.Categories = new SelectList(_dataContext.Categories,"Id","Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        [Route("Create")]
        [HttpPost]
		[ValidateAntiForgeryToken]
		//lay du lieu tu form create 
		public async Task<IActionResult> Create(ProductModel product)
		{
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			//lay du lieu tu form vao csdl
			if(ModelState.IsValid)
			{
                product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "sản phẩm đã có trong database");
					return View(product);
				}
				else { 
					if(product.ImageUpload != null)
					{
						string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath,"media/products");
						string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
						string filePath = Path.Combine(uploadDirectory, imageName);
						FileStream fs = new FileStream(filePath, FileMode.Create);
						await product.ImageUpload.CopyToAsync(fs);
						fs.Close();
						product.Image = imageName;

					}
                }
                _dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
				return RedirectToAction("Index");
            }
			else
			{
				TempData["error"] = "Model đang có một vài thứ bị lỗi";
				List<string> errors = new List<string>();
				foreach(var value in ModelState.Values)
				{ 
					foreach(var error in value.Errors) 
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
				
			}
			return View(product);
        }
        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id) 
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            return View(product);
		}
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //lay du lieu tu form create 
        public async Task<IActionResult> Edit(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            var existed_product = _dataContext.Products.Find(product.Id); //tim sp theo id product
            //lay du lieu tu form vao csdl
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                
                    if (product.ImageUpload != null)
                    {
                        
                        //Upload new image
                        string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadDirectory, imageName);
                        //delete  old image
                        string oldFilePath = Path.Combine(uploadDirectory, existed_product.Image);
                        try
                        {
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An error occurred while deleting the product image");
                        }
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        existed_product.Image = imageName;

                    }
                    // Update other product properties
                    existed_product.Name = product.Name;
                    existed_product.Description = product.Description;
                    existed_product.Price = product.Price;
                    existed_product.CategoryId = product.CategoryId;
                    existed_product.BrandId = product.BrandId;
                
                _dataContext.Update(existed_product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
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
            return View(product);
        }
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if(product == null)
            {
                return NotFound(); //handle product not found
            }

            string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            string oldFilePath = Path.Combine(uploadDirectory, product.Image);
                try
                {
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath );
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while deleting the product image");
                }
            
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "sản phẩm đã xóa";
            return RedirectToAction("Index", "Admin/Products");  
        }
    }
}
