using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace TextureClassification.Model
{
    /// <summary>
    /// Class đại diện cho 1 ảnh mẫu, lớp này sẽ dùng P = 8, R = 1.
    /// </summary>
    public class TextureModel
    {
        #region Constructor

        private TextureClass _parentClass; // Nếu nằm trong tập mẫu thì parentClass khác null, chỉ dùng để trỏ vào tập ảnh mẫu cho nhanh
        public TextureClass ParentClass
        {
            get { return _parentClass; }
            set { _parentClass = value; }
        }
        private int[,] _grayLevel; // Mảng 2 chiều kích cỡ [width, height], đưa ra giá trị grayLevel tại mỗi pixel
        public int[,] GrayLevel
        {
            get { return _grayLevel; }
            set { _grayLevel = value; }
        }
        private Bitmap _myBitmap; // Bitmap hiện tại.
        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private bool _isInitBinCounter = false;
        public bool IsInitBinCounter
        {
            get { return _isInitBinCounter; }
            set { _isInitBinCounter = value; }
        }
        private int[] _binCounters;
        public int[] BinCounters
        {
            get { return _binCounters; }
            set { _binCounters = value; }
        }
        private bool _isInitProbability = false;
        private decimal[] _probability;

        // Biến này dùng để tính cho xác suất, in ra tổng số pixel trong ảnh có giá trị LBP nằm trong tập uniform.
        private int _totalBinCounters = 0; 

        public int TotalBinCounters
        {
            get { return _totalBinCounters; }
            set { _totalBinCounters = value; }
        }
        public string PathFile { get; set; }

        #region Static variables

        /// <summary>
        /// Tổng số bin dùng để thống kê trong ảnh.
        /// Nếu dùng theo uniform sẽ có 59 bin
        /// Dùng theo minROR sẽ có 36 bin
        /// Thực nghiệm thì cách uniform có vẻ tốt hơn.
        /// </summary>
        private static int _nbDimension = 59;
        public static int NbDimension
        {
            get { return TextureModel._nbDimension; }
            set { TextureModel._nbDimension = value; }
        }
        private static int[] _lbpBinTable = InitLbpBinTable();
        public static int[] LbpBinTable
        {
            get { return _lbpBinTable; }
            set { _lbpBinTable = value; }
        }

        #endregion

        public TextureModel()
        {
            _parentClass = null;
        }

        public TextureModel(Bitmap image)
        {
            _myBitmap = image;
            _width = image.Width;
            _height = image.Height;
            ConvertToGrayScale(image);
            GetProbabilityInModel();
            _parentClass = null;
        }

        public TextureModel(Bitmap image, TextureClass parentClass)
        {
            _myBitmap = image;
            _width = image.Width;
            _height = image.Height;
            ConvertToGrayScale(image);
            GetProbabilityInModel();
            _parentClass = parentClass;
        }

        #endregion

        #region Image helper

        /// <summary>
        /// Chuyển đổi ảnh ban đầu sang ảnh grayscale và khởi tạo mảng _grayLevel
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public unsafe Bitmap ConvertToGrayScale(Bitmap image)
        {
            BitmapData imageData = image.LockBits(new Rectangle(new Point(0, 0), image.Size)
                                        , ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Chuyển đổi không gian ảnh
            // (RGB) --> Y of (Y, Cb, Cr)
            byte* pData = (byte*)imageData.Scan0;
            _grayLevel = new int[image.Width, image.Height];

            for (int y = 0; y < imageData.Height; y++)
            {
                for (int x = 0; x < imageData.Width; x++)
                {
                    int grayLevel = (int)(0.11 * pData[3 * x]
                                    + 0.59 * pData[3 * x + 1]
                                    + 0.3 * pData[3 * x + 2]);  // Theo thứ tự BGR

                    pData[3 * x] = pData[3 * x + 1] = pData[3 * x + 2] = (byte)grayLevel;
                    _grayLevel[x, y] = (int)grayLevel;
                }
                pData += imageData.Stride;
            }

            // Nhị phân
            pData = (byte*)imageData.Scan0;
            for (int y = 0; y < imageData.Height; y++)
            {
                for (int x = 0; x < imageData.Width; x++)
                {
                    pData[3 * x] = pData[3 * x + 1] = pData[3 * x + 2] = (byte)(pData[3 * x]);
                }
                pData += imageData.Stride;
            }

            image.UnlockBits(imageData);

            return image;
        }

        /// <summary>
        /// Các hàm in ra ảnh này dùng để test
        /// </summary>
        /// <returns></returns>
        public Bitmap GetLBPImage()
        {
            Bitmap lbpImage = (Bitmap)_myBitmap.Clone();

            var lbp = GetLBPArray();

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1) lbp[x, y] = 0;
                    Color pixelColor = lbpImage.GetPixel(x, y);
                    Color c = Color.FromArgb(pixelColor.A, lbp[x, y], lbp[x, y], lbp[x, y]);
                    lbpImage.SetPixel(x, y, c);
                }


            return lbpImage;
        }

        public Bitmap GetUniformLBPImage()
        {
            Bitmap lbpImage = (Bitmap)_myBitmap.Clone();

            int[,] lbp = GetUniformLBPArray();

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1) lbp[x, y] = 0;
                    Color pixelColor = lbpImage.GetPixel(x, y);
                    Color c = Color.FromArgb(pixelColor.A, lbp[x, y], lbp[x, y], lbp[x, y]);
                    lbpImage.SetPixel(x, y, c);
                }

            return lbpImage;
        }

        #endregion

        #region LBP functions

        /// <summary>
        /// Đếm số lần chuyển bit từ 0->1 hoặc từ 1->0 của 1 số.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int NumberOfTransitions(int value)
        {
            int[] a = new int[8];
	        int cnt = 0;
	        int k = 7;
            while (k >= 0)
	        {
                a[k] = value % 2;
                value = value / 2;
		        --k;
	        }
	        for(k = 0; k < 7; k++)
	        {
                cnt += (a[k] != a[k + 1]) ? 1 : 0;
	        }
            cnt += (a[0] != a[7]) ? 1 : 0;
	        return cnt;
        }

        /// <summary>
        /// In ra giá trị nhỏ nhất sau khi quay vòng lần lượt các bit của giá trị value.
        /// Hàm này chỉ test chứ không dùng.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int minRotation(int value)
        {
            int[] a = new int[8];
            int k = 7;
            int oldValue = value;

            while (k >= 0)
            {
                a[k] = value % 2;
                value = value / 2;
                --k;
            }

            int res = oldValue;
            for (int i = 1; i < 8; i++)
            {
                int rotationValue = 0;
                for (int j = i; j < 8; j++)
                    rotationValue = rotationValue * 2 + a[j];
                for (int j = 0; j < i; j++)
                    rotationValue = rotationValue * 2 + a[j];
                if (res > rotationValue) res = rotationValue;
            }

            return res;
        }

        /// <summary>
        /// Đánh dấu các giá trị từ 0 -> 255 có tính chất uniform
        /// </summary>
        /// <returns></returns>
        private static int[] InitLbpBinTable()
        {
            var res = new int[256];

            _nbDimension = 0;
            for (int i = 0; i < 256; i++)
            {
                if (NumberOfTransitions(i) <= 2)
                {
                    res[i] = _nbDimension++;
                }
            }
            for (int i = 0; i < 256; i++)
                if (NumberOfTransitions(i) > 2)
                    res[i] = _nbDimension;
            _nbDimension++;
            //for (int i = 0; i < 256; i++)
            //{
            //    if (i == minRotation(i))
            //    {
            //        res[i] = _nbDimension++;
            //    }
            //    else res[i] = res[minRotation(i)];
            //}

            //_nbDimension = 256;
            //for (int i = 0; i < 256; i++) res[i] = i;
            return res;
        }

        private int[,] GetLBPArray()
        {
            int[,] lbp = new int[_width, _height];
            for (int x = 1; x < _width - 1; x++)
                for (int y = 1; y < _height - 1; y++)
                {
                    int[] neighbors = new int[8];
                    neighbors[0] = _grayLevel[x - 1, y - 1];
                    neighbors[1] = _grayLevel[x - 1, y];
                    neighbors[2] = _grayLevel[x - 1, y + 1];
                    neighbors[3] = _grayLevel[x, y + 1];
                    neighbors[4] = _grayLevel[x + 1, y + 1];
                    neighbors[5] = _grayLevel[x + 1, y];
                    neighbors[6] = _grayLevel[x + 1, y - 1];
                    neighbors[7] = _grayLevel[x, y - 1];
                    int center = _grayLevel[x, y];
                    int lbpValue = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        lbpValue += ((neighbors[i] >= center) ? 1 : 0) * (1 << i);
                    }
                    lbp[x, y] = lbpValue;
                }
            return lbp;
        }

        /// <summary>
        /// Với mỗi pixel, nếu LBP không nằm trong tập uniform thì gán lại = 0.
        /// </summary>
        /// <returns></returns>
        private int[,] GetUniformLBPArray()
        {
            int[,] lbp = new int[_width, _height];
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    int[] neighbors = new int[8];
                    neighbors[0] = GetPixelValue(x - 1, y - 1); //_grayLevel[x - 1, y - 1];
                    neighbors[1] = GetPixelValue(x - 1, y); //_grayLevel[x - 1, y];
                    neighbors[2] = GetPixelValue(x - 1, y + 1); //_grayLevel[x - 1, y + 1];
                    neighbors[3] = GetPixelValue(x, y + 1); //_grayLevel[x, y + 1];
                    neighbors[4] = GetPixelValue(x + 1, y + 1); //_grayLevel[x + 1, y + 1];
                    neighbors[5] = GetPixelValue(x + 1, y); //_grayLevel[x + 1, y];
                    neighbors[6] = GetPixelValue(x + 1, y - 1); //_grayLevel[x + 1, y - 1];
                    neighbors[7] = GetPixelValue(x, y - 1); //_grayLevel[x, y - 1];
                    int center = _grayLevel[x, y];
                    int lbpValue = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        lbpValue += ((neighbors[i] >= center) ? 1 : 0) * (1 << i);
                    }
                    lbp[x, y] = _lbpBinTable[lbpValue];
                }
            return lbp;
        }

        private int GetPixelValue(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height) return 0;
            return _grayLevel[x, y];
        }

        /// <summary>
        /// In ra mảng thống kê P : P[i] là số lần xuất hiện LBP = i trong tập ảnh.
        /// </summary>
        /// <returns></returns>
        public int[] GetBinCounters()
        {
            if (_isInitBinCounter) return _binCounters;
            else
            {
                _isInitBinCounter = true;
                int[] counter = new int[_nbDimension];
                for (int i = 0; i < _nbDimension; i++) counter[i] = 0;
                int[,] lbpArr = GetUniformLBPArray();

                _totalBinCounters = 0;
                for (int x = 0; x < _width; x++)
                    for (int y = 0; y < _height; y++)
                    //if (lbpArr[x, y] > 0)
                    {
                        counter[lbpArr[x, y]]++;
                        _totalBinCounters++;
                    }
                _binCounters = counter;
                return counter;
            }
        }

        /// <summary>
        /// In ra mảng thống kê xác suất P : P[i] là xác suất xuất hiện LBP = i trong tập ảnh
        /// </summary>
        /// <returns></returns>
        public decimal[] GetProbabilityInModel()
        {
            if (!_isInitProbability) // Biến này để đánh dấu chỉ tính 1 lần, nhằm tăng hiệu quả thời gian xử lý.
            {
                _isInitProbability = true;
                _probability = new decimal[_nbDimension];
                GetBinCounters();
                for (int i = 0; i < _nbDimension; i++)
                {
                    _probability[i] = (decimal) _binCounters[i] / _totalBinCounters;
                }
            }
            return _probability;
        }

        #endregion
    }
}
