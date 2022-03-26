using NeoCortexApi;
using NeoCortexApi.Encoders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ScalarEncoderImproved
{
    class Program
    {
        /// <summary>
        /// Function to take valid input from the console.
        /// yes || y or no || n are only valid inputs from user.
        /// This check is not case sensitive.
        /// </summary>
        /// <returns> Boolean</returns>
        public static Boolean GetInputFromConsole()
        {
            bool checker = true; // boolean to take valid input from console
            bool updater = false; // boolean to send update signal if true
            while (checker)
            {
                Console.Write("Update the Value ? : ");
                string tempN = Console.ReadLine().ToLower();
                if (tempN == "no" || tempN == "n")
                {
                    Console.WriteLine("Noted : NO!\nKeeping value as it is.\n");
                    checker = false;
                }
                else if (tempN == "yes" || tempN == "y")
                {
                    Console.WriteLine("Noted: YES!\nUpdating to appropriate value.\n");
                    updater = true;
                    checker = false;
                }
                else
                {
                    Console.WriteLine("Enter either yes or no!");
                }
            }
            return updater;
        }

        /// <summary>
        /// Function to check a given encoder settings. 
        /// If the value of N is less than required, encoderSetting can be updated
        /// according to the choice of the user. EncoderSettings updates value of N for a resolution of 1 if user choose yes.
        /// If the user choose "no" then encoderSettings will be left untouched.
        /// This makes sure that user can get distinct encoding for a Resolution of 1.
        /// </summary>
        /// <param name="encoderSettings"></param>
        /// <returns></returns>
        public static Dictionary<string, object> CheckEncoderSettings(Dictionary<string, object> encoderSettings)
        {
            // if condition checking if N is provided
            if ((Int32)encoderSettings.GetValueOrDefault("N", 0) != 0)
            {
                if (!(Boolean)(encoderSettings["Periodic"]))
                {

                    int requiredN = (int)((int)encoderSettings["W"] + (double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]);
                    if ((int)encoderSettings["N"] < requiredN)
                    {
                        Console.WriteLine(
                            $"The value of N is less than required {requiredN} for resolution of 1." +
                            $"This might cause some output SDRs to overlap."
                            );

                        Console.WriteLine("Are you sure you want to proceed with the entered value of N or " +
                            "do you want to use recommended N ? " +
                            "\n\nEnter [yes] if you would like to update N to minimum required. [no] if you don't.\n");
                        
                        // Getting input from console and updating N if required
                        if (GetInputFromConsole())
                        {
                            encoderSettings["N"] = requiredN;
                        }
               
                    }
                }
            }
            // if Resolution is provided
            else if ((double)encoderSettings.GetValueOrDefault("Resolution", 0.0) != 0.0) {

                if((double)encoderSettings["Resolution"] > 1.0)
                {
                    Console.WriteLine(
                        "The current value of Resolution will encode two values in similar encoding" +
                        ", provided that two value are separated by less than current value of Resolution " +
                        $"i.e. {(double)encoderSettings["Resolution"]}.\n"
                        );
                    Console.WriteLine("Are you sure you want to proceed with the entered value of Resolution or " +
                            "do you want to use recommended Resolution - 1 ? " +
                            "\n\nEnter [yes] if you would like to update Resolution. [no] if you don't.\n");


                    // Getting input from console and updating N if required
                    if (GetInputFromConsole())
                    {
                        encoderSettings["Resolution"] = 1.0;
                    }
                }
            }

            // if Radius is provided
            else if ((double)encoderSettings.GetValueOrDefault("Radius", 0.0) != 0.0)
            {
                double requiredRadius = (int)encoderSettings["W"];
                double Resolution = (double)encoderSettings["Radius"] / (int)encoderSettings["W"];

                if (Resolution > 1.0)
                {
                    Console.WriteLine(
                        "The current value of Radius will encode two values in similar encoding" +
                        " if two value are separated by less than  " +
                        $"{Resolution}.\n"
                        );
                    Console.WriteLine("Are you sure you want to proceed with the entered value of Radius or " +
                            "do you want to update Radius for a  Resolution of 1 ? " +
                            "\n\nEnter [yes] if you would like to update Radius. [no] if you don't.\n");

                    // Getting input from console and updating N if required
                    if (GetInputFromConsole())
                    {
                        encoderSettings["Radius"] = requiredRadius;
                    }
                }
            }

            return encoderSettings;
        }

        /// <summary>
        /// Function to set encoderSettings parameter for ScalarEncoder
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> GetDefaultEncoderSettings()
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            //encoderSettings.Add("N", 5);
            encoderSettings.Add("W", 3);
            encoderSettings.Add("MinVal", (double)0);
            encoderSettings.Add("MaxVal", (double)9);
            encoderSettings.Add("Radius", (double)6);
            //encoderSettings.Add("Resolution", (double)2.0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            
            // returning the checked encoder settings
            return CheckEncoderSettings(encoderSettings);
        }

        /// <summary>
        /// Main Function to experiment with Scalar Encoder
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //if(args != null)
            //{
            //    Console.WriteLine(args)
            //}
            Debug.WriteLine("Inside the Main of ImproveScalarEncoder Namespace.\n");
            Console.WriteLine("Inside the Main of ImproveScalarEncoder Namespace.\n");

            Dictionary<string, object> encoderSettings = GetDefaultEncoderSettings();

            // checking different values of Resolution at a time
            //for (double i = 1; i < 10; i++)
            //{
            //encoderSettings.Add("Resolution", i);
            //Debug.WriteLine($"------Output for Resolution {i}-------------------");

            //try
            // {
            //        NeoCortexApi.Encoders.ScalarEncoder encoderObject = new NeoCortexApi.Encoders.ScalarEncoder(encoderSettings);
            //        CheckDifferentConfiguration(encoderObject);

            //    }
            // catch (IndexOutOfRangeException ex)
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
            //        encoderSettings.Remove("Resolution");
            //        Debug.WriteLine("-------------------------");
            //    }
            //}

            try
            {
                NeoCortexApi.Encoders.ScalarEncoderImproved encoderObject = new NeoCortexApi.Encoders.ScalarEncoderImproved(encoderSettings);
                EncodeForParticularSetting(encoderObject);

            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine($"Index out of range exception at resolution, {ex.StackTrace}");

            }

        }


        /// <summary>
        /// function to produce output for a certain configuration of encoder settings
        /// </summary>
        /// <param name="encObject"> ScalarEncoderImproved Object</param>
        public static void EncodeForParticularSetting(NeoCortexApi.Encoders.ScalarEncoderImproved encObject)
        {
            int[] encodedData;
            for (double i = encObject.MinVal; i < encObject.MaxVal + 1; i += 1)
            {
                if ((i >= encObject.MinVal) && (i <= encObject.MaxVal))
                {

                    Debug.WriteLine($"--------input data {i}----Resolution {encObject.Resolution}---------");
                    try
                    {

                        encodedData = encObject.Encode(i);
                        Debug.WriteLine($"Input : {i} = {String.Join(',', encodedData)}");
                        Console.WriteLine($"Input : {i} = {String.Join(',', encodedData)}");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Debug.WriteLine($"Index out of range exception at {i}");
                        break;
                    }
                }
                else
                {
                    Debug.WriteLine($"Skipped : {i}");
                }
            }
        }

    }
}
