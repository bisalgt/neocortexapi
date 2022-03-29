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
        [TestCategory("categori1")]
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
        public void TestEncodingByUnImprovedEncoderProvidedTotalBits(int inputN)
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
        [TestCategory("categori2")]
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



        [TestMethod]
        [TestCategory("categori3")]
        [DataRow(0.1)]
        [DataRow(0.3)]
        [DataRow(0.5)]
        [DataRow(0.6)]
        [DataRow(0.9)]
        [DataRow(1)]
        [DataRow(1.4)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(7)]
        public void TestEncodingByUnImprovedEncoderProvidedResolution(double inputResolution)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 7);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)2);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)inputResolution);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            // Initializing the encoder object with required settings
            ScalarEncoder UnImprovedEncoderObj = new ScalarEncoder(encoderSettings);

            // This value makes sure that scalars are encoded differently
            // which are maximum 1 distance apart of each other.
            double requiredResolution = 1.0;

            // For some values of Resolution, in our case 0.6 and 2.0, IndexOutOfRangeException is thrown by the old scalar encoder while encoding
            // This is due to the value of N calculated in the older version
            // In newer version this is solved
            if (((double)encoderSettings["Resolution"] > requiredResolution) & ((double)encoderSettings["Resolution"] != 0.6 & (double)encoderSettings["Resolution"] != 2.0))
            {

                // List to append the encoded data from the scalar encoder when the value of Resolution is higher than threshold
                List<int[]> encodedListForHighResolution = new List<int[]> { };


                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);
                    // Adding the encoded data to an List for comparision
                    encodedListForHighResolution.Add(encoded_data);
                }

                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with false as the old version of scalar encoder
                // does not provides unique encoding.   
                Assert.IsFalse(CheckDistinctArrayElement(encodedListForHighResolution));

            }
            // Run encoding if the value of Resolution is more than required.
            // If the value of Resolution is less than or equal to 1.0 then encoding is distinct by the older version of scalar encoder.
            // Some values leads to indexoutofrange exception, this is because of the way how N is calculated.
            // In newer version this is solved.
            else if (((double)encoderSettings["Resolution"] <= requiredResolution) & ((double)encoderSettings["Resolution"] != 0.6 & (double)encoderSettings["Resolution"] != 2.0))
            {

                // List to append the encoded data from the scalar encoder when the value of resolution is enough for distinct encoding
                List<int[]> encodedListForEnoughResolution = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughResolution.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the new version of scalar encoder
                // provides unique encoding if the value of Resolution is less than threshold, i.e. 1.
                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughResolution));
            }

            // When encoding for some input space, with the above settings,
            // In our case, resolution 0.6 and 2.0 results in IndexOutOfRange Exception for some input spaces
            // This is because of how the value of N is calculated in older version inside the Encode method.
            else if (((double)encoderSettings["Resolution"] == 0.6) || ((double)encoderSettings["Resolution"] == 2.0))
            {
                // Variable to track number of exception occured
                int exceptionCounter = 0;

                // List to append the encoded data from the scalar encoder when the value of resolution is enough for distinct encoding
                List<int[]> encodedListForEnoughResolution = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    

                    // Getting the encoding of data
                    // We are using try and catch block because IndexOutOfRange exception occurs at random value of input data , which needs to be encoded
                    // Using try catch block to catch the exception and Assertion is done for IndexOutOfRangeException
                    try
                    {
                        int[] encoded_data = UnImprovedEncoderObj.Encode(i);

                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Increasing the counter when exception is occured
                        exceptionCounter++;

                        // Assertion is done in case Exception is thrown
                        // OPTIONAL
                        Assert.ThrowsException<IndexOutOfRangeException>(() => UnImprovedEncoderObj.Encode(i));

                    }

                }
                // Exception is occured for some input space while encoding with the above settings
                // exceptionCounter variable will be increased if exception is thrown
                // exceptionCounter should be at least one.
                Assert.IsTrue(exceptionCounter > 0);
            }

        }



        [TestMethod]
        [TestCategory("categori4")]
        [DataRow(0.1)]
        [DataRow(0.3)]
        [DataRow(0.5)]
        [DataRow(0.6)]
        [DataRow(0.9)]
        [DataRow(1)]
        [DataRow(1.4)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(7)]
        public void TestEncodingByImprovedEncoderProvidedResolution(double inputResolution)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 7);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)2);
            encoderSettings.Add("MaxVal", (double)39);
            encoderSettings.Add("Radius", (double)0);
            encoderSettings.Add("Resolution", (double)inputResolution);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            // This value makes sure that scalars are encoded differently
            // which are maximum 1 distance apart of each other.
            double requiredResolution = 1.0;

            if ((double)encoderSettings["Resolution"] > requiredResolution)
            {
                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // If the value of Resolution is high
                    // the Improved version of ScalarEncoder will generate exception.
                    // Exception is thrown while initializing the encoder with the encoder settings,
                    // this prevents un-necessary steps.
                    // This makes sure that only distinct encoding passes
                    Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
                }
            }
            // If the value of Resolution is less than or equal to 1.0 then encoding is distinct by the new version of scalar encoder
            else
            {

                // Initializing the encoder object
                ScalarEncoderImproved ImprovedEncoderObj = new ScalarEncoderImproved(encoderSettings);

                // List to append the encoded data from the scalar encoder when the value of Resolution is enough for distinct encoding
                List<int[]> encodedListForEnoughResolution = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = ImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughResolution.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the new version of scalar encoder
                // provides unique encoding if the value Resoulution is more than 1.
                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughResolution));
            }

        }




        [TestMethod]
        [TestCategory("categori5")]
        [DataRow(0.3)]
        [DataRow(0.7)]
        [DataRow(1.5)]
        [DataRow(4)]
        [DataRow(4.7)]
        [DataRow(5)]
        [DataRow(9)]
        [DataRow(24)]
        [DataRow(25)]
        [DataRow(26)]
        public void TestEncodingByUnImprovedEncoderProvidedRadius(double inputRadius)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 9);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)115);
            encoderSettings.Add("MaxVal", (double)159);
            encoderSettings.Add("Radius", (double)inputRadius);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            ScalarEncoder UnImprovedEncoderObj = new ScalarEncoder(encoderSettings);

            // The value of Radius that leads to distinct encoding is the value of total number of active bits.
            // Encoder will encode distinct value if the value of Radius is less than or equal to active bits.
            double requiredRadius = (int)encoderSettings["W"];

            // checking if the value of Radius is more than required.
            // Radius are also checked with values 0.7, 24 and 25 because these values results in exception for some input space.
            // If the value is more than required and not 0.7, 24 or 25 then encoding is done without exception.
            // Since encoding is not distinct, it assertion with false will pass.
            if (((double)encoderSettings["Radius"] > requiredRadius) & ((double)encoderSettings["Radius"] != 0.7) & ((double)encoderSettings["Radius"] != 24.0) & ((double)encoderSettings["Radius"] != 25.0))
            {
                // List to append the encoded data from the scalar encoder when the value of N is too Low
                List<int[]> encodedListForHighRadius = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);
                    // Adding the encoded data to an List for comparision
                    encodedListForHighRadius.Add(encoded_data);
                }

                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with false as the old version of scalar encoder
                // does not provides unique encoding if the value of radius is more than a threshold.   
                Assert.IsFalse(CheckDistinctArrayElement(encodedListForHighRadius));

            }
            // If the value of Radius is less than or equal to required value of Radius, then encoding is distinct by old scalar encoder
            // Encoding with Radius = 0.7 or 24 or 25 will result in exception
            else if(((double)encoderSettings["Radius"] <= requiredRadius) & ((double)encoderSettings["Radius"] != 0.7) & ((double)encoderSettings["Radius"] != 24.0) & ((double)encoderSettings["Radius"] != 25.0))
            {
                // List to append the encoded data from the scalar encoder when the value of N is enough for distinct encoding
                List<int[]> encodedListForEnoughTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughTotalBits.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the old version of scalar encoder
                // provides unique encoding

                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughTotalBits));
            }

            else if (((double)encoderSettings["Radius"] == 0.7) || ((double)encoderSettings["Radius"] == 24) || ((double)encoderSettings["Radius"] == 25))
            {
                // Variable to track number of exception occured
                int exceptionCounter = 0;

                // List to append the encoded data from the scalar encoder when the value of resolution is enough for distinct encoding
                List<int[]> encodedListForEnoughResolution = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {


                    // Getting the encoding of data
                    // We are using try and catch block because IndexOutOfRange exception occurs at random value of input data , which needs to be encoded
                    // Using try catch block to catch the exception and Assertion is done for IndexOutOfRangeException
                    try
                    {
                        int[] encoded_data = UnImprovedEncoderObj.Encode(i);

                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Increasing the counter when exception is occured
                        exceptionCounter++;

                        // Assertion is done in case Exception is thrown
                        // OPTIONAL
                        Assert.ThrowsException<IndexOutOfRangeException>(() => UnImprovedEncoderObj.Encode(i));

                    }

                }
                // Exception is occured for some input space while encoding with the above settings
                // exceptionCounter variable will be increased if exception is thrown
                // exceptionCounter should be at least one.
                Assert.IsTrue(exceptionCounter > 0);
            }

        }



        [TestMethod]
        [TestCategory("categori6")]
        [DataRow(0.3)]
        [DataRow(0.7)]
        [DataRow(1.5)]
        [DataRow(4)]
        [DataRow(4.7)]
        [DataRow(5)]
        [DataRow(9)]
        [DataRow(24)]
        [DataRow(25)]
        [DataRow(26)]
        public void TestEncodingByImprovedEncoderProvidedRadius(double inputRadius)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 9);
            encoderSettings.Add("N", (int)0);
            encoderSettings.Add("MinVal", (double)115);
            encoderSettings.Add("MaxVal", (double)159);
            encoderSettings.Add("Radius", (double)inputRadius);
            encoderSettings.Add("Resolution", (double)0);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            // The value of Radius that leads to distinct encoding is the value of total number of active bits.
            // Encoder will encode distinct value if the value of Radius is less than or equal to active bits.
            double requiredRadius = (int)encoderSettings["W"];

            if ((double)encoderSettings["Radius"] > requiredRadius)
            {

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // If the value of Radius is high than the required radius then
                    // the Improved version of ScalarEncoder will generate exception.
                    // Exception is thrown because encoding would result in similar SDRs for different input spaces.
                    // Exception is thrown while initializing the encoder with the encoder settings,
                    // This prevents un-necessary steps.
                    // This makes sure that only distinct encoding passes
                    Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
                }
            }
            // If the value of Radius is less than or equal to the requiredRadius (which is the value of total number of input bits)
            // then encoding is distinct by the new version of scalar encoder
            else
            {

                // Initializing the encoder object
                ScalarEncoderImproved ImprovedEncoderObj = new ScalarEncoderImproved(encoderSettings);

                // List to append the encoded data from the scalar encoder when the value of Resolution is enough for distinct encoding
                List<int[]> encodedListForEnoughRadius = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = ImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughRadius.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the new version of scalar encoder
                // provides unique encoding if the value of Radius is less than or equal to
                // the required value of radius for distinct encoding.
                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughRadius));
            }

        }






        // Testing if N, Resolution and Radius are mutually exclusive parameters
        // Only one of 3 of them should be set at a time, remaining two should be zero.
        // Based on anyone of their value, remaining two is calculated internally by the encoder.
        // Older version of scalar encoder encodes even when all or any one of the data are given. 
        [TestMethod]
        [TestCategory("categori7")] // DataRow(N, Resolution, Radius)
        [DataRow(18, 0.3, 0)]
        [DataRow(18, 0, 4)]
        [DataRow(0, 0.3, 4)]
        [DataRow(18, 0.3, 4)]
        [DataRow(18, 0, 0)]
        [DataRow(0, 0.3, 0)]
        [DataRow(0, 0, 4)]
        public void TestingMutualExclusivenessOfParametersForUnImprovedEncoder(int inputN, double inputResolution, double inputRadius)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)0);
            encoderSettings.Add("MaxVal", (double)9);
            encoderSettings.Add("Radius", (double)inputRadius);
            encoderSettings.Add("Resolution", (double)inputResolution);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);

            ScalarEncoder oldEncoderObj = new ScalarEncoder(encoderSettings);

            if ((inputN != 0) & (inputRadius != 0))
            {
                // If the value of N and Radius both are given then encoding should not be happening.
                // One value should be calculated based on another value
                // For the older version of encoder, defective configuration also leads to encoding.
                // One of their value is changed while encoding.
                List<int[]> encodedList = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    int[] encoded_data = oldEncoderObj.Encode(i);

                    // Adding the encoded data to the List
                    encodedList.Add(encoded_data);
                }

                // This step checks if encoding is done.
                // current version of encoder encodes data even when the configuration is not correct
                Assert.IsTrue(CheckDistinctArrayElement(encodedList));
            }


            else if ((inputN != 0) & (inputResolution != 0))
            {
                // If the value of N and Resolution both are given then encoding should not be happening.
                // One value should be calculated based on another value
                // For the older version of encoder, defective configuration also leads to encoding.
                // One of their value is changed while encoding.
                List<int[]> encodedList = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    int[] encoded_data = oldEncoderObj.Encode(i);

                    // Adding the encoded data to the List
                    encodedList.Add(encoded_data);
                }

                // This step checks if encoding is done.
                // current version of encoder encodes data even when the configuration is not correct
                Assert.IsTrue(encodedList.Count > 0);
            }

            else if ((inputResolution != 0) & (inputRadius != 0))
            {
                // If the value of Resolution and Radius both are given then encoding should not be happening.
                // One value should be calculated based on another value
                // For the older version of encoder, defective configuration also leads to encoding.
                // One of their value is changed while encoding.
                List<int[]> encodedList = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    int[] encoded_data = oldEncoderObj.Encode(i);

                    // Adding the encoded data to the List
                    encodedList.Add(encoded_data);
                }

                // This step checks if encoding is done.
                // current version of encoder encodes data even when the configuration is not correct
                Assert.IsTrue(encodedList.Count > 0);
            }

            else if ((inputN != 0) & (inputRadius != 0) & (inputResolution != 0))
            {
                // N, Radius and Resolution should be mutually exclusive parameters.
                // If all the values (N, Radius and Resolution) are provided to the ScalarEncoder for encoding,
                // exception should be thrown. Older version of encoder can encodes on such values and can also leads to exception sometimes
                List<int[]> encodedList = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    int[] encoded_data = oldEncoderObj.Encode(i);

                    // Adding the encoded data to the List
                    encodedList.Add(encoded_data);
                }

                // This step checks if encoding is done.
                // current version of encoder encodes data even when the configuration is not correct
                Assert.IsTrue(encodedList.Count > 0);
            }

            // This else block will run if N, Resolution and Radius are Mutually Exclusive
            else
            {
                // If any one of N, Radius or Resolution is provided such that they are mutually exclusive then ecoding is correctly done for correct value of N, Radius or Resolution
                List<int[]> encodedList = new List<int[]> { };

                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    int[] encoded_data = oldEncoderObj.Encode(i);

                    // Adding the encoded data to the List
                    encodedList.Add(encoded_data);
                }

                // current version of encoder encodes data distinctly if the values used of generating the encoder configuration is correct
                // If the value of parameters are not as required then encoding is not done
                Assert.IsTrue(CheckDistinctArrayElement(encodedList));

            }

        }


        // Testing if N, Resolution and Radius are mutually exclusive parameters
        // Only one of 3 of them should be set at a time, remaining two should be zero.
        // Based on anyone of their value, remaining two is calculated internally by the encoder.
        // Improved version of scalar encoder makes sure that N, Radius and Resolution are mutually exclusive.
        [TestMethod]
        [TestCategory("categori8")]
        // DataRow(N, Resolution, Radius)
        [DataRow(18, 0.3, 0)]
        [DataRow(18, 0, 4)]
        [DataRow(0, 0.3, 4)]
        [DataRow(18, 0.3, 4)]
        [DataRow(18, 0, 0)]
        [DataRow(0, 0.3, 0)]
        [DataRow(0, 0, 4)]
        public void TestingMutualExclusivenessOfParametersForImprovedEncoder(int inputN, double inputResolution, double inputRadius)
        {
            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 5);
            encoderSettings.Add("N", (int)inputN);
            encoderSettings.Add("MinVal", (double)0);
            encoderSettings.Add("MaxVal", (double)9);
            encoderSettings.Add("Radius", (double)inputRadius);
            encoderSettings.Add("Resolution", (double)inputResolution);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("Name", "TestScalarEncoderImproved");
            encoderSettings.Add("IsRealCortexModel", false);


            if ((inputN != 0) & (inputRadius != 0))
            {
                // If the value of N and Radius both are given then proper argument exception is generated on the new encoder.
                // One value should be calculated based on another value
                Assert.ThrowsException<ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
            }


            else if ((inputN != 0) & (inputResolution != 0))
            {
                // If the value of N and Resolution both are given then proper argument exception is generated on the new encoder.
                // One value should be calculated based on another value
                Assert.ThrowsException<ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
            }

            else if ((inputResolution != 0) & (inputRadius != 0))
            {
                // If the value of Resolution and Radius both are given then proper argument exception is generated on the new encoder.
                // One value should be calculated based on another value
                Assert.ThrowsException<ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
            }

            else if ((inputN != 0) & (inputRadius != 0) & (inputResolution != 0))
            {
                // N, Radius and Resolution should be mutually exclusive parameters.
                // If all the values (N, Radius and Resolution) are provided to the ScalarEncoder for encoding,
                // then Exception is generated properly.
                // Remaining two values should be calculated based on one value
                Assert.ThrowsException<ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
            }

            // This else block will run if N, Resolution and Radius are Mutually Exclusive
            else
            {
                ScalarEncoderImproved newEncoderObj = new ScalarEncoderImproved(encoderSettings);

                // List to keep the encoded data as a integer array
                List<int[]> encodedList = new List<int[]> { };

                for (int i = 0; i < 10; i++)
                {
                    int[] encoded_data = newEncoderObj.Encode(i);

                    // Adding the encoded data to the List
                    encodedList.Add(encoded_data);
                }

                // For a proper encoder settings, distinct encoding is generated by the improved encoder
                Assert.IsTrue(CheckDistinctArrayElement(encodedList));

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

