using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
    public class KafeContext : DbContext
    {
        public KafeContext() : base("name=baglanti")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Urun>().ToTable("Urunler"); //tablo ismini burada verdik

            modelBuilder.Entity<Urun>() //elimizde urun var 
                .HasMany(x => x.SiparisDetay) //bi siaprisdetay icinden birden fazla urun bulunur. Buradan silmemeiz gerekir
                .WithRequired(x => x.Urun) //urunu zorunlu yapiyoruz
                .HasForeignKey(x => x.UrunId) //yabanci anahtarini ayarlar
                .WillCascadeOnDelete(false); //default true -> false
            //bu kod sayesinde urunler silinir fakat diger bulundugu kisimlarda kalmaya devam eder
        }

        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylar { get; set; }


    }
}
