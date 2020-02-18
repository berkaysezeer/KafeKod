using KafeKod.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class SiparisForm : Form
    {
        KafeVeri db;
        Siparis siparis;
        BindingList<SiparisDetay> blSiparisDetay;

        public SiparisForm(KafeVeri kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            blSiparisDetay = new BindingList<SiparisDetay>(siparis.SiparisDetaylar); //datagridviewi tetikler
            InitializeComponent();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler;
            dgvSiparisDetay.DataSource = blSiparisDetay;
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.ToplamTutarTl;
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo;
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz");
                return;
            }

            Urun seciliUrun = (Urun)cboUrun.SelectedItem;

            var sd = new SiparisDetay
            {
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };

            blSiparisDetay.Add(sd);
            nudAdet.Value = 1;
            cboUrun.SelectedIndex = 0;
            TutarGuncelle();

        }

        private void btnAnasayfa_Click(object sender, EventArgs e) => Close();

        private void btnSpairsIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Sipariş iptal edilecektir",
                "Sipariş İptal Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                Close();
                siparis.KapanisZamani = DateTime.Now;
                siparis.Durum = SiparisDurum.Iptal;
            }
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Masa kapatılıyor. Onaylıyor musun?",
                "Ödeme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.KapanisZamani = DateTime.Now;
                siparis.Durum = SiparisDurum.Odendi;
                siparis.OdenenTutar = siparis.ToplamTutar();
                Close();

            }
        }
    }
}
