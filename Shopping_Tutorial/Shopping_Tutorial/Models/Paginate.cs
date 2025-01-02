namespace Shopping_Tutorial.Models
{
    public class Paginate
    {
        public int TotalItems { get;private set; } 
        public int PageSize { get;private set; } //tong so items / trang
        public int CurrentPage { get;private set; } //trang hien tai
        public int TotalPages { get;private set; }
        public int StartPage { get;private set; }
        public int EndPage { get;private set; }
        public Paginate()
        {

        }
        public Paginate(int totalItems, int page, int pageSize = 10)
        {
            //lam tron tong items/10 items tren 1 trang VD: 33 items/10 = tron` 4 trang
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page; //page hien tai = 1
            int startPage = currentPage - 5; //trang bat dau tru` 5 button
            int endPage = currentPage + 4; //trang cuoi se cong them 4 button
            if(startPage <= 0)
            {
                //neu so trang bat dau nho hon hoac = 0 thi so trang cuoi se bang
                endPage = endPage - (startPage - 1); //6 - (-3 -1) = 10
                startPage = 1;
            }
            if(endPage > totalPages) 
            {
                endPage = totalPages; 
                if(endPage > 10) 
                {
                    startPage = endPage - 9;
                }
            }
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;

        }
    }
}
