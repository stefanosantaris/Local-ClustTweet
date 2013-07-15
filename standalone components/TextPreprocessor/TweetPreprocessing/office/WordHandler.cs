using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace TextPreprocessor.office
{
    class WordHandler
    {
        private Word.Application oWord;
        private Word.Document oDoc;
        private Word.Paragraph oPara1;
        private object doNotSaveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
        private object oMissing = System.Reflection.Missing.Value;
        public WordHandler()
        {
            InitializeWord();
        }

        private void InitializeWord()
        {
            

            oWord = new Word.Application();
            oWord.Visible = false;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, false);
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);

        }


        public bool CheckTokenForSpelling(string token)
        {
            oPara1.Range.Text = token;
            Word.ProofreadingErrors errors = oDoc.SpellingErrors;
            if(errors.Count > 0) 
                return false;
                
            return true;
        }


        public void CloseWord()
        {
            oWord.Quit(ref doNotSaveChanges, ref oMissing, ref oMissing);
        }

    }
}
