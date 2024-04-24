using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalago.Migrations
{
     public partial class PopulaCategorias : Migration
    {
         protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Insert into Categories(Name, ImageUrl) VALUES('Bebidas','bebidas.jpg' )");
            migrationBuilder.Sql("Insert into Categories(Name, ImageUrl) VALUES('Lanches','Lanches.jpg' )");
            migrationBuilder.Sql("Insert into Categories(Name, ImageUrl) VALUES('Sobremesas','Sobremesas.jpg' )");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from categories");
        }
    }
}
