using System.ComponentModel.DataAnnotations;

namespace FirstWebApp.Data
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }
    }
}
