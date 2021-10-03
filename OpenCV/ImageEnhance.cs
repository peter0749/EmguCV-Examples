using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.XFeatures2D;

namespace OpenCV
{
    class ImageEnhancementAlgo
    {
        public static void globalLuminanceAdapt(ref Image<Bgr, byte> src, out Image<Bgr, byte> dst, double gainClip = 4.0)
        {
            /*
             * Method: Global Adaptation
             * Hyunchan Ahn, Byungjik Keum, Daehoon Kim, and Hwang Soo Lee, Member, IEEE Department of Electrical Engineering, KAIST, Daejeon, Korea
             * Adaptive Local Tone Mapping Based on Retinex for High Dynamic Range Images
             * ICCE 2013 
             */
            double minVal = 0.0, maxVal = 0.0;
            Point nullPoint = new Point(0, 0);
            Image<Bgr, float> src_float = src.Convert<Bgr, float>().Mul(1.0 / 255.0);
            Image<Lab, float> Lab = new Image<Lab, float>(src.Width, src.Height);
            CvInvoke.CvtColor(src_float, Lab, ColorConversion.Bgr2Lab);
            Image<Gray, float> luminance = Lab[0];
            CvInvoke.cvConvertScale(luminance, luminance, 1.0 / 100.0, 1e-5);
            CvInvoke.Threshold(luminance, luminance, 1.0, 1.0, ThresholdType.Trunc);
            CvInvoke.MinMaxLoc(luminance, ref minVal, ref maxVal, ref nullPoint, ref nullPoint, null);
            double logAve = Math.Exp((double)luminance.Log().GetAverage().Intensity);
            Image<Gray, float> hdrL = new Image<Gray, float>(src.Width, src.Height);
            CvInvoke.cvConvertScale(luminance, hdrL, 1.0 / logAve, 1.0);
            CvInvoke.Log(hdrL, hdrL);
            CvInvoke.cvConvertScale(hdrL, hdrL, 1.0 / Math.Log(1.0 + maxVal / logAve), 0.0);
            Image<Gray, float> mask = luminance.ThresholdBinary(new Gray(1e-3), new Gray(1.0));

            hdrL = hdrL.Mul(luminance.Pow(-1)).ThresholdTrunc(new Gray(gainClip)).Mul(mask);
            Image<Bgr, float> dst_float = new Image<Bgr, float>(src.Width, src.Height);
            Lab[0] = Lab[0].Mul(hdrL).ThresholdTrunc(new Gray(100.0));
            CvInvoke.CvtColor(Lab, dst_float, ColorConversion.Lab2Bgr);
            dst_float = dst_float.Mul(255.0);
            dst = dst_float.Convert<Bgr, Byte>();
        }
        public static void globalLuminanceAdapt(ref Image<Gray, byte> src, out Image<Gray, byte> dst, double gainClip = 4.0)
        {
            /*
             * Method: Global Adaptation
             * Hyunchan Ahn, Byungjik Keum, Daehoon Kim, and Hwang Soo Lee, Member, IEEE Department of Electrical Engineering, KAIST, Daejeon, Korea
             * Adaptive Local Tone Mapping Based on Retinex for High Dynamic Range Images
             * ICCE 2013 
             */
            double minVal = 0.0, maxVal = 0.0;
            Point nullPoint = new Point(0, 0);
            Image<Gray, float> luminance = src.Convert<Gray, float>();
            CvInvoke.cvConvertScale(luminance, luminance, 1.0 / 255.0, 1e-5);
            CvInvoke.Threshold(luminance, luminance, 1.0, 1.0, ThresholdType.Trunc);
            CvInvoke.MinMaxLoc(luminance, ref minVal, ref maxVal, ref nullPoint, ref nullPoint, null);
            double logAve = Math.Exp((double)luminance.Log().GetAverage().Intensity);
            Image<Gray, float> hdrL = new Image<Gray, float>(src.Width, src.Height);
            CvInvoke.cvConvertScale(luminance, hdrL, 1.0 / logAve, 1.0);
            CvInvoke.Log(hdrL, hdrL);
            CvInvoke.cvConvertScale(hdrL, hdrL, 1.0 / Math.Log(1.0 + maxVal / logAve), 0.0);
            Image<Gray, float> mask = luminance.ThresholdBinary(new Gray(1e-3), new Gray(1.0));
            hdrL = hdrL.Mul(luminance.Pow(-1)).ThresholdTrunc(new Gray(gainClip)).Mul(mask);
            Image<Gray, float> dst_float = new Image<Gray, float>(src.Width, src.Height);
            dst_float = luminance.Mul(hdrL).Mul(255.0).ThresholdTrunc(new Gray(255.0));
            dst = dst_float.Convert<Gray, byte>();
        }
        public static void CLAHE(ref Image<Bgr, byte> frame, out Image<Bgr, byte> frame_eq)
        {
            frame_eq = frame.Copy();
            CvInvoke.CvtColor(frame, frame_eq, ColorConversion.Bgr2Lab);
            Image<Gray, byte>[] Lab = frame_eq.Split();
            CvInvoke.CLAHE(Lab[0], 2.0, new Size(new Point(10, 10)), Lab[0]);
            CvInvoke.Merge(new VectorOfMat(Lab[0].Mat, Lab[1].Mat, Lab[2].Mat), frame_eq);
            CvInvoke.CvtColor(frame_eq, frame_eq, ColorConversion.Lab2Bgr);
        }
        public static void CLAHE(ref Image<Gray, byte> frame, out Image<Gray, byte> frame_eq)
        {
            frame_eq = frame.Copy();
            CvInvoke.CLAHE(frame, 2.0, new Size(new Point(10, 10)), frame_eq);
        }

