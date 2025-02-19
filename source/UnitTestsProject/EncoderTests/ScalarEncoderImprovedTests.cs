﻿// Copyright (c) Damir Dobric. All rights reserved.
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
    /// Test class that compares the encoding behaviour of Improved and UnImproved Scalar Encoder for similar settings.
    /// </summary>
    [TestClass]
    public class ScalarEncoderImprovedTests
    {


        # region Helper Function initialized with option arguments to return EncoderSettings Dictionary object

        /// <summary>
        /// Function to set encoder settings as a dictionary.
        /// This function is provided with named arguments and optional arguments.
        /// </summary>
        /// <param name="W"></param>
        /// <param name="MinVal"></param>
        /// <param name="MaxVal"></param>
        /// <param name="N"></param>
        /// <param name="Radius"></param>
        /// <param name="Resolution"></param>
        /// <param name="Periodic"></param>
        /// <param name="ClipInput"></param>
        /// <param name="Name"></param>
        /// <param name="IsRealCortexModel"></param>
        /// <returns>Dictionary object of Encoder Settings</returns>
        public static Dictionary<string, object> GetEncoderSettings
            (
                int W,
                double MinVal,
                double MaxVal,
                int N = 0,
                double Radius = 0,
                double Resolution = 0,
                bool Periodic = false,
                bool ClipInput = true,
                string Name = "TestScalarEncoderImproved",
                bool IsRealCortexModel = false
            )
        {

            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", W);
            encoderSettings.Add("N", N);
            encoderSettings.Add("MinVal", MinVal);
            encoderSettings.Add("MaxVal", MaxVal);
            encoderSettings.Add("Radius", Radius);
            encoderSettings.Add("Resolution", Resolution);
            encoderSettings.Add("Periodic", Periodic);
            encoderSettings.Add("ClipInput", ClipInput);
            encoderSettings.Add("Name", Name);
            encoderSettings.Add("IsRealCortexModel", IsRealCortexModel);


            return encoderSettings;

        }


        #endregion


        #region Helper function to check if the array of encoded data are distinct
        /// <summary>
        /// Function to check if a List has unique integer array elements.
        /// Array of encoded data from the Scalar Encoder is passed to this function.
        /// This function makes sure that encodings are distinct from a given Scalar Encoder.
        /// </summary>
        /// <param name="resultArray">List of integer arrays that consists of encoded datas</param>
        /// <returns>Boolean: True if the list of encoded datas passed to this function is unique.</returns>
        public static Boolean CheckDistinctArrayElement(List<int[]> resultArray)
        {
            bool isDistinctElement = true;

            for (int i = 0; i < resultArray.Count; i++)
            {
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
                        if (match == resultArray[0].Length)
                        {
                            isDistinctElement = false;
                        }
                    }
                }

            }
            return isDistinctElement;

        }

        #endregion


        #region Test Encoding for different value of Total Bits for UnImproved Encoder - Non-Periodic

        // Encoding is tested for improved encoder with respect to different value of Total bits (N).
        // Value of N which leads to non-distinct encoding is also used and non-distinct encoding is
        // output by the old scalar encoder.
        // This test case is for non-periodic inputs. (i.e., Periodic is False in encoderSettings)
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
        public void TestEncodingByUnImprovedEncoderProvidedTotalBitsForNonPeriodic(int inputN)
        {

            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 5,
                    MinVal: 22,
                    MaxVal: 39,
                    N: inputN,
                    Periodic: false
                );

            ScalarEncoder UnImprovedEncoderObj = new ScalarEncoder(encoderSettings);

            // This value refers to the total different encoding that a ScalarEncoder can encode.
            // If the range of data that we need to encode is more than the total different encoding that our ScalarEncoder can encode then
            // encodings are not distinct.
            int requiredTotalBits = (int)((int)encoderSettings["W"] + (double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]); // The way how requiredTotalBits is calculated is different
                                                                                                                                              // in Periodic and Non-Periodic encoding. This logic is implemented
                                                                                                                                              // inside the newer version of ScalarEncoder to provide distinct encoding.

            if ((int)encoderSettings["N"] < requiredTotalBits)
            {
                // List to append the encoded data from the scalar encoder when the value of N is too Low
                List<int[]> encodedListForLowTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"] + 1; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);
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
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughTotalBits.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the old version of scalar encoder
                // provides unique encoding if the value of N is equal to or more than a threshold.

                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughTotalBits));
            }

        }

        #endregion


        #region Test Encoding for different value of Total Bits for Improved Encoder - Non-Periodic

        // Encoding is tested for improved encoder with respect to different value of Total bits (N).
        // Value of N which leads to non-distinct encoding is thrown as an exception.
        // This test case is for non-periodic inputs. (i.e., Periodic is False in encoderSettings)
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
        public void TestEncodingByImprovedEncoderProvidedTotalBitsForNonPeriodic(int inputN)
        {
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 5,
                    MinVal: 22,
                    MaxVal: 39,
                    N: inputN,
                    Periodic: false
                );



            // This value refers to the total different encoding that a ScalarEncoder can encode.
            // If the range of data that we need to encode is more than the total different encoding that our ScalarEncoder can encode then
            // encodings are not distinct.
            int requiredTotalBits = (int)((int)encoderSettings["W"] + (double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]);    // The way how requiredTotalBits is calculated is different
                                                                                                                                                 // in Periodic and Non-Periodic inputs. This logic is implemented
                                                                                                                                                 // inside the newer version of ScalarEncoder to provide distinct encoding.

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

        #endregion


        #region Test Encoding for different value of Total Bits for UnImproved Encoder - Periodic

        // Encoding is tested for improved encoder with respect to different value of Total bits (N).
        // Value of N which leads to non-distinct encoding is also used and non-distinct encoding is
        // output by the old scalar encoder.
        // This test case is for periodic inputs. (i.e., Periodic is True in encoderSettings)
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
        public void TestEncodingByUnImprovedEncoderProvidedTotalBitsForPeriodic(int inputN)
        {
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 5,
                    MinVal: 22,
                    MaxVal: 39,
                    N: inputN,
                    Periodic: true
                );

            ScalarEncoder UnImprovedEncoderObj = new ScalarEncoder(encoderSettings);

            // This value refers to the total distinct encoding that a ScalarEncoder can encode for periodic input.
            // If the range of data that we need to encode is more than the total different encodings that our ScalarEncoder can encode then
            // encodings are not distinct.
            // For periodic input, the way in which required total bits is calculated is different.
            int requiredTotalBits = (int)((double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]); // The way how requiredTotalBits is calculated is different
                                                                                                                  // in Periodic and Non-Periodic inputs. This logic is implemented
                                                                                                                  // inside the newer version of ScalarEncoder to provide distinct encoding.

            if ((int)encoderSettings["N"] < requiredTotalBits)
            {
                // List to append the encoded data from the scalar encoder when the value of N is too Low
                List<int[]> encodedListForLowTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to a step before the maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                // Input should be strictly less than the MaxVal for Periodic encoding
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"]; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);
                    // Adding the encoded data to an List for comparision
                    encodedListForLowTotalBits.Add(encoded_data);
                }

                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with false as the old version of scalar encoder
                // does not provides unique encoding.   
                Assert.IsFalse(CheckDistinctArrayElement(encodedListForLowTotalBits));

            }
            else
            {
                // List to append the encoded data from the scalar encoder when the value of N is enough for distinct encoding
                List<int[]> encodedListForEnoughTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"]; i++)
                {
                    // Getting the encoding of data
                    int[] encoded_data = UnImprovedEncoderObj.Encode(i);

                    // Adding the encoded data to an List
                    encodedListForEnoughTotalBits.Add(encoded_data);
                }
                // Checks if the List of integer array consists of unique elements.
                // Assertion is done with true as the old version of scalar encoder
                // provides unique encoding if the value of N is equal to or more than a threshold (requiredTotalBits).

                Assert.IsTrue(CheckDistinctArrayElement(encodedListForEnoughTotalBits));
            }

        }

        #endregion


        #region Test Encoding for different value of Total Bits for Improved Encoder - Periodic


        // Encoding is tested for improved encoder with respect to different value of Total bits (N).
        // Value of N which leads to non-distinct encoding is thrown as an exception.
        // This test case is for periodic inputs. (i.e., Periodic is True in encoderSettings)
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
        public void TestEncodingByImprovedEncoderProvidedTotalBitsForPeriodic(int inputN)
        {
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 5,
                    MinVal: 22,
                    MaxVal: 39,
                    N: inputN,
                    Periodic: true
                );



            // This value refers to the total different encoding that a ScalarEncoder can encode.
            // If the range of data that we need to encode is more than the total different encoding that our ScalarEncoder can encode then
            // encodings are not distinct.

            // The way in which requiredTotalBits is calculated is different for Periodic encoding
            int requiredTotalBits = (int)((double)encoderSettings["MaxVal"] - (double)encoderSettings["MinVal"]);   // The way how requiredTotalBits is calculated is different
                                                                                                                    // in Periodic and Non-Periodic inputs. This logic is implemented
                                                                                                                    // inside the newer version of ScalarEncoder to provide distinct encoding.

            if ((int)encoderSettings["N"] < requiredTotalBits)
            {
                // Looping from  minimum-value that a encoder can encode to a step before maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                // For periodic input, input data for encoder should be strictly less than MaximumValue
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"]; i++)
                {
                    // If the value of N is too low, 
                    // the Improved version of ScalarEncoder will generate exception.
                    // Exception is thrown while initializing the encoder with the encoder settings,
                    // this prevents un-necessary steps.
                    // This makes sure that only distinct encoding passes
                    Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));
                }

            }
            // If the value of N is more than or equal to a required value then encoding is distinct by the improved scalar encoder
            else
            {

                // Initializing the encoder object
                ScalarEncoderImproved ImprovedEncoderObj = new ScalarEncoderImproved(encoderSettings);

                // List to append the encoded data from the scalar encoder when the value of N is enough for distinct encoding
                List<int[]> encodedListForEnoughTotalBits = new List<int[]> { };

                // Looping from  minimum-value that a encoder can encode to maximum value.
                // Minimum Value and Maximum Value are used from encoder settings.
                for (double i = (double)encoderSettings["MinVal"]; i < (double)encoderSettings["MaxVal"]; i++)
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



        #endregion


        #region Test Encoding for different value of Resolution for UnImproved Encoder

        // Test function to check encoding when Resolution is set(i.e., N and Radius unset or zero).
        // Any value of Resolution that leads to non-distinct encoding is output by the old scalar encoder.
        // For some configuration of input space, IndexOutOfRangeException is observed for unimproved encoder.
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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

            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 7,
                    MinVal: 2,
                    MaxVal: 39,
                    Resolution: inputResolution
                );

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

        #endregion


        #region Test Encoding for different value of Resolution for Improved Encoder

        // Test function to check encoding when Resolution is set(i.e., N and Radius are unset or zero).
        // Any value of Resolution that leads to non-distinct encoding is thrown as an exception.
        // Encoding can only be done by those value of Resolution that make distinct encodings.
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 7,
                    MinVal: 2,
                    MaxVal: 39,
                    Resolution: inputResolution
                );


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

        #endregion


        #region Test Encoding for different value of Radius for UnImproved Encoder


        // Test function to check encoding when Radius is set(i.e., N and Resolution are unset or zero).
        // Any value of radius that leads to non-distinct encoding is also encoded.
        // For some input space, some value leads to IndexOutofRange exception for UnImproved Scalar Encoder.
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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

            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 9,
                    MinVal: 115,
                    MaxVal: 159,
                    Radius: inputRadius
                );

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
            else if (((double)encoderSettings["Radius"] <= requiredRadius) & ((double)encoderSettings["Radius"] != 0.7) & ((double)encoderSettings["Radius"] != 24.0) & ((double)encoderSettings["Radius"] != 25.0))
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

        #endregion


        #region Test Encoding for different value of Radius for Improved Encoder


        // Test function to check encoding when Radius is set(i.e., N and Resolution are unset or zero).
        // Any value of radius that leads to non-distinct encoding is thrown as an exception.
        // Encoding can only be done by those value of Radius that make distinct encodings.
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 9,
                    MinVal: 115,
                    MaxVal: 159,
                    Radius: inputRadius
                );


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

        #endregion


        #region Test Mutually Exclusiveness of Parameters N, Radius and Resolution for UnImproved Encoder

        // Testing if N, Resolution and Radius are mutually exclusive parameters
        // Only one of 3 of them should be set at a time, remaining two should be zero.
        // Based on anyone of their value, remaining two is calculated internally by the encoder.
        // Older version of scalar encoder encodes even when all or any two of these parameters are given. 
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        // DataRow(N, Resolution, Radius)
        [DataRow(18, 0.3, 0)]
        [DataRow(18, 0, 4)]
        [DataRow(0, 0.3, 4)]
        [DataRow(18, 0.3, 4)]
        [DataRow(18, 0, 0)]
        [DataRow(0, 0.3, 0)]
        [DataRow(0, 0, 4)]
        public void TestingMutualExclusivenessOfParametersForUnImprovedEncoder(int inputN, double inputResolution, double inputRadius)
        {
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 5,
                    MinVal: 0,
                    MaxVal: 9,
                    N: inputN,
                    Resolution: inputResolution,
                    Radius: inputRadius
                );

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

        #endregion


        #region Test Mutually Exclusiveness of Parameters N, Radius and Resolution for Improved Encoder

        // Testing if N, Resolution and Radius are mutually exclusive parameters
        // Only one of 3 of them should be set at a time, remaining two should be zero.
        // Based on anyone of their value, remaining two is calculated internally by the encoder.
        // Improved version of scalar encoder makes sure that N, Radius and Resolution are mutually exclusive.
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
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
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 5,
                    MinVal: 0,
                    MaxVal: 9,
                    N: inputN,
                    Resolution: inputResolution,
                    Radius: inputRadius
                );


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

        #endregion


        #region Test Case to show how initializing the default value of Radius  to negative leads to OverFlowException in UnImproved ScalarEncoder

        /// <summary>
        /// Test Case to show how default value initialized in EncoderBase.cs file with Radius: -1 leads to System.OverFlowException in OldScalarEncoder.
        /// This is caused because old version is not handling the mutually exclusiveness properties of N, Radius and Resolution.
        /// (Non-Deterministic behaviour of program is observed.)
        /// </summary>
        /// <param name="input"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImproved")]
        [DataRow(3)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(8)]
        public void TestingForNegativeValueOfRadiusForUnImprovedEncoder(int input)
        {
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 3,
                    MinVal: 0,
                    MaxVal: 9,
                    Resolution: 1,
                    Radius: -1
                );

            ScalarEncoder encoderObj = new ScalarEncoder(encoderSettings);


            // Assertion for the value of N to be Negative
            // This is caused because old encoder doesnot makes N, Radius or Resolution mutually exclusive
            // Default value of Radius was set negative inside Encoder base file
            // This value caused the value of TotalBits to cause Negative.
            // This results in OverFlowException.
            Assert.IsTrue(encoderObj.N < 0);
            // In case of old scalar encoder, OverflowException is thrown
            Assert.ThrowsException<System.OverflowException>(() => encoderObj.Encode(input));
        }

        #endregion


        #region Test Case to show how initializing the default value of Radius to negative leads to ArgumentException in Improved ScalarEncoder


        /// <summary>
        /// Test Case to show how default value initialized in EncoderBase.cs file with Radius: -1 leads to System.ArgumentException in New ScalarEncoder.
        /// This is caused because new version is handling the mutually exclusiveness properties of N, Radius and Resolution.
        /// The flow of program is controlled(Deterministic behaviour of program is observed.)
        /// </summary>
        /// <param name="input"></param>
        [TestMethod]
        [TestCategory("ScalarEncoderImprovedgi")]
        [DataRow(3)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(8)]
        public void TestingForNegativeValueOfRadiusForImprovedEncoder(int input)
        {
            // Getting the encoder settings from the GetEncoderSettings Helper function
            Dictionary<string, object> encoderSettings =

                GetEncoderSettings
                (
                    W: 3,
                    MinVal: 0,
                    MaxVal: 9,
                    Resolution: 1,
                    Radius: -1
                );

            // In the improved version of ScalarEncoder, Argument exception is checked making N, Radius and Resolution Mutually Exclusive
            // Instead of trying to encode for incorrect configuration, argument exception is thrown while initializing the new scalar encoder object
            Assert.ThrowsException<System.ArgumentException>(() => new ScalarEncoderImproved(encoderSettings));

        }

        #endregion




    }
}

        
