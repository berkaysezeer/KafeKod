﻿using KafeKod.Data;
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
    public partial class GecmisSiparisForm : Form
    {
        KafeContext db;

        public GecmisSiparisForm(KafeContext kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();

            dgvSiparisler.DataSource = db.Siparisler.Where(x => x.Durum != SiparisDurum.Aktif).ToList();
        }

        private void dgvSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSiparisler.SelectedRows.Count > 0)
            {
                DataGridViewRow satir = dgvSiparisler.SelectedRows[0];
                Siparis siparis = (Siparis)satir.DataBoundItem;
                dgvSiparisDetay.DataSource = siparis.SiparisDetaylar;


            }
        }

        private void GecmisSiparisForm_Load(object sender, EventArgs e)
        {

        }
    }
}
