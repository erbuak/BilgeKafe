using BilgeKafe.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BilgeKafe.UI
{
    public partial class UrunlerForm : Form
    {
        private readonly KafeVeri db;
        private readonly BindingList<Urun> blUrunler;
        public UrunlerForm(KafeVeri db)
        {
            this.db = db;   
            InitializeComponent();
            blUrunler = new BindingList<Urun>(db.Urunler);
            dgvUrunler.AutoGenerateColumns = false;
            dgvUrunler.DataSource = blUrunler;
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAd.Text.Trim();
            if(urunAd == "")
            {
                MessageBox.Show("Lütfen ürün adı belirleyin.");
                return;
            }
           
            
            if(btnUrunEkle.Text == "Ekle")
            {
                blUrunler.Add(new Urun() { UrunAd = urunAd, BirimFiyat = nudBirimFiyat.Value });
            } 
            else
            {
                DataGridViewRow satir = dgvUrunler.SelectedRows[0];
                Urun urun = (Urun)satir.DataBoundItem;
                urun.UrunAd = txtUrunAd.Text;
                urun.BirimFiyat = nudBirimFiyat.Value;
                blUrunler.ResetBindings();
            }
            FormuResetle();
        }

        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            DataGridViewRow satir = dgvUrunler.SelectedRows[0];
            Urun urun = (Urun)satir.DataBoundItem;
            txtUrunAd.Text = urun.UrunAd;
            nudBirimFiyat.Value = urun.BirimFiyat;
            btnUrunEkle.Text = "Kaydet";
            dgvUrunler.Enabled = false;
            btnDuzenle.Enabled = false;
            btnIptal.Show();
            txtUrunAd.Focus();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            FormuResetle();
        }

        private void FormuResetle()
        {
            txtUrunAd.Text = "";
            nudBirimFiyat.Value = 0;
            btnUrunEkle.Text = "Ekle";
            dgvUrunler.Enabled = true;
            btnDuzenle.Enabled = true;
            btnIptal.Hide();
            txtUrunAd.Focus();
        }
    }
}
