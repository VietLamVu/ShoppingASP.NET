using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Nhập Username")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Nhập Username"), EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage ="Nhập Password")]
		public string Password { get; set; }
	}
}
