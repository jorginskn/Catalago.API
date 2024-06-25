using APICatalago.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalago.DTOS
{
    public class ProductDTOUpdateResponse
    {

         public int ProductId { get; set; }
        public string? Name { get; set; }


        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public float Stock { get; set; }
        public DateTime CreateOn { get; set; }
        public int CategoryId { get; set; }
    }
}
