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
using System.Reflection;

namespace TextureClassification
{
    public partial class NewForm : Form
    {
        #region Fields

        private List<TextureClass> _parternClass;
        private TextureModel[] _currentSample;
        private const double EPSILON = -1;

        #endregion

        #region Constructor

        public NewForm()
        {
            Icon = Properties.Resources.icon;
            InitializeComponent();
            InitDataGridView();
            lblResult.Text = string.Empty;
            //LoadSavedData();
        }

        private void LoadSavedData()
        {
            _parternClass = new List<TextureClass>();
            if (!Directory.Exists(ParternSavedData)) return;
            string[] parternFolders = Directory.GetDirectories(ParternSavedData);
            if (parternFolders.Length > 0)
            {
                InvokeThreadStart(() =>
                {
                    LSMGridView.Columns.Clear();
                    DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
                    nameColumn.Name = "ImageName";
                    nameColumn.HeaderText = "Ảnh \\ Mẫu";
                    nameColumn.ReadOnly = true;
                    nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    LSMGridView.Columns.Add(nameColumn);
                });

                foreach (var folderName in parternFolders)
                {
                    List<TextureModel> models = new List<TextureModel>();
                    List<Bitmap> bitmaps = new List<Bitmap>();
                    string[] fileNames = Directory.GetFiles(folderName);
                    foreach (var fileName in fileNames)
                    {
                        try
                        {
                            string text = System.IO.File.ReadAllText(fileName);
                            string[] items = text.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                            if (items.Length == 60)
                            {
                                string pathItem = items[0];
                                if (bitmaps.Count < NB_PARTERNS) bitmaps.Add(new Bitmap(pathItem));
                                TextureModel model = new TextureModel();
                                model.IsInitBinCounter = true;
                                model.BinCounters = new int[59];
                                model.PathFile = pathItem;
                                for (int i = 0; i < 59; i++)
                                {
                                    model.BinCounters[i] = int.Parse(items[i + 1]);
                                    model.TotalBinCounters += model.BinCounters[i];
                                }
                                models.Add(model);
                            }
                        }
                        catch { }
                    }

                    DirectoryInfo info = new DirectoryInfo(folderName);
                    var partern = new TextureClass(models);
                    partern.Name = info.Name;
                    _parternClass.Add(partern);

                    InvokeThreadStart(() =>
                    {
                        object[] parternRow = new object[NB_PARTERNS + 1];
                        parternRow[0] = info.Name;
                        for (int i = 0; i < NB_PARTERNS; i++)
                        {
                            if (bitmaps.Count <= i) parternRow[i + 1] = string.Empty;
                            else parternRow[i + 1] = bitmaps[i];
                        }
                        int id = dataGridView.Rows.Count;
                        dataGridView.Rows.Add(parternRow);
                        dataGridView.Rows[id].Height = 62;

                        decimal[] probablities = partern.GetProbabilityInClass();
                        object[] probablityRow = new object[probablities.Length + 1];
                        probablityRow[0] = info.Name;
                        for (int i = 0; i < probablities.Length; i++)
                        {
                            probablityRow[i + 1] = probablities[i].ToString("F5");
                        }
                        int count = gridViewProbablityTab1.Rows.Count;
                        gridViewProbablityTab1.Rows.Add(probablityRow);
                        gridViewProbablityTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();


                        DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
                        nameColumn.Name = "Name" + id.ToString();
                        nameColumn.HeaderText = info.Name;
                        nameColumn.ReadOnly = true;
                        nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        LSMGridView.Columns.Add(nameColumn);
                    });
                }
            }
        }

        #endregion

        #region GUI

        private const int NB_PARTERNS = 7;

