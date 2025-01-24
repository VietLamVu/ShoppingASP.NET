using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Slider")]
	[Authorize(Roles = "Admin")]
	public class SliderController : Controller
	{
		private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SliderController(DataContext Context, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = Context;
            _webHostEnvironment = webHostEnvironment;
        }
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
		     return View(await _dataContext.Sliders.OrderByDescending(p => p.Id).ToListAsync());
		}
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //lay du lieu tu form create 
        public async Task<IActionResult> Create(SliderModel slider)
        {
            

            //lay du lieu tu form vao csdl
            if (ModelState.IsValid)
            {
                            
                    if (slider.ImageUpload != null)
                    {
                        string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                        string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadDirectory, imageName);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await slider.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        slider.Image = imageName;

                    }
                
                _dataContext.Add(slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm slider thành công";
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
            return View(slider);
        }
        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(Id);
            
            return View(slider);
        }
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //lay du lieu tu form create 
        public async Task<IActionResult> Edit(SliderModel slider)
        {
            var slider_existed = _dataContext.Sliders.Find(slider.Id);
            //lay du lieu tu form vao csdl
            if (ModelState.IsValid)
            {

                if (slider.ImageUpload != null)
                {
                    string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                    string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDirectory, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    slider_existed.Image = imageName;

                }

                slider_existed.Name = slider.Name;
                slider_existed.Description = slider.Description;
                slider_existed.Status = slider.Status;

                _dataContext.Update(slider_existed);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật slider thành công";
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
            return View(slider);
        }
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(Id);
            if (slider == null)
            {
                return NotFound(); //handle product not found
            }

            string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
            string oldFilePath = Path.Combine(uploadDirectory, slider.Image);
            try
            {
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the slider image");
            }

            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "slider đã xóa thành công";
            return RedirectToAction("Index", "Slider", new { area = "Admin" });
        }
    }
}
