using System.ComponentModel.DataAnnotations;

namespace APICatalago.DTOS
{
    public class ProductDTOUpdateRequest: IValidatableObject
    {
        [Range(1, 9999, ErrorMessage ="Estoque deve está entre 1 e 9999")]
        public float Stock { get; set; }

        public DateTime CreateOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CreateOn <= DateTime.Now.Date)
                yield return new ValidationResult("A data de criação do produto não pode ser menor ou igual a data atual", new[] { nameof(this.CreateOn) });
        }
    }
}