        /*
        public static void toneMapping_Bilateral(ref Image<Bgr, byte> frame, out Image<Bgr, byte> frame_eq, double alpha=2.0, double compress=1.2)
        {
            frame_eq = frame.Copy();
            Image<Bgr, float> src_float = frame.Convert<Bgr, float>().Mul(1.0 / 255.0);
            Image<Lab, float> Lab = new Image<Lab, float>(frame.Width, frame.Height);
            CvInvoke.CvtColor(src_float, Lab, ColorConversion.Bgr2Lab);
            Image<Gray, float> luminance = Lab[0];
            CvInvoke.cvConvertScale(luminance, luminance, 1.0 / 100.0, 1e-5);
            CvInvoke.Threshold(luminance, luminance, 1.0, 1.0, ThresholdType.Trunc);
            Image<Gray, float> luminance_inv = luminance.Pow(-1);
            Image<Bgr, float> rgb_norm = src_float.Copy();
            for (int ch = 0; ch < 3; ++ch) 
                rgb_norm[ch] = rgb_norm[ch].Mul(luminance_inv);
            Image<Bgr, float> lowFreq = rgb_norm.SmoothBilatral(7, 31, 31);
            Image<Bgr, float> hiFreq = rgb_norm - lowFreq;
            Bgr Lw_mean = lowFreq.Log().GetAverage();
            Image<Bgr, float> alphaBgr = new Image<Bgr, float>(1, 1);
            alphaBgr[0,0] = Lw_mean;
            alphaBgr = alphaBgr.Pow(-1).Mul(alpha);
            Image<Bgr, float> Lm = lowFreq.Copy();
            for (int ch = 0; ch < 3; ++ch) Lm[ch] = Lm[ch].Mul(alphaBgr.Data[0,0,ch]);
            Image<Bgr, float> Ld = Lm.Copy();
            CvInvoke.cvConvertScale(Ld, Ld, 1.0, 1.0); // Lm + 1
            Ld = Ld.Pow(-1).Mul(Lm); // Lm / (1 + Lm)
            Image<Bgr, float> fixV = (Ld + hiFreq).ThresholdTrunc(new Bgr(1.0,1.0,1.0));
            Image<Bgr, float> ldr = src_float.Pow(compress).Mul(fixV).ThresholdTrunc(new Bgr(1.0, 1.0, 1.0));
            frame_eq = ldr.Mul(255).Convert<Bgr, byte>();
        }
        */
    }
}
