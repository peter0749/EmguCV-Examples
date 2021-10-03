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
using Emgu.CV.Features2D;
using Emgu.CV.XFeatures2D;

namespace OpenCV
{
    public partial class Form2 : Form
    {
        VideoCapture m_CaptureWebCam = null;
        private Image<Bgr, byte> templateImage = null;
        MKeyPoint[] keyPoints_template = null;
        ORB orb = null;
        VectorOfKeyPoint vKeyPoints_template = null;
        Image<Bgr, byte> orbFeatures_template = null;
        Mat orbDesc_template = null;
        public Form2()
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(this.cameraClosing);
            const int padding = 0;
            String exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String templatePath = System.IO.Path.Combine(exePath, "john.png");
            Image<Lab, byte> tempImg = new Image<Lab, byte>(templatePath);
            CvInvoke.EqualizeHist(tempImg[0], tempImg[0]);
            CvInvoke.CvtColor(tempImg, tempImg, ColorConversion.Lab2Bgr);
            templateImage = new Image<Bgr, byte>(tempImg.Width + padding*2, tempImg.Height + padding*2);
            CvInvoke.CopyMakeBorder(tempImg, templateImage, padding, padding, padding, padding, BorderType.Constant, new MCvScalar(255, 255, 255));
            orb = new ORB(6000, scaleFactor: 1.2f, nLevels: 8, patchSize: 39, scoreType: ORB.ScoreType.Fast);
            keyPoints_template = orb.Detect(templateImage);
            vKeyPoints_template = new VectorOfKeyPoint(keyPoints_template);
            orbFeatures_template = templateImage.Copy();
            Features2DToolbox.DrawKeypoints(templateImage, vKeyPoints_template, orbFeatures_template, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.Default);
            orbDesc_template = new Mat();
            orb.Compute(templateImage, vKeyPoints_template, orbDesc_template);
            pictureBox2.Image = BitmapExtension.ToBitmap<Bgr, Byte>(templateImage);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Update();
            pictureBox3.Image = BitmapExtension.ToBitmap<Bgr, Byte>(orbFeatures_template);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Update();
            m_CaptureWebCam = new VideoCapture();
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
                Image<Bgr, byte> frame_eq = null;
                // CvInvoke.cvConvertScale(frame, frame, 0.5, 0.0);

                ImageEnhancementAlgo.globalLuminanceAdapt(ref frame, out frame_eq);
                ImageEnhancementAlgo.CLAHE(ref frame_eq, out frame_eq);

                Image<Bgr, byte> orbFeatures = frame.Copy();
                Image<Bgr, byte> orbFeatures_eq = frame_eq.Copy();
                MKeyPoint[] keyPoints = orb.Detect(frame);
                MKeyPoint[] keyPoints_eq = orb.Detect(frame_eq);
                VectorOfKeyPoint vKeyPoints = new VectorOfKeyPoint(keyPoints);
                VectorOfKeyPoint vKeyPoints_eq = new VectorOfKeyPoint(keyPoints_eq);
                Features2DToolbox.DrawKeypoints(frame, vKeyPoints, orbFeatures, new Bgr(0, 255, 0), Features2DToolbox.KeypointDrawType.Default);
                Features2DToolbox.DrawKeypoints(frame_eq, vKeyPoints_eq, orbFeatures_eq, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.Default);
                
                // orbFeatures_cat = orbFeatures_cat.Resize(0.75, Inter.Area);
                Mat orbDesc = new Mat();
                Mat orbDesc_eq = new Mat();
                VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
                VectorOfVectorOfDMatch matches_eq = new VectorOfVectorOfDMatch();
                orb.Compute(frame, vKeyPoints, orbDesc);
                orb.Compute(frame_eq, vKeyPoints_eq, orbDesc_eq);
                BFMatcher matcher = new BFMatcher(DistanceType.Hamming);
                matcher.Add(orbDesc_template);

