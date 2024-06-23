using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Data
{
    public class ComponentDto
    {
        public int Id { get; set; }

        public string UId { get; set; }

        public string Package { get; set; }

        public string NameOfComponent { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
