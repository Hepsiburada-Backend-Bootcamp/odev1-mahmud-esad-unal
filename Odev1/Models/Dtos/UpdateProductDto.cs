using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Odev1
{
    public class UpdateProductDto
    {
        public ProductCategoryEnum Category { get; set; }
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "Product Name should be minimum 3 characters and a maximum of 50 characters")]
        public string Name { get; set; }
        [Range(0, 1000000,
            ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public float Price { get; set; }
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Product Vendor should be minimum 3 characters and a maximum of 50 characters")]
        public string Vendor { get; set; }

    }
}
