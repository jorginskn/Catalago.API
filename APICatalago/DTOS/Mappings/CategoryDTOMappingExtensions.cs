using APICatalago.Models;

namespace APICatalago.DTOS.Mappings
{
    public static class CategoryDTOMappingExtensions
    {

        public static CategoryDTO? ToCategoryDTO(this Category category)
        {
            if (category is null)
            {
                return null;
            }

            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl
            };
        }

        public static Category? toCategory(this CategoryDTO categoryDTO)
        {
            if (categoryDTO is null)
            {
                return null;
            }

            return new Category()
            {
                CategoryId = categoryDTO.CategoryId,
                Name = categoryDTO.Name,
                ImageUrl = categoryDTO.ImageUrl
            };
        }
        public static IEnumerable<CategoryDTO> toCategorytDTOList(this IEnumerable<Category> categories)
        {
            if (categories is null || !categories.Any())
            {
                return new List<CategoryDTO>();
            }
            return categories.Select(categories => new CategoryDTO
            {
                CategoryId = categories.CategoryId,
                Name = categories.Name,
                ImageUrl = categories.ImageUrl
            }).ToList();
        }
    }
}
