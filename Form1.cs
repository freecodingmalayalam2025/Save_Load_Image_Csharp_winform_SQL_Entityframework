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

namespace WindowsFormsApp_Load_Save_image
{
    public partial class Form1 : Form
    {
        string FileName;
        FreeCodingDBEntities db=new FreeCodingDBEntities();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog() { Multiselect=false,Filter="jpeg|*.jpg"})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileName = ofd.FileName;
                    lblfilename.Text = FileName;
                    pictureBox1.Image = Image.FromFile(FileName);
                }
            }
        }

        byte[] ConvertImageToBinary(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        private void btnupload_Click(object sender, EventArgs e)
        {
            using(db=new FreeCodingDBEntities())
            {
                FileData _filedata=new FileData ();
                _filedata.FileName = FileName;
                _filedata.Photo = ConvertImageToBinary(pictureBox1.Image);
                db.FileDatas.Add(_filedata);
                db.SaveChanges();
                MessageBox.Show("Photo uploaded successfully");
                Clear();
            }
        }

        void LoadData()
        {
            using (db = new FreeCodingDBEntities())
            {
                var filelist=db.FileDatas.ToList(); 
                
                dgv.DataSource = filelist;

                DataGridViewImageColumn imgCol =
                 (DataGridViewImageColumn)dgv.Columns["Photo"];

                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;


                dgv.RowTemplate.Height = 80;


                imgCol.Width = 100;

            }
        }
        void Clear()
        {
           // pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
            lblfilename.Text=string.Empty;
            LoadData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            if (dgv.CurrentRow.Index >= 0)
            {
                byte[] _imgdata = (byte[]) dgv.CurrentRow.Cells["Photo"].Value;
                pictureBox1.Image = ConvertBinarytoImage(_imgdata);
            }
        }


        Image ConvertBinarytoImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

    }
}
