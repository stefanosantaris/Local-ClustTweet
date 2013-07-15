using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextPreprocessor
{
    class Tokenizer
    {
        public static string[] getTokens(string text, StopWordHandler stopWordHandler)
        {
            Regex oRegex = new Regex("([ \\t{}():;. \n])");
            text = text.ToLower();

            string[] words = oRegex.Split(text);
            ArrayList tokensArrayList = new ArrayList();

            for (int i = 0; i < words.Length; i++)
            {
                MatchCollection mc = oRegex.Matches(words[i]);
                if (mc.Count <= 0 && words[i].Trim().Length > 0)
                {
                    if (!stopWordHandler.isStopWord(words[i]))
                    {
                        tokensArrayList.Add(words[i]);
                    }
                }
            }


            // Cleaning up the extra characters after tokenization.
            char[] bothsidestrimchar = { '\'', '<', '>', '/', ':', ';', '"', '{', '}', '|', '\\', '[', ']', '.', ',', '~', '`', '!', '?', '@', '#', '%', '^', '&', '*', '(', ')', '_', '-', '+', '=' };
            char[] endtrimchar = { '$' };
            for (int i = 0; i < tokensArrayList.Count; i++)
            {
                string sObj = (tokensArrayList[i] as string);
                sObj = sObj.Trim();
                sObj = sObj.Trim(bothsidestrimchar);
                sObj = sObj.TrimEnd(endtrimchar);
                tokensArrayList[i] = sObj;
            }

            //int arr_cnt = 0;
            string[] tokensArray = tokensArrayList.ToArray(typeof(string)) as string[];
            return tokensArray;
            //for (int i = 0; i < tokensArrayList.Count; i++) if (((string)tokensArrayList[i]).Trim().Length > 0) arr_cnt++;
            //string[] oArray = new string[arr_cnt];
            //for (int i = 0, j = 0; i < oArraylist.Count; i++)
            //{
            //    if (((string)oArraylist[i]).Trim().Length > 0)
            //    {
            //        oArray[j] = (string)oArraylist[i];
            //        j++;
            //    }
            //}
            //return oArray;


        }
    }
}
