using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalago.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO PRODUCTS(Name, Description, Price, ImageUrl, Stock, CreateOn, CategoryId) " + "VALUES ('Coca-cola-diet', 'Refrigerante de cola 350ml', 5.45,  'coca-cola.jpg', 50, GETDATE(), 1)");
            migrationBuilder.Sql("INSERT INTO PRODUCTS(Name, Description, Price, ImageUrl, Stock, CreateOn, CategoryId) " + "VALUES ('Lanche de atum', 'Lanche de atum com maionese', 8.50,  'atum.jpg', 10,GETDATE(), 2)");
            migrationBuilder.Sql("INSERT INTO PRODUCTS(Name, Description, Price, ImageUrl, Stock, CreateOn, CategoryId) " + "VALUES ('Pudim', 'Pudim  de leite condesado', 6.75,  'pudim.jpg', 20, GETDATE(), 3)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from Products");
        }
    }
}
