using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.Aruco;

namespace OpenCV
{
    public partial class Form3 : Form
    {
        VideoCapture m_CaptureWebCam = null;
        public Form3()
        {
            InitializeComponent();
            m_CaptureWebCam = new VideoCapture();
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cameraClosing);
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
                Dictionary arucoDict = new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_50);
                Dictionary arucoDict_eq = new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_50);
                VectorOfVectorOfPointF corners = new VectorOfVectorOfPointF(); // corners of the detected marker
                VectorOfVectorOfPointF corners_eq = new VectorOfVectorOfPointF(); // corners of the detected marker
                VectorOfVectorOfPointF rejected = new VectorOfVectorOfPointF(); // rejected contours
                VectorOfVectorOfPointF rejected_eq = new VectorOfVectorOfPointF(); // rejected contours
                VectorOfInt arucoIds = new VectorOfInt();
                VectorOfInt arucoIds_eq = new VectorOfInt();
                DetectorParameters arucoParameters = DetectorParameters.GetDefault();
                Image<Gray, byte> frame = m_CaptureWebCam.QueryFrame().ToImage<Gray, byte>();
                Image<Gray, byte> arucoDet = frame.Copy();
                Image<Gray, byte> frame_eq = null;
                ImageEnhancementAlgo.globalLuminanceAdapt(ref frame, out frame_eq);
                ImageEnhancementAlgo.CLAHE(ref frame_eq, out frame_eq);
                // CvInvoke.CLAHE(frame_eq, 2.0, new Size(new Point(8, 8)), frame_eq);
                Image<Gray, byte> arucoDet_eq = frame_eq.Copy();
                ArucoInvoke.DetectMarkers(arucoDet, arucoDict, corners, arucoIds, arucoParameters, rejected);
                ArucoInvoke.DrawDetectedMarkers(arucoDet, corners, arucoIds, new MCvScalar(255));
                ArucoInvoke.DetectMarkers(frame_eq, arucoDict_eq, corners_eq, arucoIds_eq, arucoParameters, rejected_eq);
                ArucoInvoke.DrawDetectedMarkers(arucoDet_eq, corners_eq, arucoIds_eq, new MCvScalar(255));
                Image<Gray, byte> arucoDet_cat = arucoDet.ConcateHorizontal(arucoDet_eq);
                // arucoDet_cat = arucoDet_cat.Resize(0.75, Inter.Area);
                pictureBox1.Image = BitmapExtension.ToBitmap<Gray, Byte>(arucoDet_cat);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                // pictureBox1.Update();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(ProcessWebCamFrame);
        }
    }
}
