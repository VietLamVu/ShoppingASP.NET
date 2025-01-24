using Shopping_Tutorial.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Tutorial.Models
{
	public class SliderModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên slider")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập mô tả slider")]
		public string Description { get; set; }
		public int? Status { get; set; }
		public string Image { get; set; }
		[NotMapped]
		[FileExtention]
		public IFormFile? ImageUpload { get; set; }
	}
}