        private void InitDataGridView()
        {
            #region Tab1

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

            DataGridViewTextBoxColumn firstColumn = new DataGridViewTextBoxColumn();
            firstColumn.Name = "ParternName";
            firstColumn.HeaderText = "Tên mẫu";
            firstColumn.ReadOnly = true;
            firstColumn.Width = 100;
            firstColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            gridViewProbablityTab1.Columns.Add(firstColumn);

            for (int i = 0; i < 59; i++)
            {
                DataGridViewTextBoxColumn indexColumn = new DataGridViewTextBoxColumn();
                indexColumn.Name = "Index" + i.ToString();
                indexColumn.HeaderText = i.ToString();
                indexColumn.ReadOnly = true;
                indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridViewProbablityTab1.Columns.Add(indexColumn);
            }

            DataGridViewTextBoxColumn firstTrichRutColumn = new DataGridViewTextBoxColumn();
            firstTrichRutColumn.Name = "ParternName";
            firstTrichRutColumn.HeaderText = "Tên mẫu";
            firstTrichRutColumn.ReadOnly = true;
            firstTrichRutColumn.Width = 100;
            firstTrichRutColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewTrichRutTab1.Columns.Add(firstTrichRutColumn);

            for (int i = 0; i < 59; i++)
            {
                DataGridViewTextBoxColumn indexColumn = new DataGridViewTextBoxColumn();
                indexColumn.Name = "Index" + i.ToString();
                indexColumn.HeaderText = i.ToString();
                indexColumn.ReadOnly = true;
                indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewTrichRutTab1.Columns.Add(indexColumn);
            }

            #endregion

            #region Tab3

            DataGridViewTextBoxColumn firstMau1Column = new DataGridViewTextBoxColumn();
            firstMau1Column.Name = "ParternName";
            firstMau1Column.HeaderText = "Tên file";
            firstMau1Column.ReadOnly = true;
            firstMau1Column.Width = 100;
            firstMau1Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMau1.Columns.Add(firstMau1Column);

            for (int i = 0; i < 59; i++)
            {
                DataGridViewTextBoxColumn indexColumn = new DataGridViewTextBoxColumn();
                indexColumn.Name = "Index" + i.ToString();
                indexColumn.HeaderText = i.ToString();
                indexColumn.ReadOnly = true;
                indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewMau1.Columns.Add(indexColumn);
            }

            DataGridViewTextBoxColumn firstMau2Column = new DataGridViewTextBoxColumn();
            firstMau2Column.Name = "ParternName";
            firstMau2Column.HeaderText = "Tên file";
            firstMau2Column.ReadOnly = true;
            firstMau2Column.Width = 100;
            firstMau2Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMau2.Columns.Add(firstMau2Column);

            for (int i = 0; i < 59; i++)
            {
                DataGridViewTextBoxColumn indexColumn = new DataGridViewTextBoxColumn();
                indexColumn.Name = "Index" + i.ToString();
                indexColumn.HeaderText = i.ToString();
                indexColumn.ReadOnly = true;
                indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewMau2.Columns.Add(indexColumn);
            }

            DataGridViewTextBoxColumn firstTestColumn = new DataGridViewTextBoxColumn();
            firstTestColumn.Name = "ParternName";
            firstTestColumn.HeaderText = "Tên file";
            firstTestColumn.ReadOnly = true;
            firstTestColumn.Width = 100;
            firstTestColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewTest.Columns.Add(firstTestColumn);

            for (int i = 0; i < 59; i++)
            {
                DataGridViewTextBoxColumn indexColumn = new DataGridViewTextBoxColumn();
                indexColumn.Name = "Index" + i.ToString();
                indexColumn.HeaderText = i.ToString();
                indexColumn.ReadOnly = true;
                indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewTest.Columns.Add(indexColumn);
            }

            DataGridViewTextBoxColumn firstXS1Column = new DataGridViewTextBoxColumn();
            firstXS1Column.Name = "ParternName";
            firstXS1Column.HeaderText = "Tên file";
            firstXS1Column.ReadOnly = true;
            firstXS1Column.Width = 100;
            firstXS1Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewXS1.Columns.Add(firstXS1Column);

            for (int i = 0; i < 59; i++)
            {
                DataGridViewTextBoxColumn indexColumn = new DataGridViewTextBoxColumn();
                indexColumn.Name = "Index" + i.ToString();
                indexColumn.HeaderText = i.ToString();
                indexColumn.ReadOnly = true;
                indexColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewXS1.Columns.Add(indexColumn);
            }

            #endregion
        }

        private void StartProgressbarTab1()
        {
            InvokeThreadStart(() =>
            {
                progressBarTab1.Visible = true;
                progressBarTab1.Style = ProgressBarStyle.Marquee;
                progressBarTab1.MarqueeAnimationSpeed = 30;
            });
        }

