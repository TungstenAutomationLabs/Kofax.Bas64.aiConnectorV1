﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Kofax.Base64ConnectorV1
{
    /// <summary>
    /// Base64 Connector v1.0 by Kofax Labs.
    /// This class can be added to KTA to help and communicate with Base64 and their API.
    /// </summary>
    public class Base64Connector
    {
        private const string Base64Uri = "https://base64.ai/api";

        #region "Public Methods"

        /// <summary>
        /// Queries Base64.ai getting all fields from the document, returns a simple JSON with the key/value pairs.
        /// </summary>
        /// <param name="Base64FieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the Base64GetExtractionResult method.</param>
        /// <returns>Concatenated text results.</returns>
        public string Base64GetExtractionResult(string ktaDocID, string docFileExtension, string ktaSDKUrl, string ktaSessionID, string Base64Token)
        {
            string json = Base64ScanFile(ktaDocID, docFileExtension, ktaSDKUrl, ktaSessionID, Base64Token);
            string[,] array =  ExtractFields(json);
            return JsonBuilder.BuildJson(array);
        }

        /// <summary>
        /// Queries Base64.ai getting all fields from the document, returns a simple JSON with the key/value pairs.
        /// </summary>
        /// <param name="Base64FieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the Base64GetExtractionResult method.</param>
        /// <returns>Concatenated text results.</returns>
        public string Base64GetExtractionResultFromFile(string filepath, string docFileExtension, string Base64Token)
        {
            string json = Base64ScanFileFromFile(filepath, docFileExtension, Base64Token);
            string[,] array = ExtractFields(json);
            return JsonBuilder.BuildJson(array);
        }

        /// <summary>
        /// Gets the  simple JSON with the key/value pairs.
        /// </summary>
        /// <param name="Base64FieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the Base64GetExtractionResult method.</param>
        /// <returns>Concatenated text results.</returns>
        public string GetFieldValueFromBase64Extraction(string jsonExtraction, string fieldName, string Base64Token)
        {
            //string json = Base64ScanFileFromFile(filepath, docFileExtension, Base64Token);
            //string[,] array = ExtractFields(json);
            //return JsonBuilder.BuildJson(array);
            return "";
        }

        #endregion

        #region "Private Methods"

        /// <summary>
        /// Uploads a KTA PDF file to Base64
        /// </summary>
        /// <param name="docID">KTA Document ID.</param>
        /// <param name="ktaSDKUrl">Complete URL for KTA's SDK instance.</param>
        /// <param name="sessionID">KTA Session ID.</param>
        /// <param name="Base64Token">Token provided by Base64; please include 'Bearer' before it.</param>
        /// <returns>Base64 File ID.</returns>
        private string Base64ScanFile(string docID, string docFileExtension, string ktaSDKUrl, string ktaSessionID, string Base64Token)
        {
            if (docID.Trim().Length == 0)
            {
                throw new Exception("KTA Doc ID needs to be populated");
            }

            if (Base64Token.Trim().Length == 0)
            {
                throw new Exception("Base64Token needs to be populated");
            }

            if ((ktaSDKUrl.Trim().Length == 0) || (ktaSessionID.Trim().Length == 0))
            {
                throw new Exception("KTA information needs to be populated");
            }

            string text = "Nothing read";
            byte[] fileBytes = GetKTADocumentFile(docID, docFileExtension, ktaSDKUrl, ktaSessionID);
            string fileBase64 = FileToBase64Encoder.EncodeFileToBase64(fileBytes);
            // Get the file's MIME type
            string mimeType = FileToBase64Encoder.GetMimeType(docFileExtension, fileBytes);
            // Construct the JSON body
            string requestBody = $"{{\"document\":\"data:{mimeType};base64,{fileBase64}\"}}";
            // Convert the JSON data to bytes
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(requestBody);

            //Now let's send the file to Base64
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Base64Uri + "/scan");
            httpWebRequest.Headers.Add("Authorization", Base64Token);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentLength = jsonDataBytes.Length;
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            // Write the JSON data to the request stream
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                requestStream.Close();
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }

        /// <summary>
        /// Uploads a KTA PDF file to Base64
        /// </summary>
        /// <param name="docID">KTA Document ID.</param>
        /// <param name="ktaSDKUrl">Complete URL for KTA's SDK instance.</param>
        /// <param name="sessionID">KTA Session ID.</param>
        /// <param name="Base64Token">Token provided by Base64; please include 'Bearer' before it.</param>
        /// <returns>Base64 File ID.</returns>
        private string Base64ScanFileFromFile(string filepath, string docFileExtension, string Base64Token)
        {

            string text = "Nothing read";
            //byte[] fileBytes = GetKTADocumentFile(docID, docFileExtension, ktaSDKUrl, ktaSessionID);
            string fileBase64 = FileToBase64Encoder.EncodeFileToBase64(filepath);
            // Get the file's MIME type
            string mimeType = FileToBase64Encoder.GetMimeType(docFileExtension, null);
            // Construct the JSON body
            string requestBody = $"{{\"document\":\"data:{mimeType};base64,{fileBase64}\"}}";
            // Convert the JSON data to bytes
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(requestBody);

            //Now let's send the file to Base64
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Base64Uri + "/scan");
            httpWebRequest.Headers.Add("Authorization", Base64Token);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentLength = jsonDataBytes.Length;
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            // Write the JSON data to the request stream
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                requestStream.Close();
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }

        private byte[] GetKTADocumentFile(string docID, string fileType, string ktaSDKUrl, string sessionID)
        {
            byte[] result = new byte[1];
            byte[] buffer = new byte[4096];
            string status = "OK";

            try
            {

                //Setting the URi and calling the get document API
                var KTAGetDocumentFile = ktaSDKUrl + "/CaptureDocumentService.svc/json/GetDocumentFile";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(KTAGetDocumentFile);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                // CONSTRUCT JSON Payload
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"sessionId\":\"" + sessionID + "\",\"documentId\":\"" + docID + "\",\"fileType\":\"" + fileType.ToLower() + "\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream receiveStream = httpWebResponse.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(receiveStream, encode);
                int streamContentLength = unchecked((int)httpWebResponse.ContentLength);

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);

                        } while (count != 0);

                        result = memoryStream.ToArray();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                status = "An error occured: " + ex.ToString();
                return result;
            }

        }


        private static string GetJSONPropertyValue(string jsonString, string propertyName)
        {
            JToken root = JToken.Parse(jsonString);

            if (root.Type == JTokenType.Array)
            {
                foreach (JToken item in root.Children())
                {
                    string result = GetJSONPropertyValue(item.ToString(), propertyName);
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }
            }
            else if (root.Type == JTokenType.Object)
            {
                JToken propertyValue = root[propertyName];
                if (propertyValue != null)
                    return propertyValue.ToString();

                foreach (JProperty property in root.Children<JProperty>())
                {
                    string result = GetJSONPropertyValue(property.Value.ToString(), propertyName);
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }
            }

            return string.Empty; // Property not found, return an empty string or throw an exception as needed
        }

        //public string[][] ExtractFields(string json)
        //{
        //    using var doc = JsonDocument.Parse(json);
        //    var root = doc.RootElement;

        //    if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
        //    {
        //        throw new Exception("Expected JSON array at the root.");
        //    }

        //    var fieldsElement = root[0].GetProperty("fields");
        //    var keyValues = new List<string[]>();

        //    foreach (var field in fieldsElement.EnumerateObject())
        //    {
        //        string key = field.Value.GetProperty("key").GetString();
        //        string value;

        //        var valueType = field.Value.GetProperty("value").ValueKind;

        //        if (valueType == JsonValueKind.String)
        //        {
        //            value = field.Value.GetProperty("value").GetString();
        //        }
        //        else
        //        {
        //            value = field.Value.GetProperty("value").ToString();
        //        }

        //        keyValues.Add(new string[] { key, value });
        //    }

        //    return keyValues.ToArray();
        //}

        /// <summary>
        /// Concatenates all text extracted from the results for the specified Field ID.
        /// </summary>
        /// <param name="zuvaFieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the ZuvaGetExtractionResult method.</param>
        /// <returns>Concatenated text results.</returns>
        private string[,] ExtractFields(string json)
        {

            // Parse the JSON using Newtonsoft.Json
            JArray jsonArray = JArray.Parse(json);

            if (jsonArray != null)
            {
                var resultList = new List<string[]>();

                foreach (JToken jsonElement in jsonArray)
                {
                    var model = jsonElement["model"];
                    resultList.Add(new string[] { "Model", model["name"].ToString(), model["confidence"].ToString() });
                    
                    JObject fields = jsonElement["fields"].ToObject<JObject>();

                    foreach (JProperty field in fields.Properties())
                    {
                        string key = field.Value["key"].ToString();
                        string value = field.Value["value"].ToString();
                        string confidence = field.Value["confidence"].ToString();
                        resultList.Add(new string[] { key, value, confidence });
                    }

                    string[,] resultArray = new string[resultList.Count, 3];
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        resultArray[i, 0] = resultList[i][0];
                        resultArray[i, 1] = resultList[i][1];
                        resultArray[i, 2] = resultList[i][2];
                    }

                    return resultArray;
                }
            }
            return null;
        }


        private static string EscapeFieldList(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Check if the string is already formatted
            if (input.StartsWith("\"") && input.EndsWith("\""))
                return input;

            // Split the string by commas
            string[] parts = input.Split(',');
            string[] modifiedParts = parts.Select(part => part.Replace("\"", "").Trim()).ToArray();

            // Escape each part and join them with commas
            string formattedString = string.Join(",", modifiedParts.Select(part => $"\"{part.Trim()}\""));

            return formattedString;
        }

        #endregion
    }
}
