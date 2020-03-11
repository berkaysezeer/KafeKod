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
        BindingList<SiparisDetay> blSiparisDetay;

        public SiparisForm(KafeContext kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            blSiparisDetay = new BindingList<SiparisDetay>(siparis.SiparisDetaylar); //datagridviewi tetikler
            InitializeComponent();
            MasaNoYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler;
            dgvSiparisDetay.DataSource = blSiparisDetay;
        }

        private void MasaNoYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                if (!db.AktifSiparisler.Any(x => x.MasaNo == i)) //aktif siparislerde masa nosu i olan yoksa
                {
                    cboMasaNo.Items.Add(i);
                }
            }

            cboMasaNo.SelectedIndex = 0;
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

        private void dgvSiparisDetay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetay.HitTest(e.X, e.Y).RowIndex;
                //MessageBox.Show(rowIndex.ToString());
                if (rowIndex > -1)
                {
                    dgvSiparisDetay.ClearSelection();
                    dgvSiparisDetay.Rows[rowIndex].Selected = true;
                    cmsSiparisDetay.Show(Cursor.Position);
                    //cmsSiparisDetay.Show(MousePosition);
                }

            }
        }

        private void tsmiSiparisSil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetay.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvSiparisDetay.SelectedRows[0];
                var sipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                blSiparisDetay.Remove(sipDetay);
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
            MasaNoGuncelle();
            MasaNoYukle();
        }
    }

    public class MasaTasimaEventArgs : EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int YeniMasaNo { get; set; }
        public int EskiMasaNo { get; set; }
    }
}
