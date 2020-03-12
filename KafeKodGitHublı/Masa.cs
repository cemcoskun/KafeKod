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

namespace KafeKodGitHublı
{
    public partial class Masa : Form
    {
        public event EventHandler<MasaTasimaEventArgs> MasaTasindi;
        KafeContext db;
        Siparis siparis;
        public Masa(KafeContext kafeveri, Siparis siparis)
        {
            db = kafeveri;
            this.siparis = siparis;
            InitializeComponent();
            dgvSiparisDetaylari.AutoGenerateColumns = false;
            MasaNolariYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler.Where(x => !x.StoktaYok).ToList();
            cboUrun.SelectedItem = null;
            dgvSiparisDetaylari.DataSource = siparis.SiparisDetaylar;
        }

        private void MasaNolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i < Properties.Settings.Default.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif))
                {
                    cboMasaNo.Items.Add(i);
                }
            }
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.SiparisDetaylar.Sum(x => x.Adet * x.BirimFiyat).ToString("0.00") + "tl";
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
                MessageBox.Show("Lütfen bir ürün seçiniz.");
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
            dgvSiparisDetaylari.DataSource = new BindingSource(siparis.SiparisDetaylar, null);
            cboUrun.SelectedIndex = 0;
            nudAdet.Value = 1;
            TutarGuncelle();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Sipariş iptal edilecektir. Onaylıyor musunuz?",
                "Sipariş İptal Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Iptal;
                siparis.KapanisZamani = DateTime.Now;
                db.SaveChanges();
                Close();
            }
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Ödeme alındıysa masanın hesabı kapatılacaktır. Onaylıyor musunuz?",
                "Masa Hesabı Kapatma Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Odendi;
                siparis.KapanisZamani = DateTime.Now;
                siparis.OdenenTutar = siparis.SiparisDetaylar.Sum(x => x.Adet * x.BirimFiyat);
                db.SaveChanges();
                Close();
            }
        }

        private void dgvSiparisDetaylari_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetaylari.HitTest(e.X, e.Y).RowIndex;
                if (rowIndex > -1)
                {
                    dgvSiparisDetaylari.ClearSelection();
                    dgvSiparisDetaylari.Rows[rowIndex].Selected = true;
                    MessageBox.Show(MousePosition.ToString());
                }
            }
        }

        private void tsmiSiparisDetaySil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetaylari.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvSiparisDetaylari.SelectedRows[0];
                var sipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                siparis.SiparisDetaylar.Remove(sipDetay);
                db.SaveChanges();
            }

            TutarGuncelle();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen hedef masa no seçiniz");
                return;
            }
            int eskiMasaNo = siparis.MasaNo;
            int hedefMasaNo = (int)cboMasaNo.SelectedItem;
            siparis.MasaNo = hedefMasaNo;
            MasaNoGuncelle();
            MasaNolariYukle();
            if (MasaTasindi != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    TasinanSiparis = siparis,
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo
                };
                MasaTasindi(this, args);
            }
            db.SaveChanges();
        }
    }
    public class MasaTasimaEventArgs : EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int EskiMasaNo { get; set; }
        public int YeniMasaNo { get; set; }
    }
}
