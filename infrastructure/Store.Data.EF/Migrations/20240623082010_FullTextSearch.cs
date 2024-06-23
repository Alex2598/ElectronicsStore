using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class FullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG StoreFullTextCatalog AS DEFAULT", suppressTransaction: true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Components(Package, NameOfComponent) KEY INDEX PK_Components WITH STOPLIST = SYSTEM", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Components", suppressTransaction: true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG StoreFullTextCatalog", suppressTransaction: true);
        }
    }
}