        private void StopProgressbarTab1()
        {
            InvokeThreadStart(() =>
            {
                progressBarTab1.Style = ProgressBarStyle.Continuous;
                progressBarTab1.MarqueeAnimationSpeed = 0;
                progressBarTab1.Visible = false;
            });
        }

        private void StartProgressbarTab2()
        {
            InvokeThreadStart(() =>
            {
                progressBarTab2.Visible = true;
                progressBarTab2.Style = ProgressBarStyle.Marquee;
                progressBarTab2.MarqueeAnimationSpeed = 30;
            });
        }

        private void StopProgressbarTab2()
        {
            InvokeThreadStart(() =>
            {
                progressBarTab2.Style = ProgressBarStyle.Continuous;
                progressBarTab2.MarqueeAnimationSpeed = 0;
                progressBarTab2.Visible = false;
            });
        }

        #endregion

        #region Event window form handler

        //private void btnRun_Click(object sender, EventArgs e)
        //{
        //    if (_currentSample == null)
        //    {
        //        MessageBox.Show("Bạn chưa chọn ảnh");
        //        return;
        //    }

        //    if (_parternClass == null || _parternClass.Count == 0)
        //    {
        //        MessageBox.Show("Bạn chưa chọn tập ảnh mẫu");
        //        return;
        //    }

        //    new Thread(() =>
        //    {
        //        StartProgressbarTab2();

        //        double minValue = double.MaxValue;
        //        double maxValue = minValue;
        //        string parternName = string.Empty;
        //        foreach (var partern in _parternClass)
        //        {
        //            double lValue = partern.L();
        //            if (minValue >= lValue)
        //            {
        //                maxValue = minValue;
        //                minValue = lValue;
        //                parternName = partern.Name;
        //            }
        //        }

        //        if (maxValue - minValue <= EPSILON) parternName = "Không có kết quả thỏa mãn";
        //        else parternName = string.Format("Kết quả : {0}, {1} : {2}", parternName, minValue.ToString("F2"), (maxValue - minValue).ToString("F2"));
        //        InvokeThreadStart(() =>
        //            {
        //                lblResult.Text = parternName;
        //            });

        //        StopProgressbarTab2();
        //    }).Start();

