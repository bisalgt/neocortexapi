// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortex;
using NeoCortexApi.Encoders;
using NeoCortexApi.Network;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace UnitTestsProject.EncoderTests
{



    /// <summary>
    /// Defines the <see cref="ScalarEncoderImprovedTests"/>
    /// </summary>
    [TestClass]
    public class ScalarEncoderImprovedTests
    {
        /// <summary>
        /// Function to return encoding for a particular Input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoderSettings"></param>
        /// <returns></returns>
        public static int[] ReturnSDRsForEncoderSetting(double input, Dictionary<string, object> encoderSettings)
        {
            ScalarEncoderImproved encoder = new ScalarEncoderImproved(encoderSettings);
            var result = encoder.Encode(input);
            return result;
        }


        /// <summary>
        /// Function to update encoderSettings key with respect value
        /// </summary>
        /// <param name="key">Key of encoderSettings to update</param>
        /// <param name="value">Value for encoderSettings key to update</param>
        /// <param name="encoderSettings">encoderSettings dictionary object</param>
        /// <returns>Updated EncoderSetting Dictionary</returns>
        public static Dictionary<string, object> UpdateEncoderSetting(string key, int value, Dictionary<string, object> encoderSettings)
        {
            if (key.ToLower() == "n")
            {
                encoderSettings.Remove("N");
                encoderSettings.Add("N", value);
            }
            else if (key.ToLower() == "resolution")
            {
                encoderSettings.Remove("Resolution");
                encoderSettings.Add("Resolution", (double)value);
            }
            else if (key.ToLower() == "radius")
            {
                encoderSettings.Remove("Radius");
                encoderSettings.Add("Radius", (double)value);
            }

            return encoderSettings;
        }


        /// <summary>
        /// Test method to check if encoding produces a distinct output.
        /// Encoded integer array are added to an Array.
        /// Assertion is done with the length of distinct element of Array to the length of Array
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        public void TestDistinctEncoding()
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)5);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            List<int[]> resultArray = new List<int[]> { };

            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"]; i++)
            {
                int[] result = ReturnSDRsForEncoderSetting(i, encoderSettings);
                
                resultArray.Add(result);

            }
            Console.WriteLine(resultArray.Distinct().ToList().Count);
            Console.WriteLine(resultArray.Count);

            Assert.IsTrue(resultArray.Distinct().ToList().Count == resultArray.Count);
        }


        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22)]
        [DataRow(25)]
        [DataRow(29)]
        [DataRow(32)]
        [DataRow(39)]
        public void TestCheckDifferentValueOfTotalBitsForNonPeriodic(int input)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            for(int n = (int)encoderSettings["W"]; n < 30; n++)
            {
                encoderSettings = UpdateEncoderSetting("N", n, encoderSettings);


                // if the value of N is less than required then exception is generated
                if ( n < ((int)encoderSettings["W"] + (double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]))
                {
                    Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(input, encoderSettings));
                }
                else
                {
                    Assert.IsInstanceOfType(ReturnSDRsForEncoderSetting(input, encoderSettings), typeof(int[]));
                }
            }

        }

        /// <summary>
        /// Test if encoding works properly for a periodic input 
        /// with different value of N(Total Bits).
        /// </summary>
        /// <param name="input"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22)]
        [DataRow(25)]
        [DataRow(29)]
        [DataRow(32)]
        [DataRow(39)]
        public void TestCheckDifferentValueOfTotalBitsForPeriodic(int input)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)true);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            for (int n = (int)encoderSettings["W"]; n < 30; n++)
            {
                encoderSettings = UpdateEncoderSetting("N", n, encoderSettings);

                // if the value of N is less than required then exception is generated
                if (input < (double)encoderSettings["MaxVal"] & n < ((double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]))
                {
                    Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(input, encoderSettings));
                }
                else if (input < (double)encoderSettings["MaxVal"] & n >= ((double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]))
                {
                    Assert.IsInstanceOfType(ReturnSDRsForEncoderSetting(input, encoderSettings), typeof(int []));
                }
                else if (input >= (double)encoderSettings["MaxVal"])
                {
                    Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(input, encoderSettings));
                }
            }

        }


        /// <summary>
        /// Tests to show Low number of Total Bits (N) will result in ArgumentException
        /// </summary>
        /// <param name="input">Input to check</param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(11)]
        [DataRow(13)]
        [DataRow(15)]
        [DataRow(18)]
        [DataRow(20)]
        public void TestArgumentExceptionWithLowTotalBits(int input)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 3);
            encoderSettings.Add("N", (int)10);
            encoderSettings.Add("MinVal", (double)11);
            encoderSettings.Add("MaxVal", (double)20);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
        }

        /// <summary>
        /// Tests to show enough Total Bits results in Encoding.
        /// Calls ScalarEncoderImproved.Encode Method
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expectedResult"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(11, new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(12, new int[] { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(14, new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(17, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 })]
        [DataRow(20, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1 })]
        public void TestNoArgumentExceptionWithEnoughTotalBits(int input, int[] expectedResult)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 3);
            encoderSettings.Add("N", (int)15);
            encoderSettings.Add("MinVal", (double)11);
            encoderSettings.Add("MaxVal", (double)20);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            ScalarEncoderImproved encoder = new ScalarEncoderImproved(encoderSettings);

            var result = encoder.Encode(input);

            Debug.WriteLine(input);
            Debug.WriteLine("Current Result : ");
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.Write("ExpectedResult : ");
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));
        }


        /// <summary>
        /// Tests to check if High value of Resolution results in Argument Exception.
        /// High value of Resolution will generate exception is the Resolution is greater than 1.
        /// Resolution 1 encodes two inputs which are 1 apart with different SDR representations.
        /// </summary>
        /// <param name="input"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void TestArgumentExceptionWithHighResolution(int input)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 3);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)0);
            encoderSettings.Add("MaxVal", (double)9);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)5);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
        }


        /// <summary>
        /// Test Case to check if Resolution 1 results in Encoding without exception
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expectedResult"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(25, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(29, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(34, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 })]
        [DataRow(39, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 })]
        public void TestNoArgumentExceptionWithResolutionForUniqueEncoding(int input, int[] expectedResult)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)1);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            ScalarEncoderImproved encoder = new ScalarEncoderImproved(encoderSettings);

            var result = encoder.Encode(input);

            Debug.WriteLine(input);
            Debug.WriteLine("Current Result : ");
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.Write("ExpectedResult : ");
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));
        }


        /// <summary>
        /// High value of Radius results in similar encoding of two different inputs.
        /// Checking to see if too high Radius resulted in Argument Exception.
        /// Argument exception is thrown during initializing the Scalar Encoder.
        /// </summary>
        /// <param name="input"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22)]
        [DataRow(25)]
        [DataRow(29)]
        [DataRow(34)]
        [DataRow(39)]
        public void TestArgumentExceptionWithHighRadius(int input)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)10);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
        }



        /// <summary>
        /// No Argument Exception should be thrown when the value of Radius is just required,
        /// so that distinct encoding occurs
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expectedResult"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(25, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(29, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
        [DataRow(34, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 })]
        [DataRow(39, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 })]
        public void TestNoArumentExceptionWithRadiusForUniqueEncoding(int input, int[] expectedResult)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)5);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            ScalarEncoderImproved encoder = new ScalarEncoderImproved(encoderSettings);

            var result = encoder.Encode(input);

            Debug.WriteLine(input);
            Debug.WriteLine("Current Result : ");
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.Write("ExpectedResult : ");
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));
        }
    }
}

