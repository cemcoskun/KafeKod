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

namespace KafeKodGitHublı
{
    public partial class Form1 : Form
    {
        KafeContext db = new KafeContext();
        public Form1()
        {
            //db = new KafeVeri();
            //OrnekVerileriYukle();
            InitializeComponent();
            MasalariOlustur();
        }
        
        private void MasalariOlustur()
        {
            #region ListView Imajlarının Hazırlanması
            ImageList il = new ImageList();
            il.Images.Add("bos", Properties.Resources.masabos);
            il.Images.Add("dolu", Properties.Resources.masadolu);
            il.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = il;
            #endregion
            lvwMasalar.Items.Clear();
            ListViewItem lvi;
            for (int i = 1; i <= Properties.Settings.Default.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i);
                Siparis sip = db.Siparisler.FirstOrDefault(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif);
                if (sip == null)
                {
                    lvi.Tag = i;
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

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var lvi = lvwMasalar.SelectedItems[0];
                lvi.ImageKey = "dolu";
                Siparis sip;
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
                    db.Siparisler.Add(sip);
                    db.SaveChanges();
                }
                Masa frmSiparis = new Masa(db, sip);
                frmSiparis.ShowDialog();
                if (sip.Durum != SiparisDurum.Aktif)
                {
                    lvi.Tag = sip.MasaNo;
                    lvi.ImageKey = "bos";
                }
            }
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparisler(db);
            frm.ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            var frm = new Urunler(db);
            frm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Dispose();
        }

        private void tsmiAyarlar_Click(object sender, EventArgs e)
        {
            var frm = new AyarlarForm();
            DialogResult dr = frm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                MasalariOlustur();
            }
        }
    }
}
