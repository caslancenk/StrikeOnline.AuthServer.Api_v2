using Swashbuckle.AspNetCore.Annotations;


namespace AuthServer.Core.Dtos
{
    public class ProductDto
    {
        //[SwaggerSchema(ReadOnly = true)] // Swagger'da gösterilmez, ama JSON'da bulunur
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
        public int Stock { get; set; }
        public string UserId { get; set; }
    }
}
