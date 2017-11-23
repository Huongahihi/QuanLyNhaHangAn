using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextureClassification.Model;
using System.Threading;
using System.IO;

namespace TextureClassification
{
    public partial class MainForm : Form
    {
        #region Fields

        private List<TextureClass> _parternClass;
        private TextureModel _currentSample;
        private const double EPSILON = -1;

        #endregion

        #region Constructor

        public MainForm()
        {
            Icon = Properties.Resources.icon;
            InitializeComponent();
            InitDataGridView();
            lblResult.Text = string.Empty;
            _parternClass = new List<TextureClass>();
        }

        #endregion

        #region GUI

        private const int NB_PARTERNS = 10;

        private void InitDataGridView()
        {
            dataGridView.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridView.RowsDefaultCellStyle.SelectionForeColor = System.Drawing.Color.Transparent;


            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "ParternName";
            nameColumn.HeaderText = "Tên mẫu";
            nameColumn.ReadOnly = true;
            nameColumn.Width = 100;
            nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns.Add(nameColumn);

            for (int i = 1; i <= NB_PARTERNS; i++)
            {
                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                imageColumn.Name = "Image" + i.ToString();
                imageColumn.HeaderText = "Mẫu " + i.ToString();
                imageColumn.Width = 62;
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
                dataGridView.Columns.Add(imageColumn);
            }
        }

        private void StartProgressbar()
        {
            InvokeThreadStart(() =>
            {
                waitingProgressBar.Visible = true;
                waitingProgressBar.Style = ProgressBarStyle.Marquee;
                waitingProgressBar.MarqueeAnimationSpeed = 30;
            });
        }

        private void StopProgressbar()
        {
            InvokeThreadStart(() =>
            {
                waitingProgressBar.Style = ProgressBarStyle.Continuous;
                waitingProgressBar.MarqueeAnimationSpeed = 0;
                waitingProgressBar.Visible = false;
            });
        }

        #endregion

        #region Event window form handler

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (_currentSample == null)
            {
                MessageBox.Show("Bạn chưa chọn ảnh");
                return;
            }

            if (_parternClass == null || _parternClass.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn tập ảnh mẫu");
                return;
            }

            new Thread(() =>
            {
                StartProgressbar();

                double minValue = double.MaxValue;
                double maxValue = minValue;
                string parternName = string.Empty;
                foreach (var partern in _parternClass)
                {
                    double lValue = partern.L(_currentSample);
                    if (minValue >= lValue)
                    {
                        maxValue = minValue;
                        minValue = lValue;
                        parternName = partern.Name;
                    }
                }

                if (maxValue - minValue <= EPSILON) parternName = "Không có kết quả thỏa mãn";
                else parternName = string.Format("Kết quả : {0}, {1} : {2}", parternName, minValue.ToString("F2"), (maxValue - minValue).ToString("F2"));
                InvokeThreadStart(() =>
                    {
                        lblResult.Text = parternName;
                    });

                StopProgressbar();
            }).Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" + "All files (*.*)|*.*";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                new Thread(() =>
                {
                    StartProgressbar();
                    string imageFile = openFile.FileName;
                    Image myImage = Image.FromFile(imageFile);
                    originalImage.Image = myImage;
                    _currentSample = new TextureModel(new Bitmap(imageFile));
                    Image _lbpImage = _currentSample.GetUniformLBPImage();
                    lbpImage.Image = _lbpImage;
                    StopProgressbar();
                    InvokeThreadStart(() =>
                    {
                        lblResult.Text = string.Empty;
                    });
                }).Start();
            }
        }

        private void btnLoadPartern_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] parternFolders = Directory.GetDirectories(fbd.SelectedPath);
                if (parternFolders.Length == 0)
                {
                    MessageBox.Show("Không tìm thấy mẫu nào.");
                    return;
                }

                new Thread(() =>
                {
                    StartProgressbar();

                    _parternClass.Clear();
                    dataGridView.Rows.Clear();
                    GC.Collect();

                    foreach (var parternFolderName in parternFolders)
                    {
                        var parternDirectory = new DirectoryInfo(parternFolderName);
                        string[] fileNames = Directory.GetFiles(parternFolderName);
                        List<TextureModel> models = new List<TextureModel>();
                        List<Bitmap> bitmaps = new List<Bitmap>();
                        foreach (var image in fileNames)
                        {
                            try
                            {
                                Bitmap imageBitmap = new Bitmap(image);
                                if (bitmaps.Count < NB_PARTERNS) bitmaps.Add((Bitmap)imageBitmap.Clone());
                                TextureModel model = new TextureModel(imageBitmap);
                                models.Add(model);
                            }
                            catch { }
                        }

                        InvokeThreadStart(() =>
                        {
                            object[] dataRow = new object[NB_PARTERNS + 1];
                            dataRow[0] = parternDirectory.Name;
                            for (int i = 0; i < NB_PARTERNS; i++)
                            {
                                if (bitmaps.Count <= i) dataRow[i + 1] = string.Empty;
                                else dataRow[i + 1] = bitmaps[i];
                            }
                            int id = dataGridView.Rows.Count;
                            dataGridView.Rows.Add(dataRow);
                            dataGridView.Rows[id].Height = 62;
                        });

                        var partern = new TextureClass(models);
                        partern.Name = parternDirectory.Name;
                        _parternClass.Add(partern);
                    }
                    StopProgressbar();
                    InvokeThreadStart(() =>
                    {
                        lblResult.Text = string.Empty;
                    });
                }).Start();
            }
        }

        private void InvokeThreadStart(ThreadStart ts)
        {
            if (InvokeRequired)
            {
                try { Invoke(ts); }
                catch { }
            }
            else
                ts.Invoke();
        }

        #endregion
    }
}
