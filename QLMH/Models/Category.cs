using System.ComponentModel.DataAnnotations;

namespace QLMH.Models
{
    public class Category
    {
        public Category()
        {
           
            Products = new List<Product>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [Display(Name = "Tên loại")]
        public string Name { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; } 
    }
}
