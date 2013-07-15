using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace TextPreprocessor
{
    class SpellChecker
    {

        private string[] tokens;
        public List<string> wrongSpelledWordsList { get; set; }
        public SpellChecker(string[] tokens)
        {
            this.tokens = tokens;
            wrongSpelledWordsList = new List<string>();
        }


        public void checkSpelling()
        {
            //object oMissing = System.Reflection.Missing.Value;
            
            //Word.Application oWord;
            //Word.Document oDoc;
            //oWord = new Word.Application();
            //oWord.Visible = false;
            //oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, false);

            //Word.Paragraph oPara1;

            //oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            
            //object doNotSaveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;

            foreach (string token in tokens)
            {
                
                //oPara1.Range.Text = token;
                //Word.ProofreadingErrors errors = oDoc.SpellingErrors;
                if (!Program.wordManager.CheckTokenForSpelling(token))
                {
                    wrongSpelledWordsList.Add(token);
                }

            }
            //oWord.Quit(ref doNotSaveChanges, ref oMissing, ref oMissing);
//            oWord.Quit(ref doNotSaveChanges, ref oMissing, ref oMissing);
            
        }
    }
}
