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
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasindiEventArgs> MasaTasindi;
        private readonly KafeVeri db;
        private readonly Siparis siparis;
        private readonly BindingList<SiparisDetay> blSiparisDetaylar;

        public SiparisForm(KafeVeri db, Siparis siparis)
        {
            this.db = db;
            this.siparis = siparis;
            blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            blSiparisDetaylar.ListChanged += BlSiparisDetaylar_ListChanged;
            InitializeComponent();
            dgvSiparisDetaylari.AutoGenerateColumns = false;
            dgvSiparisDetaylari.DataSource = blSiparisDetaylar;
            dgvSiparisDetaylari.CellEndEdit += DgvSiparisDetaylari_CellEndEdit;

            UrunleriListele();
            MasaNolariGuncelle();
            MasaNolariListele();
            OdemeTutariniGuncelle();
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = db.Urunler;
        }
        private void MasaNolariListele()
        {
            for (int i = 1; i < db.MasaAdet; i++)
            {
                if (!db.AktifSiparisler.Any(x => x.MasaNo == i))
                {
                    cboMasaNo.Items.Add(i);
                }
            }
        }

        private void MasaNolariGuncelle()
        {
            this.Text = $"Masa {siparis.MasaNo:00}";
            lblMasaNo.Text = $"{siparis.MasaNo:00}";
        }

        private void btnDetayEkle_Click(object sender, EventArgs e)
        {
            Urun urun = (Urun)cboUrun.SelectedItem;
            int adet = (int)nudAdet.Value;
            SiparisDetay siparisDetay;

            if (urun == null)
            {
                MessageBox.Show("Önce bir ürün seçin");
                return;
            }
            siparisDetay = siparis.SiparisDetaylar.SingleOrDefault(x => x.UrunAd == urun.UrunAd);

            if (siparisDetay != null)
            {
                siparisDetay.Adet += adet;
                blSiparisDetaylar.ResetBindings();
            } 
            else
            {
                siparisDetay = new SiparisDetay()
                {
                    UrunAd = urun.UrunAd,
                    BirimFiyat = urun.BirimFiyat,
                    Adet = adet
                };
                blSiparisDetaylar.Add(siparisDetay);
            }
        }

        private void BlSiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            OdemeTutariniGuncelle();
        }

        private void DgvSiparisDetaylari_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            OdemeTutariniGuncelle();
            blSiparisDetaylar.ResetBindings();
        }

        private void OdemeTutariniGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show($"{siparis.ToplamTutarTL} tahsil edildiyse sipariş kapanacaktır. Onaylıyor musunuz?", "Ödeme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dr == DialogResult.Yes)
            {
                SiparisKapat(SiparisDurum.Odendi);
            }
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show($"Sipariş iptal edilecektir. Onaylıyor musunuz?", "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                SiparisKapat(SiparisDurum.Iptal);
            }
        }

        private void SiparisKapat(SiparisDurum durum)
        {
            siparis.Durum = durum;
            if (durum == SiparisDurum.Odendi)
            {
                siparis.OdenenTutar = siparis.ToplamTutar();
            }
            else
            {
                siparis.OdenenTutar = 0;
            }
            siparis.KapanisZamani = DateTime.Now;
            db.AktifSiparisler.Remove(siparis);
            db.GecmisSiparisler.Add(siparis);
            Close();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            int eskiMasaNo = siparis.MasaNo;
            int yeniMasaNo = (int)cboMasaNo.SelectedItem;
            siparis.MasaNo = yeniMasaNo;
            MasaNolariGuncelle();
            MasaNolariListele();

            if(MasaTasindi != null)
            {
                MasaTasindiEventArgs args = new MasaTasindiEventArgs() { EskiMasaNo = eskiMasaNo, YeniMasaNo = yeniMasaNo };
                MasaTasindi(this, args);
            }
        }
    }
}
