using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;

namespace OpenCV
{
    public partial class Form4 : Form
    {
        VideoCapture m_CaptureWebCam = null;
        Matrix<float>[] gaborFilters = null;
        const int numGaborFilters = 32;
        public Form4()
        {
            InitializeComponent();
            m_CaptureWebCam = new VideoCapture();
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cameraClosing);
            gaborFilters = new Matrix<float>[numGaborFilters];
            int iFilters = 0;
            for (double theta=0; theta<Math.PI; theta+=Math.PI/(double)numGaborFilters)
            {
                Mat kernel = CvInvoke.GetGaborKernel(new Size(new Point(31, 31)), 4.0, theta, 10.0, 0.5, 0, ktype: DepthType.Cv32F);
                Matrix<float> kernelF = new Matrix<float>(kernel.Rows, kernel.Cols);
                kernel.CopyTo(kernelF);
                double sum = kernelF.Sum;
                kernelF = kernelF / (1.5 * sum);
                gaborFilters[iFilters++] = kernelF;
            }
            if (numGaborFilters != iFilters)
                throw new ArgumentException("numGaborFilters != iFilters");
        }
        private void cameraClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_CaptureWebCam != null)
            {
                m_CaptureWebCam.Dispose();
                m_CaptureWebCam = null;
            }
        }
        private void ProcessWebCamFrame(object sender, EventArgs e)
        {
            if (m_CaptureWebCam != null)
            {
                Image<Bgr, byte> frame = m_CaptureWebCam.QueryFrame().ToImage<Bgr, byte>();
                Image<Bgr, float> frame_float = frame.Convert<Bgr, float>();
                Image<Bgr, float> canvas = frame_float.CopyBlank();
                for (int i=0; i<numGaborFilters; ++i)
                {
                    Image<Bgr, float> fimg = frame_float.CopyBlank();
                    CvInvoke.Filter2D(frame_float, fimg, gaborFilters[i], new Point(-1,-1));
                    canvas = canvas.Max(fimg);
                }
                canvas = canvas.ThresholdTrunc(new Bgr(255, 255, 255));
                Image<Bgr, byte> output = canvas.Convert<Bgr, byte>();
                pictureBox1.Image = BitmapExtension.ToBitmap<Bgr, byte>(output);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(ProcessWebCamFrame);
        }
    }
}
