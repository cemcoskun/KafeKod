using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKodGitHublı
{
    public partial class Form1 : Form
    {
        //int masaAdet = 20;
        //KafeVeri db;
        public Form1()
        {
            InitializeComponent();
        }
        /*private void MasalariOlustur()
        {
            #region Resim Listesinin Tanımlanması
            ImageList resimListesi = new ImageList();
            resimListesi.Images.Add("bos", Resources.masabos);
            resimListesi.Images.Add("dolu", Resources.masadolu);
            resimListesi.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = resimListesi;
            #endregion

            #region Masaların Oluşturulması
            int masaNo;
            ListViewItem lvi;
            for (int i = 0; i < masaAdet; i++)
            {
                masaNo = i + 1;
                lvi = new ListViewItem("Masa " + masaNo);
                lvwMasalar.Items.Add(lvi);
                lvi.ImageKey = db.MasaDoluMu(masaNo) ? "dolu" : "bos";
                lvi.Tag = masaNo;
            }
            #endregion
        }*/
    }
}
