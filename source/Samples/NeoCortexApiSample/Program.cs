using NeoCortexApi.Encoders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NeoCortexApiSample
{
    class Program
    {

        public static Dictionary<string, object> GetDefaultEncoderSettings()
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("N", 9);
            encoderSettings.Add("W", 3);
            encoderSettings.Add("MinVal", (double)0);
            encoderSettings.Add("MaxVal", (double)9);
            //encoderSettings.Add("Radius", (double)6);
            //encoderSettings.Add("Resolution", (double)3);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            return encoderSettings;
        }

        /// <summary>
        /// This is the place to experiment with scalar encoder
        ///
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Debug.WriteLine("Inside the Main of ImproveScalarEncoder Namespace");

            Dictionary<string, object> encoderSettings = GetDefaultEncoderSettings();

            // checking different values of N
            //for (int i = 4; i < 20; i++)
            //{
            //    //encoderSettings.Add("N", i);
            //    Debug.WriteLine($"------Output for N {i}-------------------");

            //    try
            //    {
            //        NeoCortexApi.Encoders.ScalarEncoder encoderObject = new NeoCortexApi.Encoders.ScalarEncoder(encoderSettings);
            //        CheckDifferentConfiguration(encoderObject);

            //    }
            //    catch (IndexOutOfRangeException ex)
            //    {
            //        Debug.WriteLine($"Index out of range exception at {i}");

            //    }
            //    catch (OverflowException ex)
            //    {
            //        //Debug.WriteLine($"Overflow exception at {i}, {ex.StackTrace}");
            //        //Debug.WriteLine($"Source : {ex.Source}");
            //        //Debug.WriteLine($"TargetSite : {ex.TargetSite}");
            //        //if (ex.Data.Count > 0)
            //        //{
            //        //    Debug.WriteLine("  Extra details:");
            //        //    foreach (DictionaryEntry de in ex.Data)
            //        //        Debug.WriteLine("    Key: {0,-20}      Value: {1}",
            //        //                          "'" + de.Key.ToString() + "'", de.Value);
            //        //}
            //        //Debug.WriteLine($"Data : {ex.Data}");
            //        Debug.WriteLine(ex.GetBaseException());


            //    }
            //    finally
            //    {
            //        //encoderSettings.Remove("N");
            //        //Debug.WriteLine("-------------------------");
            //    }
            //}

            try
            {
                NeoCortexApi.Encoders.ScalarEncoder encoderObject = new NeoCortexApi.Encoders.ScalarEncoder(encoderSettings);
                CheckDifferentConfiguration(encoderObject);

            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine($"Index out of range exception at resolution, {ex.StackTrace}");

            }

        }


        /// <summary>
        /// function to produce output for a certain configuration of encoder settings
        /// </summary>
        /// <param name="encObject"></param>
        public static void CheckDifferentConfiguration(NeoCortexApi.Encoders.ScalarEncoder encObject)
        {
            int[] encodedData;
            for (double i = encObject.MinVal; i < encObject.MaxVal + 1; i++)
            {
                if ( (i >= encObject.MinVal) && (i <= encObject.MaxVal) )
                {

                    Debug.WriteLine($"--------input data {i}----Resoultuion {encObject.Resolution}---------");
                    try
                    {

                        encodedData = encObject.Encode(i);
                        Debug.WriteLine($"Input : {i} = {String.Join(',', encodedData)}");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Debug.WriteLine($"Index out of range exception at {i}");
                        break;
                    }
                }
                else {
                Debug.WriteLine($"Skipped : {i}");
                }
            }
        }

    }
}