                Mat homography = null, homography_eq = null;
                if (!orbDesc.IsEmpty)
                {
                    matcher.KnnMatch(orbDesc, matches, 2, null);
                    Mat mask = new Mat(matches.Size, 1, Emgu.CV.CvEnum.DepthType.Cv8U, 1);// 1 column, 8bit unsigned depth type, 1 channel image
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, 0.75, mask);
                    int count = CvInvoke.CountNonZero(mask);
                    if (count >= 4)
                    {
                        try
                        {
                            count = Features2DToolbox.VoteForSizeAndOrientation(vKeyPoints_template, vKeyPoints, matches, mask, 1.5, 20);
                            if (count >= 4)
                            {
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(vKeyPoints_template,
                                       vKeyPoints, matches, mask, 2);
                            }
                        }
                        catch (Exception ex) 
                        {
                            // System.Diagnostics.Debug.WriteLine(ex.ToString());
                            homography = null;
                        }
                    } 
                    else
                    {
                        homography = null;
                    }
                    //System.Diagnostics.Debug.WriteLine(count.ToString());
                }
                if (!orbDesc_eq.IsEmpty)
                {
                    matcher.KnnMatch(orbDesc_eq, matches_eq, 2, null);
                    Mat mask = new Mat(matches_eq.Size, 1, Emgu.CV.CvEnum.DepthType.Cv8U, 1);// 1 column, 8bit unsigned depth type, 1 channel image
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches_eq, 0.75, mask);
                    int count = CvInvoke.CountNonZero(mask);
                    if (count >= 4)
                    {
                        try
                        {
                            count = Features2DToolbox.VoteForSizeAndOrientation(vKeyPoints_template, vKeyPoints_eq, matches_eq, mask, 1.5, 20);
                            if (count >= 4)
                            {
                                homography_eq = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(vKeyPoints_template,
                                        vKeyPoints_eq, matches_eq, mask, 2);
                            }
                        }
                        catch (Exception ex)
                        {
                            // System.Diagnostics.Debug.WriteLine(ex.ToString());
                            homography_eq = null;
                        }
                    }
                    else
                    {
                        homography_eq = null;
                    }
                    //System.Diagnostics.Debug.WriteLine(count.ToString());
                }
                //System.Diagnostics.Debug.WriteLine((homography==null).ToString());
                if (homography != null && homography.Width==3 && homography.Height==3)
                {
                    PointF[] src = new PointF[] {
                        new PointF(0,0),
                        new PointF(templateImage.Width-1,0),
                        new PointF(templateImage.Width-1,templateImage.Height-1),
                        new PointF(0,templateImage.Height-1)
                    };
                    PointF[] tarF = CvInvoke.PerspectiveTransform(src, homography);
                    Point[] tar = Array.ConvertAll<PointF, Point>(tarF, Point.Round);
                    CvInvoke.Polylines(orbFeatures, tar, true, new MCvScalar(255, 0, 0), thickness: 5);
                    /*
                    System.Diagnostics.Debug.WriteLine("\n");
                    for (int i=0; i<tar.Length; ++i)
                        System.Diagnostics.Debug.WriteLine(tar[i].ToString());
                    */
                }
                if (homography_eq != null && homography_eq.Width==3 && homography_eq.Height==3)
                {
                    PointF[] src = new PointF[] {
                        new PointF(0,0),
                        new PointF(templateImage.Width-1,0),
                        new PointF(templateImage.Width-1,templateImage.Height-1),
                        new PointF(0,templateImage.Height-1)
                    };
                    PointF[] tarF = CvInvoke.PerspectiveTransform(src, homography_eq);
                    Point[] tar = Array.ConvertAll<PointF, Point>(tarF, Point.Round);
                    CvInvoke.Polylines(orbFeatures_eq, tar, true, new MCvScalar(255, 0, 0), thickness: 5);
                    /*
                    System.Diagnostics.Debug.WriteLine("\n");
                    for (int i = 0; i < tar.Length; ++i)
                        System.Diagnostics.Debug.WriteLine(tar[i].ToString());
                    */
                }
                Image<Bgr, byte> orbFeatures_cat = orbFeatures.ConcateHorizontal(orbFeatures_eq);
                //Mat imgMatches = new Mat();
                //Features2DToolbox.DrawMatches(templateImage, vKeyPoints_template, frame, vKeyPoints, matches, imgMatches, new MCvScalar(0, 255, 0), new MCvScalar(0, 0, 255));

                pictureBox1.Image = BitmapExtension.ToBitmap<Bgr, Byte>(orbFeatures_cat);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                // pictureBox1.Update();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(ProcessWebCamFrame);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
