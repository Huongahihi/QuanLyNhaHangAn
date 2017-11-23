using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextureClassification.Model
{
    public class TextureClass
    {
        /// <summary>
        /// Class đại diện cho Tập các ảnh mẫu
        /// </summary>
        
        public string Name { get; set; } // Tên của lớp này

        private List<TextureModel> _models; // Danh sách các ảnh mẫu
        public List<TextureModel> Models // Biến này để public ra ngoài
        {
            get { return _models; }
            set { _models = value; }
        }

        public TextureClass(List<TextureModel> models)
        {
            _models = models;
            foreach (var item in models) item.ParentClass = this;
        }

        /// <summary>
        /// Hàm trích rút vector đặc trưng của mẫu.
        /// </summary>
        /// <returns></returns>
        public int[] GetBinCounters()
        {
            int[] totalCounter = new int[TextureModel.NbDimension];
            for (int i = 0; i < TextureModel.NbDimension; i++) totalCounter[i]++;
            foreach (var item in _models)
            {
                var counter = item.GetBinCounters();
                for (int i = 0; i < TextureModel.NbDimension; i++)
                    totalCounter[i] += counter[i];
            }
            return totalCounter;
        }

        /// <summary>
        /// Hàm tính xác suất xuất hiện của từng bin trong mẫu
        /// </summary>
        /// <returns></returns>
        public decimal[] GetProbabilityInClass()
        {
            int[] totalCounter = new int[TextureModel.NbDimension];
            int totalCounterValue = 0;
            for (int i = 0; i < TextureModel.NbDimension; i++) totalCounter[i] = 0;
            foreach (var item in _models)
            {
                var counter = item.GetBinCounters();
                for (int i = 0; i < TextureModel.NbDimension; i++)
                    totalCounter[i] += counter[i];

                totalCounterValue += item.TotalBinCounters;
            }

            decimal[] res = new decimal[TextureModel.NbDimension];
            for (int i = 0; i < TextureModel.NbDimension; i++)
                res[i] = (decimal) totalCounter[i] / totalCounterValue;
            return res;
        }

        /// <summary>
        /// Tính giá trị thống kê L xuất hiện của sample model trong lớp này.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public double L(TextureModel model)
        {
            var S = model.GetProbabilityInModel();
            var M = GetProbabilityInClass();
            double res = 0;
            for (int i = 0; i < TextureModel.NbDimension; i++)
            if (M[i] > 0)
            {
                res -= (double)S[i] * Math.Log((double)M[i]);
            }
            return res;
        }
    }
}
