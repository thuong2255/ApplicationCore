using System;
using System.Collections.Generic;
using System.Text;

namespace SystemCore.Service.ViewModels.Product
{
    public class ProductQuantityViewModel
    {

        public int ProductId { get; set; }

        public int SizeId { get; set; }


        public int ColorId { get; set; }

        public int Quantity { get; set; }

        public virtual ProductViewModel Product { get; set; }

        public virtual SizeViewModel Size { get; set; }

        public virtual ColorViewModel Color { get; set; }
    }
}
