using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KafeKod.Data;
using Newtonsoft.Json;

namespace KafeKod
{
    public partial class Form1 : Form
    {
        KafeVeri db;

        public Form1()
        {
            VerileriOku();

            InitializeComponent();
            MasalarOlustur();
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
                db = new KafeVeri();
            }
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun>()
            {
                new Urun{UrunAd="Kola", BirimFiyat=6.99m},
                new Urun{UrunAd="Su", BirimFiyat=2.00m},
                new Urun{UrunAd="Ayvalık Tostu", BirimFiyat=12.00m},
            };

            db.Urunler.Sort();
        }

        private void MasalarOlustur()
        {
            #region Resimlerin Hazırlanması
            ImageList images = new ImageList();
            images.Images.Add("bos", Properties.Resources.masabos);
            images.Images.Add("dolu", Properties.Resources.masadolu);
            images.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = images;
            #endregion


            ListViewItem lvi;
            for (int masaNo = 1; masaNo <= db.MasaAdet; masaNo++)
            {

                lvi = new ListViewItem("Masa " + masaNo);
                //i masa no degeriyle kayitli bir siparis var mi

                Siparis sip = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo); //??
                if (sip == null)
                {
                    lvi.Tag = masaNo;
                    lvi.ImageKey = "bos";
                }
                else
                {
                    lvi.Tag = sip;
                    lvi.ImageKey = "dolu";
                }

                lvwMasalar.Items.Add(lvi);
                //foreach (Siparis x db.AktifSiparisler)
                //{
                //    if (x.MasaNo == masaNo)
                //    {
                //        sip = x;
                //        break;
                //    }
                //}



            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var lvi = lvwMasalar.SelectedItems[0];
                lvi.ImageKey = "dolu";

                Siparis sip;
                //masa doluysa olanini al, bossa yeni olustur
                if (lvi.Tag is Siparis)
                {
                    sip = (Siparis)lvi.Tag;
                }
                else
                {
                    sip = new Siparis();
                    sip.MasaNo = (int)lvi.Tag;
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip;
                    db.AktifSiparisler.Add(sip);
                }

                SiparisForm frmSiparis = new SiparisForm(db, sip);
                frmSiparis.MasaTasindi += FrmSiparis_MasaTasindi;
                frmSiparis.ShowDialog();




                if (sip.Durum != SiparisDurum.Aktif)
                {
                    lvi.Tag = sip.MasaNo;
                    lvi.ImageKey = "bos";
                    db.AktifSiparisler.Remove(sip);
                    db.GecmisSiparisler.Add(sip);
                }
            }
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasimaEventArgs e)
        {
            //adim1: eski masayi bosalt
            ListViewItem lviEskiMasa = null;
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag == e.TasinanSiparis)
                {
                    lviEskiMasa = item;
                    break;
                }

                lviEskiMasa.Tag = e.EskiMasaNo;
                lviEskiMasa.ImageKey = "bos";
            }
            //adim2: yeni masaya siparisi koy
        }

        private void tsmiGecmisSiparis_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparisForm(db);
            frm.ShowDialog();
        }

        private void tsmiUrunler_Click_1(object sender, EventArgs e)
        {
            var frm = new UrunlerForm(db);
            frm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string json = JsonConvert.SerializeObject(db);
            File.WriteAllText("veri.json", json);
        }
    }
}