        //}
        /// <summary>
        /// Mơ tập ảnh test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenTest_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" + "All files (*.*)|*.*";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                new Thread(() =>   //Dòng chảy, luồng..
                {
                    StartProgressbarTab2();
                    _currentSample = new TextureModel[openFile.FileNames.Length];
                    InvokeThreadStart(() =>// gọi hiện lên xong clear.
                    {
                        dataGridViewTab2.Rows.Clear();
                    });
                    for (int i = 0; i < _currentSample.Length; i++)
                    {
                        string imageFile = openFile.FileNames[i];// đọc ảnh
                        Bitmap myBitmap = new Bitmap(imageFile);
                        _currentSample[i] = new TextureModel((Bitmap)myBitmap.Clone());
                        _currentSample[i].PathFile = imageFile;

                        InvokeThreadStart(() =>
                        {
                            object[] dataRow = new object[2];
                            dataRow[0] = GetFileName(imageFile);
                            dataRow[1] = myBitmap;
                            int id = dataGridViewTab2.Rows.Count;
                            dataGridViewTab2.Rows.Add(dataRow);
                            dataGridViewTab2.Rows[id].Height = 200;
                        });
                    }
                    StopProgressbarTab2();
                    InvokeThreadStart(() =>
                    {
                        lblResult.Text = string.Empty;
                    });
                }).Start();
            }
        }
        /// <summary>
        /// /Mở tập ảnh huấn luyện.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                if (_parternClass != null) _parternClass.Clear();
                else _parternClass = new List<TextureClass>();
                dataGridView.Rows.Clear();
                gridViewProbablityTab1.Rows.Clear();

                new Thread(() =>
                {
                    StartProgressbarTab1();

                    InvokeThreadStart(() =>
                    {
                        LSMGridView.Columns.Clear();
                        DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
                        nameColumn.Name = "ImageName";
                        nameColumn.HeaderText = "L(S,M)";
                        nameColumn.ReadOnly = true;
                        nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        LSMGridView.Columns.Add(nameColumn);
                    });

                    GC.Collect();  // tập hợp

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
                                model.PathFile = image;
                                models.Add(model);
                            }
                            catch { }
                        }

                        var partern = new TextureClass(models);
                        partern.Name = parternDirectory.Name;
                        _parternClass.Add(partern);

                        InvokeThreadStart(() =>
                        {
                            object[] parternRow = new object[NB_PARTERNS + 1];
                            parternRow[0] = parternDirectory.Name;
                            for (int i = 0; i < NB_PARTERNS; i++)
                            {
                                if (bitmaps.Count <= i) parternRow[i + 1] = string.Empty;
                                else parternRow[i + 1] = bitmaps[i];
                            }
                            int id = dataGridView.Rows.Count;
                            dataGridView.Rows.Add(parternRow);
                            dataGridView.Rows[id].Height = 62;

                            decimal[] probablities = partern.GetProbabilityInClass();
                            object[] probablityRow = new object[probablities.Length + 1];
                            probablityRow[0] = parternDirectory.Name;
                            for (int i = 0; i < probablities.Length; i++)
                            {
                                probablityRow[i + 1] = probablities[i].ToString("F5");
                            }
                            int count = gridViewProbablityTab1.Rows.Count;
                            gridViewProbablityTab1.Rows.Add(probablityRow);
                            gridViewProbablityTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();

                            int[] binCounters = partern.GetBinCounters();
                            object[] vectorRow = new object[binCounters.Length + 1];
                            vectorRow[0] = parternDirectory.Name;
                            for (int i = 0; i < binCounters.Length; i++)
                                vectorRow[i + 1] = binCounters[i].ToString();
                            count = dataGridViewTrichRutTab1.Rows.Count;
                            dataGridViewTrichRutTab1.Rows.Add(vectorRow);
                            dataGridViewTrichRutTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();

                            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
                            nameColumn.Name = "Name" + id.ToString();
                            nameColumn.HeaderText = parternDirectory.Name;
                            nameColumn.ReadOnly = true;
                            nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            LSMGridView.Columns.Add(nameColumn);
                        });
                    }
                    StopProgressbarTab1();
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

        private void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach(FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
        /// <summary>
        /// Lưu tập huấn luyện.
        /// </summary>
        private static string ParternSavedData = "ParternSavedVectorData";
        private void btnSavePartern_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file|*.txt";
            dialog.Title = "Save an text file";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dialog.FileName;
                new Thread(() =>
                {
                    StartProgressbarTab1();

                    using (System.IO.StreamWriter file =
                                new System.IO.StreamWriter(fileName))
                    {
                        file.WriteLine(_parternClass.Count);
                        foreach (var partern in _parternClass)
                        {
                            file.WriteLine(partern.Name);
                            file.WriteLine(partern.Models.Count);
                            foreach (var model in partern.Models)
                            {
                                file.WriteLine(model.Width + " " + model.Height);
                                for (int i = 0; i < model.Width; i++)
                                {
                                    for (int j = 0; j < model.Height; j++)
                                        file.Write(model.GrayLevel[i, j] + " ");
                                    file.WriteLine();
                                }
                            }
                        }
                    }

                    StopProgressbarTab1();
                    InvokeThreadStart(() =>
                    {
                        MessageBox.Show("Lưu thành công");
                    });
                }).Start();
            };
        }
        /// <summary>
        /// Mở mô hình huấn luyện đã lưu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenSavedFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                new Thread(() =>
                {
                    StartProgressbarTab1();
                    InvokeThreadStart(() =>
                    {
                        dataGridView.Rows.Clear();
                        gridViewProbablityTab1.Rows.Clear();
                        dataGridViewTrichRutTab1.Rows.Clear();
                    });
                    string fileName = dialog.FileName;
                    string line = string.Empty;
                    try
                    {
                        System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                        int nbPartern = int.Parse(file.ReadLine().Trim());
                        _parternClass = new List<TextureClass>();
                        for (int i = 0; i < nbPartern; i++)
                        {
                            string className = file.ReadLine().Trim();
                            List<TextureModel> models = new List<TextureModel>();
                            int nbModels = int.Parse(file.ReadLine().Trim());
                            for (int j = 0; j < nbModels; j++)
                            {
                                string[] items = file.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                int width = int.Parse(items[0].Trim()), height = int.Parse(items[1].Trim());
                                int[,] grayLevel = new int[width, height];
                                for (int k = 0; k < width; k++)
                                {
                                    items = file.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int k1 = 0; k1 < height; k1++)
                                        grayLevel[k, k1] = int.Parse(items[k1]);
                                }
                                TextureModel model = new TextureModel();
                                model.GrayLevel = grayLevel;
                                model.Width = width;
                                model.Height = height;
                                models.Add(model);
                            }
                            var myClass = new TextureClass(models);
                            myClass.Name = className;
                            _parternClass.Add(myClass);

                            InvokeThreadStart(() =>
                            {
                                int[] binCounters = myClass.GetBinCounters();
                                object[] vectorRow = new object[binCounters.Length + 1];
                                vectorRow[0] = className;
                                for (int i1 = 0; i1 < binCounters.Length; i1++)
                                    vectorRow[i1 + 1] = binCounters[i1];
                                int count = gridViewProbablityTab1.Rows.Count;
                                dataGridViewTrichRutTab1.Rows.Add(vectorRow);
                                dataGridViewTrichRutTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();

                                decimal[] probablities = myClass.GetProbabilityInClass();
                                object[] probablityRow = new object[probablities.Length + 1];
                                probablityRow[0] = className;
                                for (int i1 = 0; i1 < probablities.Length; i1++)
                                {
                                    probablityRow[i1 + 1] = probablities[i1].ToString("F5");
                                }
                                count = gridViewProbablityTab1.Rows.Count;
                                gridViewProbablityTab1.Rows.Add(probablityRow);
                                gridViewProbablityTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();


                                DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
                                nameColumn.Name = "Name" + i.ToString();
                                nameColumn.HeaderText = className;
                                nameColumn.ReadOnly = true;
                                nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                LSMGridView.Columns.Add(nameColumn);
                            });
                        }
                        StopProgressbarTab1();
                        InvokeThreadStart(() =>
                        {
                            MessageBox.Show(string.Format("Load thành công {0} mẫu", _parternClass.Count));
                        });
                    }
                    catch (Exception ex)
                    {
                        StopProgressbarTab1();
                        InvokeThreadStart(() =>
                        {
                            MessageBox.Show(string.Format("Load dữ liệu bị lỗi : {0} : {1}", ex.Message, ex.StackTrace));
                        });
                    }
                }).Start();
            }
        }
        private static string GetFileName(string pathFile)
        {
            int endIndex = pathFile.Length;
            int startIndex = 0;
            for (int i = endIndex - 1; i >= 0; i--)
            {
                if (pathFile[i] == '\\' || pathFile[i] == '/')
                {
                    startIndex = i + 1;
                    break;
                }
                if (pathFile[i] == '.') endIndex = i;
            }

            return pathFile.Substring(startIndex, endIndex - startIndex);
        }

        private static string GetParentFileName(string pathFile)
        {
            int endIndex = -1;
            int startIndex = -1;
            for (int i = pathFile.Length-1; i >= 0; i--)
            {
                if (pathFile[i] == '\\' || pathFile[i] == '/')
                {
                    if (endIndex == -1) endIndex = i;
                    else
                    {
                        startIndex = i + 1;
                        break;
                    }
                }
            }

            return pathFile.Substring(startIndex, endIndex - startIndex);
        }
        /// <summary>
        /// Xác định mẫu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_parternClass == null || _parternClass.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn tập huấn luyện");
                return;
            }

            if (_currentSample == null || _currentSample.Length == 0)
            {
                MessageBox.Show("Bạn chưa chọn tập test");
                return;
            }

            new Thread(() =>
            {
                StartProgressbarTab2();

                InvokeThreadStart(() =>
                {
                    LSMGridView.Rows.Clear();
                    dataGridViewResult.Rows.Clear();
                });

                int totalExactlyItem = 0;
                foreach (var sample in _currentSample)
                {
                    double minValue = double.MaxValue;
                    double maxValue = minValue;
                    string parternName = string.Empty;
                    string sampleName = GetFileName(sample.PathFile);
                    string parentFileName = GetParentFileName(sample.PathFile);
                    object[] LSMRow = new object[_parternClass.Count + 1];
                    object[] resultRow = new object[2];
                    LSMRow[0] = resultRow[0] = sampleName;
                    int id = 0;
                    foreach (var partern in _parternClass)
                    {
                        double lValue = partern.L(sample);
                        LSMRow[++id] = lValue.ToString("F5");
                        if (minValue >= lValue)
                        {
                            maxValue = minValue;
                            minValue = lValue;
                            parternName = partern.Name;
                        }
                    }

                    if (maxValue - minValue <= EPSILON) parternName = "Không có kết quả thỏa mãn";
                    resultRow[1] = parternName;
                    if (parentFileName.Equals(parternName)) totalExactlyItem++;

                    InvokeThreadStart(() =>
                    {
                        LSMGridView.Rows.Add(LSMRow);
                        dataGridViewResult.Rows.Add(resultRow);
                    });
                }
                if (_currentSample.Length > 0)
                {
                    object[] resultRow = new object[2];
                    resultRow[0] = "Độ chính xác";
                    resultRow[1] = (((decimal)totalExactlyItem / _currentSample.Length) * 100).ToString("F4") + "%";
                    InvokeThreadStart(() =>
                    {
                        dataGridViewResult.Rows.Add(resultRow);
                    });
                }
                StopProgressbarTab2();
            }).Start();
        }

        private bool ReadMaxtrixFromFile(string pathFile, ref int[,] matrix, ref int width, ref int height)
        {
            string text = System.IO.File.ReadAllText(pathFile);
            string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int rows = lines.Length, columns = 0;
            foreach (var line in lines)
            {
                string[] items = line.Split(new char[] { ' ', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (columns == 0) columns = items.Length;
                else if (columns != items.Length) return false;
            }

            matrix = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                string[] items = lines[i].Split(new char[] { ' ', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < columns; j++)
                {
                    if (!int.TryParse(items[j], out matrix[i, j])) return false;
                }
            }

            width = rows;
            height = columns;
            return true;
        }
        /// <summary>
        /// Thêm mẫu ma trận huấn luyện.
        /// </summary>
        private TextureClass _parternTest1;
        private TextureClass _parternTest2;
        private List<TextureModel> _sampleTest;

        private void btnAddMau1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataGridViewMau1.Rows.Clear();
                string[] files = openFileDialog.FileNames;
                List<TextureModel> models = new List<TextureModel>();
                foreach (var file in files)
                {
                    int[,] matrix = null;
                    int width = 0, height = 0;
                    if (!ReadMaxtrixFromFile(file, ref matrix, ref width, ref height))
                    {
                        MessageBox.Show(string.Format("Dữ liệu file {0} không đúng", file));
                        return;
                    }

                    TextureModel model = new TextureModel();
                    model.GrayLevel = matrix;
                    model.Width = width;
                    model.Height = height;
                    model.PathFile = file;
                    models.Add(model);

                    object[] row = new object[60];
                    row[0] = GetFileName(file);
                    for (int i = 1; i < 60; i++) row[i] = string.Empty;
                    int count = dataGridViewMau1.Rows.Count;
                    dataGridViewMau1.Rows.Add(row);
                    dataGridViewMau1.Rows[count].Tag = model;
                }
                _parternTest1 = new TextureClass(models);
            }
        }

        private void btnAddMau2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataGridViewMau2.Rows.Clear();
                string[] files = openFileDialog.FileNames;
                List<TextureModel> models = new List<TextureModel>();
                foreach (var file in files)
                {
                    int[,] matrix = null;
                    int width = 0, height = 0;
                    if (!ReadMaxtrixFromFile(file, ref matrix, ref width, ref height))
                    {
                        MessageBox.Show(string.Format("Dữ liệu file {0} không đúng", file));
                        return;
                    }

                    TextureModel model = new TextureModel();
                    model.GrayLevel = matrix;
                    model.Width = width;
                    model.Height = height;
                    model.PathFile = file;
                    models.Add(model);

                    object[] row = new object[60];
                    row[0] = GetFileName(file);
                    for (int i = 1; i < 60; i++) row[i] = string.Empty;
                    int count = dataGridViewMau2.Rows.Count;
                    dataGridViewMau2.Rows.Add(row);
                    dataGridViewMau2.Rows[count].Tag = model;
                }
                _parternTest2 = new TextureClass(models);
            }
        }

        private void btnAddTest_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataGridViewTest.Rows.Clear();
                string[] files = openFileDialog.FileNames;
                _sampleTest = new List<TextureModel>();
                foreach (var file in files)
                {
                    int[,] matrix = null;
                    int width = 0, height = 0;
                    if (!ReadMaxtrixFromFile(file, ref matrix, ref width, ref height))
                    {
                        MessageBox.Show(string.Format("Dữ liệu file {0} không đúng", file));
                        return;
                    }

                    TextureModel model = new TextureModel();
                    model.GrayLevel = matrix;
                    model.Width = width;
                    model.Height = height;
                    model.PathFile = file;
                    _sampleTest.Add(model);

                    object[] row = new object[60];
                    row[0] = GetFileName(file);
                    var binCounters = model.GetBinCounters();
                    for (int i = 1; i < 60; i++) row[i] = binCounters[i - 1];
                    int count = dataGridViewTest.Rows.Count;
                    dataGridViewTest.Rows.Add(row);
                    dataGridViewTest.Rows[count].Tag = model;
                }
            }
        }
        /// <summary>
        /// Trích rút đặc trưng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrepairTest_Click(object sender, EventArgs e)
        {
            if (_parternTest1 == null || _parternTest1.Models.Count == 0)
            {
                MessageBox.Show("Dữ liệu mẫu 1 chưa có");
                return;
            }

            if (_parternTest2 == null || _parternTest2.Models.Count == 0)
            {
                MessageBox.Show("Dữ liệu mẫu 2 chưa có");
                return;
            }

            foreach (DataGridViewRow row in dataGridViewMau1.Rows)
            {
                TextureModel model = (TextureModel)row.Tag;
                object[] rowData = new object[60];
                rowData[0] = GetFileName(model.PathFile);
                int[] binCounters = model.GetBinCounters();
                for (int i = 0; i < 59; i++) rowData[i + 1] = binCounters[i];
                row.SetValues(rowData);
            }

            foreach (DataGridViewRow row in dataGridViewMau2.Rows)
            {
                TextureModel model = (TextureModel)row.Tag;
                object[] rowData = new object[60];
                rowData[0] = GetFileName(model.PathFile);
                int[] binCounters = model.GetBinCounters();
                for (int i = 0; i < 59; i++) rowData[i + 1] = binCounters[i];
                row.SetValues(rowData);
            }
        }
        /// <summary>
        /// Huấn luyện mẫu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTrainingTest_Click(object sender, EventArgs e)
        {
            if (_parternTest1 == null || _parternTest1.Models.Count == 0)
            {
                MessageBox.Show("Dữ liệu mẫu 1 chưa có");
                return;
            }

            if (_parternTest2 == null || _parternTest2.Models.Count == 0)
            {
                MessageBox.Show("Dữ liệu mẫu 2 chưa có");
                return;
            }

            dataGridViewXS1.Rows.Clear();

            decimal[] probablities = _parternTest1.GetProbabilityInClass();
            object[] probablityRow = new object[probablities.Length + 1];
            probablityRow[0] = "Mẫu 1";
            for (int i = 0; i < probablities.Length; i++)
            {
                probablityRow[i + 1] = probablities[i].ToString("F5");
            }
            int count = dataGridViewXS1.Rows.Count;
            dataGridViewXS1.Rows.Add(probablityRow);
            dataGridViewXS1.Rows[count].HeaderCell.Value = (count + 1).ToString();

            probablities = _parternTest2.GetProbabilityInClass();
            probablityRow = new object[probablities.Length + 1];
            probablityRow[0] = "Mẫu 2";
            for (int i = 0; i < probablities.Length; i++)
            {
                probablityRow[i + 1] = probablities[i].ToString("F5");
            }
            count = dataGridViewXS1.Rows.Count;
            dataGridViewXS1.Rows.Add(probablityRow);
            dataGridViewXS1.Rows[count].HeaderCell.Value = (count + 1).ToString();
        }
        /// <summary>
        /// Xác định mẫu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyTest_Click(object sender, EventArgs e)
        {
            if (_parternTest1 == null || _parternTest1.Models.Count == 0)
            {
                MessageBox.Show("Dữ liệu mẫu 1 chưa có");
                return;
            }

            if (_parternTest2 == null || _parternTest2.Models.Count == 0)
            {
                MessageBox.Show("Dữ liệu mẫu 2 chưa có");
                return;
            }

            if (_sampleTest == null || _sampleTest.Count == 0)
            {
                MessageBox.Show("Tập test chưa có");
                return;
            }

            dataGridViewLSMTab3.Rows.Clear();
            dataGridViewResultTab3.Rows.Clear();
            foreach (DataGridViewRow row in dataGridViewTest.Rows)
            {
                TextureModel sample = (TextureModel) row.Tag;
                string name = GetFileName(sample.PathFile);

                double value1 = _parternTest1.L(sample);
                double value2 = _parternTest2.L(sample);
                string parternName = "Mẫu 1";
                if (value1 > value2) parternName = "Mẫu 2";

                object[] rowData = new object[3];
                rowData[0] = name;
                rowData[1] = value1.ToString("F5");
                rowData[2] = value2.ToString("F5");
                dataGridViewLSMTab3.Rows.Add(rowData);

                object[] resultData = new object[2];
                resultData[0] = name;
                resultData[1] = parternName;
                dataGridViewResultTab3.Rows.Add(resultData);
            }
        }
        /// <summary>
        /// Mở tập test đã lưu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenSavedFileTab2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                new Thread(() =>
                {
                    StartProgressbarTab2();
                    InvokeThreadStart(() =>
                    {
                        dataGridView.Rows.Clear();
                        gridViewProbablityTab1.Rows.Clear();
                        dataGridViewTrichRutTab1.Rows.Clear();
                    });
                    string fileName = dialog.FileName;
                    string line = string.Empty;
                    try
                    {
                        System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                        int nbPartern = int.Parse(file.ReadLine().Trim());
                        _parternClass = new List<TextureClass>();
                        for (int i = 0; i < nbPartern; i++)
                        {
                            string className = file.ReadLine().Trim();
                            List<TextureModel> models = new List<TextureModel>();
                            int nbModels = int.Parse(file.ReadLine().Trim());
                            for (int j = 0; j < nbModels; j++)
                            {
                                string[] items = file.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                int width = int.Parse(items[0].Trim()), height = int.Parse(items[1].Trim());
                                int[,] grayLevel = new int[width, height];
                                for (int k = 0; k < width; k++)
                                {
                                    items = file.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int k1 = 0; k1 < height; k1++)
                                        grayLevel[k, k1] = int.Parse(items[k1]);
                                }
                                TextureModel model = new TextureModel();
                                model.GrayLevel = grayLevel;
                                model.Width = width;
                                model.Height = height;
                                models.Add(model);
                            }
                            var myClass = new TextureClass(models);
                            myClass.Name = className;
                            _parternClass.Add(myClass);

                            InvokeThreadStart(() =>
                            {
                                int[] binCounters = myClass.GetBinCounters();
                                object[] vectorRow = new object[binCounters.Length + 1];
                                vectorRow[0] = className;
                                for (int i1 = 0; i1 < binCounters.Length; i1++)
                                    vectorRow[i1 + 1] = binCounters[i1];
                                int count = gridViewProbablityTab1.Rows.Count;
                                dataGridViewTrichRutTab1.Rows.Add(vectorRow);
                                dataGridViewTrichRutTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();

                                decimal[] probablities = myClass.GetProbabilityInClass();
                                object[] probablityRow = new object[probablities.Length + 1];
                                probablityRow[0] = className;
                                for (int i1 = 0; i1 < probablities.Length; i1++)
                                {
                                    probablityRow[i1 + 1] = probablities[i1].ToString("F5");
                                }
                                count = gridViewProbablityTab1.Rows.Count;
                                gridViewProbablityTab1.Rows.Add(probablityRow);
                                gridViewProbablityTab1.Rows[count].HeaderCell.Value = (count + 1).ToString();


                                DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
                                nameColumn.Name = "Name" + i.ToString();
                                nameColumn.HeaderText = className;
                                nameColumn.ReadOnly = true;
                                nameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                LSMGridView.Columns.Add(nameColumn);
                            });
                        }
                        StopProgressbarTab2();
                        InvokeThreadStart(() =>
                        {
                            MessageBox.Show(string.Format("Load thành công {0} mẫu", _parternClass.Count));
                        });
                    }
                    catch (Exception ex)
                    {
                        StopProgressbarTab1();
                        InvokeThreadStart(() =>
                        {
                            MessageBox.Show(string.Format("Load dữ liệu bị lỗi : {0} : {1}", ex.Message, ex.StackTrace));
                        });
                    }
                }).Start();
            }
        }
    }
}
