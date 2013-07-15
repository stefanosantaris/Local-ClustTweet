using TextPreprocessor.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPreprocessor
{
    class TweetPreprocessor
    {
        public ParsedTweet tweet { get; set; }
        private int numOfErrors = 0;
        private int numOfTokens = 0;
        private StopWordHandler stopWordsManager;
        public TweetPreprocessor(ParsedTweet tweet)
        {
            this.tweet = tweet;
            stopWordsManager = new StopWordHandler();
        }

        public void PreprocessTweet()
        {
            //Tokenize the tweet's Text and handle the tokens into a string array
            string[] tokens = Tokenizer.getTokens(tweet.text, stopWordsManager);
            numOfTokens = tokens.Length;

            //Check for spelling errors
            SpellChecker spelling = new SpellChecker(tokens);
            spelling.checkSpelling();
            numOfErrors = spelling.wrongSpelledWordsList.Count;


            //If there are errors check whether we have slangs in them and replace them with their translations
            if (numOfErrors != 0)
            {
                SlangChecker slangHandler = new SlangChecker();
                slangHandler.checkForSlangs(tokens, spelling.wrongSpelledWordsList);
                numOfErrors -= slangHandler.numOfSlangs;
            }

            float errorRate = (float)numOfErrors / numOfTokens;
            if (errorRate > 0.4)
            {
                tweet = null;
                return;
            }
             
            string processedTokensString = String.Join(" ", tokens);
            //Retokenize processedTokensString
            tokens = Tokenizer.getTokens(processedTokensString, stopWordsManager);

            //Stem Tokens
            PorterStemmer stemmer = new PorterStemmer();
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = stemmer.stemTerm(tokens[i]);
            }

            //Join the tokens array into a final string
            string processedText = String.Join(" ", tokens);


            tweet.text = processedText;


        }
       
    }
}
