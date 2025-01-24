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
    [Route("Admin/Contact")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactController(DataContext Context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = Context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("Index")]
        public IActionResult Index()
        {
            var contact = _dataContext.Contact.ToList();
            return View(contact);
        }
        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            ContactModel contact = await _dataContext.Contact.FirstOrDefaultAsync();
            return View(contact);
        }
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //lay du lieu tu form create 
        public async Task<IActionResult> Edit(ContactModel contact)
        {          
            var existed_contact = _dataContext.Contact.FirstOrDefault();

            //lay du lieu tu form vao csdl
            if (ModelState.IsValid)
            {
           

                if (contact.ImageUpload != null)
                {

                    //Upload new image
                    string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                    string imageName = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDirectory, imageName);
                    if (!string.IsNullOrEmpty(existed_contact.LogoImg))
                    {
                        string oldFilePath = Path.Combine(uploadDirectory, existed_contact.LogoImg);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await contact.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_contact.LogoImg = imageName;

                }
                // Update other product properties
                existed_contact.Name = contact.Name;
                existed_contact.Email = contact.Email;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Map = contact.Map;

                _dataContext.Update(existed_contact);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thông tin website thành công";
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
            return View(contact);
        }
    }
}
