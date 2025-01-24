using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Tutorial.Repository.Components
{
	public class FooterViewComponent : ViewComponent
	{
		public readonly DataContext _dataContext;
		public FooterViewComponent(DataContext Context)
		{
			_dataContext = Context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Contact.FirstOrDefaultAsync());
	}
}
