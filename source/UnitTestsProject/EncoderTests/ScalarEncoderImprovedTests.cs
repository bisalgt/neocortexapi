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
        /// Function to update encoderSettings key with respect to given value
        /// </summary>
        /// <param name="key">Key of encoderSettings to update</param>
        /// <param name="value">Value for encoderSettings key to update</param>
        /// <param name="encoderSettings">encoderSettings dictionary object</param>
        /// <returns>Updated EncoderSetting Dictionary</returns>
        public static Dictionary<string, object> UpdateEncoderSetting(string key, double value, Dictionary<string, object> encoderSettings)
        {
            if (key.ToLower() == "n")
            {
                encoderSettings.Remove("N");
                encoderSettings.Add("N", (int)value);
            }
            else if (key.ToLower() == "resolution")
            {
                encoderSettings.Remove("Resolution");
                encoderSettings.Add("Resolution", value);
            }
            else if (key.ToLower() == "radius")
            {
                encoderSettings.Remove("Radius");
                encoderSettings.Add("Radius", value);
            }

            return encoderSettings;
        }


        /// <summary>
        /// Function to check if a List has unique integer array elements
        /// </summary>
        /// <param name="resultArray">List of integer arrays that consists of encoded datas</param>
        /// <returns>Boolean: True if the parameter passed to it is unique.</returns>
        public static Boolean CheckDistinctArrayElement(List<int []> resultArray)
        {
            bool isDistinctElement = true;

            for (int i = 0; i < resultArray.Count; i++) {
                for (int j = 0; j < resultArray.Count; j++)
                {
                    if (i != j)
                    {
                        
                        int match = 0;
                        for (int k = 0; k < resultArray[0].Length; k++)
                        {
                            if (resultArray[i][k] == resultArray[j][k])
                            {
                                match++;
                            }
                        }
                        if(match == resultArray[0].Length)
                        {
                            isDistinctElement = false;
                        }
                    }
                }

            }
            return isDistinctElement;

        }


        [TestMethod]
        [TestCategory("categori")]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        [DataRow(22)]
        [DataRow(23)]
        [DataRow(24)]
        [DataRow(25)]
        [DataRow(26)]
        public void TestEncodingByUnimprovedEncoderProvidedTotalBits(int inputN)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            ScalarEncoder UnImprovedEncoderObj = new ScalarEncoder(encoderSettings);

            // This value refers to the total different encoding that a ScalarEncoder can encode.
            // If the range of data that we need to encode is more than the total different encoding that our ScalarEncoder can encode then
            // encodings are not distinct.
            int requiredTotalBits = (int)((int)encoderSettings["W"] + (double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]);

            if ((int)encoderSettings["N"] < requiredTotalBits) 
            {
                // List to append the encoded data from the scalar encoder when the value of N is too Low
                List<int[]> encodedListForLowTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int [] encoded_data = UnImprovedEncoderObj.Encode(i);
                    // Adding the encoded data to an List for comparision
                    encodedListForLowTotalBits.Add(encoded_data);
                }

                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with false as the old version of scalar encoder
                // does not provides unique encoding.   
                Assert.IsFalse(CheckDistinctArrayElement(encodedListForLowTotalBits));

            }
            // If the value of N is more than or equal to a required value then encoding is distinct by old scalar encoder
            else 
            {
                // List to append the encoded data from the scalar encoder when the value of N is enough for distinct encoding
                List<int[]> encodedListForEnoughTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int [] encoded_data = UnImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughTotalBits.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the old version of scalar encoder
                // provides unique encoding if the value of N is equal to or more than a threshold.
                
                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughTotalBits));
            }
            
        }


        [TestMethod]
        [TestCategory("categori")]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        [DataRow(22)]
        [DataRow(23)]
        [DataRow(24)]
        [DataRow(25)]
        [DataRow(26)]
        public void TestEncodingByImprovedEncoderProvidedTotalBits(int inputN)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);



            // This value refers to the total different encoding that a ScalarEncoder can encode.
            // If the range of data that we need to encode is more than the total different encoding that our ScalarEncoder can encode then
            // encodings are not distinct.
            int requiredTotalBits = (int)((int)encoderSettings["W"] + (double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]);

            if ((int)encoderSettings["N"] < requiredTotalBits)
            {
                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // If the value of N is too low, 
                    // the Improved version of ScalarEncoder will generate exception.
                    // Exception is thrown while initializing the encoder with the encoder settings,
                    // this prevents un-necessary steps.
                    // This makes sure that only distinct encoding passes
                    Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
                }

            }
            // If the value of N is more than or equal to a required value then encoding is distinct by the scalar encoder
            else
            {

                // Initializing the encoder object
                ScalarEncoderImproved ImprovedEncoderObj = new ScalarEncoderImproved(encoderSettings);

                // List to append the encoded data from the scalar encoder when the value of N is enough for distinct encoding
                List<int[]> encodedListForEnoughTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = ImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughTotalBits.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the new version of scalar encoder
                // provides unique encoding if the value of N is equal to or more than a threshold.
                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughTotalBits));
            }

        }


        /// <summary>
        /// Test method to check if encoding produces exception for low value of Total Bits (N).
        /// Because the value of N is low, encoding could produce similar SDRs and so Argument Exception is thrown by
        /// the improved scalar encoder. 
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        public void TestEncodingWithLowTotalBits(int inputN)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            List<int[]> resultArray = new List<int[]> { };

            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"]; i++)
            {
                // Arugment exception would be thrown in case the value of N is such that
                // the ScalarEncoderImproved would result in no distinct encoding.
                Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(i, encoderSettings));              

            }
        }



        /// <summary>
        /// Test method to check if encoding produces a distinct output.
        /// Encoded integer array are added to an Array.
        /// Assertion is done with the length of distinct element of Array to the length of Original Array.
        /// Assertion is True for distinct encoding.
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22)]
        [DataRow(23)]
        [DataRow(24)]
        [DataRow(25)]
        [DataRow(26)]
        public void TestDistinctEncodingWithRequiredTotalBits(int inputN)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            List<int[]> resultArray = new List<int[]> { };

            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
            {
                // The value of N is more than required so Distinct encoding is done by the ScalarEncoderImproved
                int[] result =  ReturnSDRsForEncoderSetting(i, encoderSettings);
                resultArray.Add(result);
            }
            // Assertion is True is distinct encoding by ScalarEncoderImproved
            Assert.IsTrue(CheckDistinctArrayElement(resultArray));
        }



        /// <summary>
        /// Test method to check Old ScalarEncoder produces non distinct SDRs. 
        /// For a given specification, encoding of some input values are similar.
        /// So Assertion with False should pass
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(35)]
        [DataRow(36)]
        [DataRow(37)]
        [DataRow(38)]
        [DataRow(39)]
        [DataRow(40)]
        public void TestDistinctEncodingWithLowTotalBitsForUnimprovedScalarEncoder(int inputN)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 3);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)10);
            encoderSettings.Add("MaxVal", (double)50);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            List<int[]> resultArray = new List<int[]> { };

            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
            {
                ScalarEncoder encoderObj = new ScalarEncoder(encoderSettings);

                int[] result = encoderObj.Encode(i);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
                resultArray.Add(result);


            }
            // Old ScalarEncoder encodes two different inputs with similar encoding. 
            // So the Assertion should be False as the Array Elements are not distinct.
            Assert.IsFalse(CheckDistinctArrayElement(resultArray));
        }



        /// <summary>
        /// Test method to check if encoding produces Exception for high resolution.
        /// Because the value of Resolution is high, encoding could produce similar SDRs and so Argument Exception is thrown by
        /// the improved scalar encoder. 
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        public void TestEncodingWithHighResolution(int inputResolution)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)inputResolution);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);



            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
            {
                // Arugment exception would be thrown in case the value of Resolution is such that
                // the ScalarEncoderImproved would result in no distinct encoding.
                Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(i, encoderSettings));

            }
        }



        /// <summary>
        /// Test method to check if encoding produces a distinct output.
        /// Encoded integer array are added to an Array.
        /// Assertion is done with the length of distinct element of Array to the length of Original Array.
        /// Because the value of Resolution is just enough, Distinct encoding is done by
        /// the improved scalar encoder. 
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(0.1)]
        [DataRow(0.3)]
        [DataRow(0.5)]
        [DataRow(0.7)]
        [DataRow(0.9)]
        [DataRow(1.0)]
        public void TestDistinctEncodingWithRequiredResolution(double inputResolution )
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)inputResolution);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            List<int[]> resultArray = new List<int[]> { };

            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
            {
                resultArray.Add(ReturnSDRsForEncoderSetting(i, encoderSettings));


            }
            // ScalarEncoder Improved encodes two different inputs with distinct encoding if the encoderSetting is appropriate. 
            // Assertion should be True as the Array Elements are distinct.
            Assert.IsTrue(CheckDistinctArrayElement(resultArray));
        }


        /// <summary>
        /// Test method to check if encoding produces Exception for high Radius.
        /// Because the value of Radius is high - more than number of active bits, , encoding could produce similar SDRs and so Argument Exception is thrown by
        /// the improved scalar encoder. 
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(10)]
        public void TestEncodingWithHighRadius(int inputRadius)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)inputRadius);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);



            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
            {
                // Arugment exception would be thrown in case the value of Radius is such that
                // the ScalarEncoderImproved would result in no distinct encoding.
                Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(i, encoderSettings));

            }
        }



        /// <summary>
        /// Test method to check if distinct encoding is produced by required Radius.
        /// Because the value of Radius is just appropriate, distinct encoding is produce by ScalarEncoderImproved.
        /// Distinct assertion check is done after adding all the output to a List of integer array.
        /// </summary>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void TestDistinctEncodingWithRequiredRadius(int inputRadius)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)22);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)inputRadius);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);



            List<int[]> resultArray = new List<int[]> { };

            for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
            {
                resultArray.Add(ReturnSDRsForEncoderSetting(i, encoderSettings));


            }
            // ScalarEncoder Improved encodes two different inputs with distinct encoding if the encoderSetting is appropriate. 
            // Assertion should be True as the Array Elements are distinct.
            Assert.IsTrue(CheckDistinctArrayElement(resultArray));
        }



        /// <summary>
        /// Test Case to check encoding with different value of N.
        /// If the value of N is less than required exception is thrown.
        /// If the value is more than or equal to required, encoding is done and
        /// assertion is checked with the return type of encoder which is integer array.
        /// </summary>
        /// <param name="input"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(22)]
        [DataRow(25)]
        [DataRow(29)]
        [DataRow(32)]
        [DataRow(39)]
        public void TestCheckForDifferentValueOfTotalBitsForNonPeriodic(int input)
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
                    // This will run if the value of TotalBits is minimum required or more. 
                    // this will be true if the data returned after encoding is integer array type
                    Assert.IsInstanceOfType(ReturnSDRsForEncoderSetting(input, encoderSettings), typeof(int[]));
                }
            }

        }

        /// <summary>
        /// Test case to check if encoding works properly for a periodic input 
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
        public void TestCheckForDifferentValueOfTotalBitsForPeriodic(int input)
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
                    // this will be true if the data returned after encoding is integer array type
                    Assert.IsInstanceOfType(ReturnSDRsForEncoderSetting(input, encoderSettings), typeof(int []));
                }
                else if (input >= (double)encoderSettings["MaxVal"])
                {
                    // For a Periodic input, argument exception is thrown as value of input to encode should be
                    // strictly less than the MaxVal to encode.
                    Assert.ThrowsException<System.ArgumentException>(() => ReturnSDRsForEncoderSetting(input, encoderSettings));
                }
            }

        }



        /// <summary>
        /// Tests to show enough Total Bits results in proper encoding.
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


    }
}

