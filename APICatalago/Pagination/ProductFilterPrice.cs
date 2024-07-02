namespace APICatalago.Pagination
{
    public class ProductFilterPrice : QueryStringParameters
    {
        public decimal? Price { get; set; }
        public string? PriceCriterio { get; set; }

    }
}
