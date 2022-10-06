using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VarjoInterface
{
    public static class VarjoApp
    {
        public static VarjoData varjoData;

        [StructLayout(LayoutKind.Sequential)]
        public struct VarjoData
        {
            public GazeData gazeData;
            public EyeMeasurements eyeData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector
        {

            public double x;
            public double y;
            public double z;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GazeRay
        {
            public Vector origin;   //!< Origin of the ray.
            public Vector forward;  //!< Direction of the ray.
        }

        public enum GazeStatus : long
        {
            Invalid = 0,
            Adjust = 1,
            Valid = 2
        }

        public enum GazeEyeStatus : long
        {
            Invalid = 0,
            Visible = 1,
            Compensated = 2,
            Tracked = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GazeData
        {
            public GazeRay leftEye;                 //!< Left eye gaze ray.
            public GazeRay rightEye;                //!< Right eye gaze ray.
            public GazeRay gaze;                    //!< Normalized gaze direction ray.
            public double focusDistance;            //!< Estimated gaze direction focus point distance.
            public double stability;                //!< Focus point stability.
            public long captureTime;                //!< Varjo time when this data was captured, see varjo_GetCurrentTime()
            public GazeEyeStatus leftStatus;        //!< Status of left eye data.
            public GazeEyeStatus rightStatus;       //!< Status of right eye data.
            public GazeStatus status;               //!< Tracking main status.
            public long frameNumber;                //!< Frame number, increases monotonically.
            public double leftPupilSize;            //!< <DEPRECATED> Normalized [0..1] left eye pupil size.
            public double rightPupilSize;           //!< <DEPRECATED> Normalized [0..1] right eye pupil size.
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EyeMeasurements
        {
            public long frameNumber;                    //!< Frame number, increases monotonically.  
            public long captureTime;                    //!< Varjo time when this data was captured, see varjo_GetCurrentTime()
            public float interPupillaryDistanceInMM;    //!< Estimated IPD in millimeters
            public float leftPupilIrisDiameterRatio;    //!< Ratio between left pupil and left iris.
            public float rightPupilIrisDiameterRatio;   //!< Ratio between right pupil and right iris.
            public float leftPupilDiameterInMM;         //!< Left pupil diameter in mm
            public float rightPupilDiameterInMM;        //!< Right pupil diameter in mm
            public float leftIrisDiameterInMM;          //!< Left iris diameter in mm
            public float rightIrisDiameterInMM;         //!< Right iris diameter in mm
            public float leftEyeOpenness;               //!< Estimate of the ratio of openness of the left eye where 1 corresponds to a fully open eye and 0 corresponds to a fully closed eye. 
            public float rightEyeOpenness;              //!< Estimate of the ratio of openness of the right eye where 1 corresponds to a fully open eye and 0 corresponds to a fully closed eye. 
        }

        public static IntPtr session;

        public static IntPtr GetVarjoSession()
        {
            return session;
        }
        public static IntPtr SessionInit()
        {
            session = varjo_SessionInit();
            return session;
        }

        public static void GazeInit()
        {
            varjo_GazeInit(session);
        }

        public static bool IsGazeAllowed()
        {
            return varjo_IsGazeAllowed(session);
        }

        public static bool IsGazeCalibrated()
        {
            return varjo_GetPropertyBool(session, 0xA001);
        }

        public static void SyncProperties()
        {
            varjo_SyncProperties(session);
        }

        public static void RequestGazeCalibration()
        {
            varjo_RequestGazeCalibration(session);
        }

        public static GazeCalibrationQuality GetGazeCalibrationQuality()
        {
            return new GazeCalibrationQuality
            {
                left = GetGazeCalibrationQualityLeft(),
                right = GetGazeCalibrationQualityRight()
            };
        }

        public static GazeEyeCalibrationQuality GetGazeCalibrationQualityLeft()
        {
            return (GazeEyeCalibrationQuality)varjo_GetPropertyInt(session, 0xA004);
        }
        
        public static GazeEyeCalibrationQuality GetGazeCalibrationQualityRight()
        {
            return (GazeEyeCalibrationQuality)varjo_GetPropertyInt(session, 0xA005);
        }

        public static void GetGaze()
        {
            varjoData.gazeData = varjo_GetGaze(session);
        }

        public static bool GetGazeData()
        {
            return varjo_GetGazeData(session, out varjoData.gazeData, out varjoData.eyeData);
        }

        public static void DebugString()
        {
            Console.WriteLine("varjoData.eyeData.captureTime: " + varjoData.eyeData.captureTime);
            Console.WriteLine("varjoData.eyeData.frameNumber: " + varjoData.eyeData.frameNumber);
            Console.WriteLine("varjoData.eyeData.interPupillaryDistanceInMM: " + varjoData.eyeData.interPupillaryDistanceInMM);
            Console.WriteLine("varjoData.eyeData.leftEyeOpenness: " + varjoData.eyeData.leftEyeOpenness);
            Console.WriteLine("varjoData.eyeData.leftIrisDiameterInMM: " + varjoData.eyeData.leftIrisDiameterInMM);
            Console.WriteLine("varjoData.eyeData.leftPupilDiameterInMM: " + varjoData.eyeData.leftPupilDiameterInMM);
            Console.WriteLine("varjoData.eyeData.leftPupilIrisDiameterRatio: " + varjoData.eyeData.leftPupilIrisDiameterRatio);
            Console.WriteLine("varjoData.eyeData.rightEyeOpenness: " + varjoData.eyeData.rightEyeOpenness);
            Console.WriteLine("varjoData.eyeData.rightIrisDiameterInMM: " + varjoData.eyeData.rightIrisDiameterInMM);
            Console.WriteLine("varjoData.eyeData.rightPupilDiameterInMM: " + varjoData.eyeData.rightPupilDiameterInMM);
            Console.WriteLine("varjoData.eyeData.rightPupilIrisDiameterRatio: " + varjoData.eyeData.rightPupilIrisDiameterRatio);
            Console.WriteLine("");
        }

        public static void Shutdown()
        {
            varjo_SessionShutDown(session);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GazeCalibrationParameter
        {
            [MarshalAs(UnmanagedType.LPStr)] public string key;
            [MarshalAs(UnmanagedType.LPStr)] public string value;
        }

        public enum GazeCalibrationMode
        {
            Legacy,
            Fast
        };

        public enum GazeOutputFilterType
        {
            None,
            Standard
        }

        public enum GazeOutputFrequency
        {
            MaximumSupported,
            Frequency100Hz,
            Frequency200Hz
        }

        public enum GazeEyeCalibrationQuality
        {
            Invalid = 0,
            Low = 1,
            Medium = 2,
            High = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GazeCalibrationQuality
        {
            public GazeEyeCalibrationQuality left;
            public GazeEyeCalibrationQuality right;
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool varjo_IsAvailable();

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr varjo_SessionInit();

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void varjo_SessionShutDown(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void varjo_GazeInit(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern int varjo_GetError(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern string varjo_GetErrorDesc(int errorCode);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool varjo_IsGazeAllowed(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool varjo_IsGazeCalibrated(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern GazeData varjo_GetGaze(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool varjo_GetGazeData(IntPtr session, out GazeData gaze, out EyeMeasurements eyeMeasurements);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void varjo_RequestGazeCalibration(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool varjo_GetPropertyBool(IntPtr session, int propertyKey);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern int varjo_GetPropertyInt(IntPtr session, int propertyKey);

        [DllImport("VarjoLib", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        private static extern void varjo_SyncProperties(IntPtr session);
    }
}