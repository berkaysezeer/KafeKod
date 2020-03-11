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

        public event EventHandler<MasaTasimaEventArgs> MasaTasiniyor;

        KafeContext db;
        Siparis siparis;

        public SiparisForm(KafeContext kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            InitializeComponent();
            dgvSiparisDetay.AutoGenerateColumns = false; //tum sutunlarin gelmesini engeller
            MasaNoYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler.ToList();
            dgvSiparisDetay.DataSource = siparis.SiparisDetaylar;
        }

        private void MasaNoYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= Properties.Settings.Default.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif)) //aktif siparislerde masa nosu i olan yoksa
                {
                    cboMasaNo.Items.Add(i);
                }
            }

            cboMasaNo.SelectedIndex = 0;
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.SiparisDetaylar
                .Sum(x => x.Adet * x.BirimFiyat).ToString("0.00") + "₺";
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
                UrunId = seciliUrun.Id,
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };

            siparis.SiparisDetaylar.Add(sd);
            db.SaveChanges();
            dgvSiparisDetay.DataSource = new BindingSource(siparis.SiparisDetaylar, null);
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
                db.SaveChanges();
                Close();
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
                siparis.OdenenTutar = siparis.SiparisDetaylar.Sum(x => x.Adet * x.BirimFiyat);
                db.SaveChanges();
                Close();

            }
        }

        private void dgvSiparisDetay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetay.HitTest(e.X, e.Y).RowIndex;
                if (rowIndex > -1)
                {
                    dgvSiparisDetay.ClearSelection();
                    dgvSiparisDetay.Rows[rowIndex].Selected = true;
                    cmsSiparisDetay.Show(Cursor.Position);
                }

            }
        }

        private void tsmiSiparisSil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetay.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvSiparisDetay.SelectedRows[0];
                var sipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                db.SiparisDetaylar.Remove(sipDetay);
                db.SaveChanges();
                dgvSiparisDetay.DataSource = new BindingSource(siparis.SiparisDetaylar, null);
            }

            TutarGuncelle();

        }

        private void btnMasTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir masa no seçiniz");
                return;
            }

            int eskiMasaNo = siparis.MasaNo;
            int hedefMasaNo = (int)cboMasaNo.SelectedItem;


            if (MasaTasiniyor != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    TasinanSiparis = siparis,
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo,
                };

                MasaTasiniyor(this, args);
            }

            siparis.MasaNo = hedefMasaNo;
            db.SaveChanges();
            MasaNoGuncelle();
            MasaNoYukle();
        }

        private void SiparisForm_Load(object sender, EventArgs e)
        {

        }
    }

    public class MasaTasimaEventArgs : EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int YeniMasaNo { get; set; }
        public int EskiMasaNo { get; set; }
    }
}
