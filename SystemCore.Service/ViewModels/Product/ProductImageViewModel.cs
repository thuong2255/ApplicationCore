namespace SystemCore.Service.ViewModels.Product
{
    public class ProductImageViewModel
    {
        public int ProductId { get; set; }

        public ProductViewModel Product { get; set; }

        public string Path { get; set; }

        public string Caption { get; set; }
    }
}