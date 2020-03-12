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
        KafeContext db = new KafeContext();

        public Form1()
        {
            InitializeComponent();
            MasalarOlustur();
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

            lvwMasalar.Items.Clear();

            ListViewItem lvi;
            for (int masaNo = 1; masaNo <= Properties.Settings.Default.MasaAdet; masaNo++)
            {

                lvi = new ListViewItem("Masa " + masaNo);

                //i masa no degeriyle kayitli bir siparis ve siparis durumu aktifse 
                Siparis sip = db.Siparisler
                    .FirstOrDefault(x => x.MasaNo == masaNo && x.Durum == SiparisDurum.Aktif); //
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
                    sip.Durum = SiparisDurum.Aktif;
                    sip.MasaNo = (int)lvi.Tag;
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip;
                    db.Siparisler.Add(sip); //durum aktif
                    db.SaveChanges();
                }

                SiparisForm frmSiparis = new SiparisForm(db, sip);
                frmSiparis.MasaTasiniyor += FrmSiparis_MasaTasiniyor;
                frmSiparis.ShowDialog();

                if (sip.Durum != SiparisDurum.Aktif) //masa kapandiysa
                {
                    lvi.Tag = sip.MasaNo;
                    lvi.ImageKey = "bos";
                }
            }
        }

        private void FrmSiparis_MasaTasiniyor(object sender, MasaTasimaEventArgs e)
        {
            //adim1: eski masayi bosalt
            ListViewItem lviEskiMasa = MasaBul(e.EskiMasaNo);
            lviEskiMasa.Tag = e.EskiMasaNo;
            lviEskiMasa.ImageKey = "bos";
            //adim2: yeni masaya siparisi koy

            ListViewItem lviYeniMasa = MasaBul(e.YeniMasaNo);
            lviYeniMasa.Tag = e.TasinanSiparis;
            lviYeniMasa.ImageKey = "dolu";
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
            db.Dispose(); //baglantiyi kestik
        }

        private ListViewItem MasaBul(int masaNo)
        {
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag is int && (int)item.Tag == masaNo)
                {
                    return item;
                }
                else if (item.Tag is Siparis && ((Siparis)item.Tag).MasaNo == masaNo)
                    return item;
            }
            return null;
        }

        private void tsmiAyarlar_Click(object sender, EventArgs e)
        {
            var frm = new AyarlarForm();
            DialogResult dr = frm.ShowDialog();

            if (dr == DialogResult.OK)
            {
                MasalarOlustur();
            }
        }
    }
}
