using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Tutorial.Repository.Components
{
	public class CategoriesViewComponent : ViewComponent
	{
		public readonly DataContext _dataContext;
		public CategoriesViewComponent(DataContext Context)
		{
			_dataContext = Context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Categories.ToListAsync());
	}
}
