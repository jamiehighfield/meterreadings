using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterReadings.Shared
{
    public class PageRequest
    {
        public PageRequest(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; }

        public int PageSize { get; }

        public bool OrderAscending { get; }
    }
}