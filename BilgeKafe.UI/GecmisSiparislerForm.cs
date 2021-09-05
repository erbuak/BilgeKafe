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
    public partial class GecmisSiparislerForm : Form
    {
        private readonly List<Siparis> germisSiparisler;

        public GecmisSiparislerForm(List<Siparis> gecmisSiparisler)
        {
            this.germisSiparisler = gecmisSiparisler;
            InitializeComponent();
            dgvSiparisler.AutoGenerateColumns = false;
            dgvSiparisDetaylar.AutoGenerateColumns = false;
            dgvSiparisler.DataSource = gecmisSiparisler;           
        }

        private void dgvSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvSiparisler.SelectedRows.Count != 1)
            {
                dgvSiparisDetaylar.DataSource = null;
            } 
            else
            {
                DataGridViewRow satir = dgvSiparisler.SelectedRows[0];
                Siparis siparis = (Siparis)satir.DataBoundItem;
                dgvSiparisDetaylar.DataSource = siparis.SiparisDetaylar;
            }
        }
    }
}
