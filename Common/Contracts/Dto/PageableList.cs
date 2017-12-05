using System;
using System.Collections.Generic;
using System.Text;

namespace Threesixty.Common.Contracts.Dto
{
    public class PageableList<T>
    {
        public IEnumerable<T> Rows { get; set; }
        public int TotalCount { get; set; }
    }
}
