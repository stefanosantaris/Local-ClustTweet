using Indexer.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using TermDocument = Indexer.IndexProperties.TermDocument;

namespace Indexer
{
    class IndexHandler
    {
        private IndexProperties properties;
        public IndexHandler()
        {
            IntializeProperties();
        }

        private void IntializeProperties()
        {
            if (!File.Exists("IndexProperties.dat"))
            {
                properties = new IndexProperties();
            }
            else
            {
                FileStream fs = new FileStream("IndexProperties.dat", FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    properties = (IndexProperties)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason : " + e.Message);
                    properties = new IndexProperties();
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        public void IndexTweet(ParsedTweet tweet)
        {
            string text = tweet.text;
            string[] tokenizedText = text.Split(' ');
            foreach (string token in tokenizedText)
            {
                properties.AddTermDocument(token, tweet.id);
            }
        }


        public void ImplementTfIdfArray()
        {
            int numOfTerms = properties.GetNumOfTerms();
            int numOfDocuments = properties.GetNumOfDocuments();
            float[,] tfArray = new float[numOfTerms, numOfDocuments];
            Dictionary<TermDocument, int> tfIdfDictionary = properties.GetTfDictionary();
            foreach (KeyValuePair<TermDocument, int> tfValuePair in tfIdfDictionary)
            {
                TermDocument termDocumentStruct = tfValuePair.Key;
                int row = properties.GetTermIdentifier(termDocumentStruct.term);
                int column = properties.GetDocumentIdentifier(termDocumentStruct.document);
                int value = tfValuePair.Value;
                
                tfArray[row, column] += value;
                

            }
            float[] gfi = CalculateGfi(tfArray);
            float[] gi = CalculateGi(tfArray, gfi);
            tfArray = NormalizeTfArray(tfArray, gi);


            properties.SetTfIdfArray(tfArray);
            properties.SetGfiArray(gfi);
            properties.SetGiArray(gi);

        }

        private float[,] NormalizeTfArray(float[,] tfArray, float[] gi)
        {
            for (int i = 0; i < properties.GetNumOfTerms(); i++)
            {
                for (int j = 0; j < properties.GetNumOfDocuments(); j++)
                {
                    tfArray[i, j] = (float)(gi[i] * Math.Log(tfArray[i, j] + 1));
                }
            }
            return tfArray;
        }

        private float[] CalculateGi(float[,] tfArray, float[] gfi)
        {
            float[] gi = new float[properties.GetNumOfTerms()];
            for (int i = 0; i < properties.GetNumOfTerms(); i++)
            {
                float sum = 0;
                for (int j = 0; j < properties.GetNumOfDocuments(); j++)
                {
                    double pi = tfArray[i, j] / gfi[i];
                    float log = 0;
                    if (pi != 0)
                    {
                        log = (float)((pi * Math.Log(pi) ) / Math.Log(properties.GetNumOfDocuments()));
                    }
                    sum += log;
                }
                gi[i] = 1 + sum;
            }

            return gi;
        }

        private float[] CalculateGfi(float[,] tfArray)
        {
            float[] gfi = new float[properties.GetNumOfTerms()];
            for (int i = 0; i < properties.GetNumOfTerms(); i++)
            {
                float sum = 0;
                for (int j = 0; j < properties.GetNumOfDocuments(); j++)
                {
                    sum += tfArray[i, j];
                }
                gfi[i] = sum;
            }
            return gfi;
        }


        public void SaveProperties()
        {
            FileStream fs = new FileStream("IndexProperties.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, properties);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason : " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public ItemRepresentation GetItemRepresentation(ParsedTweet tweet)
        {
            
            
            float[] vectorValue = new float[properties.GetNumOfTerms()];


            string text = tweet.text;
            string[] tokenizedText = text.Split(' ');
            foreach (string token in tokenizedText)
            {
                if (properties.TermExists(token))
                {
                    vectorValue[properties.GetTermIdentifier(token)] += 1;
                }
            }

            for (int i = 0; i < properties.GetNumOfTerms(); i++)
            {
                vectorValue[i] = (float)(properties.gi[i] * Math.Log(vectorValue[i] + 1));
            }

            ItemRepresentation item = new ItemRepresentation(tweet.id, vectorValue, true);

            return item;
        }
    }
}
