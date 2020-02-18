using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KafeKod.Data;


namespace KafeKod
{
    public partial class Form1 : Form
    {
        KafeVeri db;
        int masaAdet = 20;


        public Form1()
        {
            db = new KafeVeri();
            OrnekVerileriYukle();
            InitializeComponent();
            MasalarOlustur();
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun>()
            {
                new Urun{UrunAd="Kola", BirimFiyat=6.99m},
                new Urun{UrunAd="Su", BirimFiyat=2.00m},
            };
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
            for (int masaNo = 1; masaNo <= masaAdet; masaNo++)
            {

                lvi = new ListViewItem("Masa " + masaNo);
                lvi.Tag = masaNo;
                lvi.ImageKey = "bos";
                lvwMasalar.Items.Add(lvi);
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

        private void tsmiGecmisSiparis_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparisForm(db);
            frm.ShowDialog();
        }
    }
}
