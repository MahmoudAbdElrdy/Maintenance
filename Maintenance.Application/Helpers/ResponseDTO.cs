using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maintenance.Application.Helpers.Paginations;

namespace Maintenance.Application.Helper
{
    public class ResponseDTO
    {
        public dynamic Result { get; set; }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get;  set; }

        public string Message { get; set; }

        public StatusEnum StatusEnum { get; set; }

        public void setPaginationData<T>(PaginatedList<T> paginatedList) where T : class
        {
            PageIndex = paginatedList.PageIndex;
            TotalPages = paginatedList.TotalPages;
            TotalItems = paginatedList.TotalItems;
            PageSize = paginatedList.PageSize;

            if (PageIndex == 1 && TotalPages == 0 && TotalItems == 0)
                TotalPages = 1;
            if (PageIndex == 1 && TotalPages == 0 && TotalItems == 0)
                PageSize = 9;
        }
    }
}
