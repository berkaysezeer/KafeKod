namespace KafeKod.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UrunSiparisDetayCascadeDeleteOff : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SiparisDetaylari", "UrunId", "dbo.Urunler");
            AddForeignKey("dbo.SiparisDetaylari", "UrunId", "dbo.Urunler", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiparisDetaylari", "UrunId", "dbo.Urunler");
            AddForeignKey("dbo.SiparisDetaylari", "UrunId", "dbo.Urunler", "Id", cascadeDelete: true);
        }
    }
}
