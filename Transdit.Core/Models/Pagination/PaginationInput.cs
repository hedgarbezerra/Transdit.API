using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Pagination
{
    public class PaginationInput
    {
        public PaginationInput()
        {
        }
        public PaginationInput(int index, int size)
        {
            Index = index;
            Size = size;
        }

        public PaginationInput(int index, int size, string searchTerm)
        {
            Index = index;
            Size = size;
            SearchTerm = searchTerm;
        }

        public int Index { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
    }
}
