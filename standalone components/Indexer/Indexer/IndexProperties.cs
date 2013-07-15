using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    [Serializable]
    class IndexProperties
    {
        [Serializable]
        public struct TermDocument
        {
            public string term;
            public string document;
        }

        private Dictionary<string, int> termDictionary;
        private Dictionary<string, int> documentDictionary;
        private Dictionary<TermDocument, int> tfDictionary;
        private float[,] tfIdfArray;
        public float[] gfi, gi; 

        public IndexProperties()
        {
            termDictionary = new Dictionary<string, int>();
            documentDictionary = new Dictionary<string, int>();
            tfDictionary = new Dictionary<TermDocument, int>();
            
        }


        public void AddTermDocument(string term, string document)
        {
            if (!documentDictionary.ContainsKey(document))
            {
                documentDictionary.Add(document, documentDictionary.Count);
            }


            if (!termDictionary.ContainsKey(term))
            {
                termDictionary.Add(term, termDictionary.Count);
               
                TermDocument tempTermDocument;
                tempTermDocument.document = document;
                tempTermDocument.term = term;

                tfDictionary.Add(tempTermDocument, 1);
            }
            else
            {
                TermDocument tempTermDocument;
                tempTermDocument.term = term;
                tempTermDocument.document = document;
                if (tfDictionary.ContainsKey(tempTermDocument))
                {
                    
                    tfDictionary[tempTermDocument]++;
                }
                else
                {
                    tfDictionary.Add(tempTermDocument, 1);
                }
            }
        }


        public Dictionary<TermDocument, int> GetTfDictionary()
        {
            return tfDictionary;
        }

        public int GetNumOfTerms()
        {
            return termDictionary.Count;
        }

        public int GetNumOfDocuments()
        {
            return documentDictionary.Count;
        }


        public void SetTfIdfArray(float[,] tfIdfArray)
        {
            this.tfIdfArray = tfIdfArray;
        }

        public void SetGfiArray(float[] gfi)
        {
            this.gfi = gfi;
        }


        public void SetGiArray(float[] gi)
        {
            this.gi = gi;
        }


        public int GetTermIdentifier(string term)
        {
            return termDictionary[term];
        }

        public int GetDocumentIdentifier(string document)
        {
            return documentDictionary[document];
        }

        public bool TermExists(string term)
        {
            if (termDictionary.ContainsKey(term))
            {
                return true;
            }
            return false;
        }

    }
}
