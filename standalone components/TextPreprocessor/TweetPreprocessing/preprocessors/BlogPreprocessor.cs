using TextPreprocessor.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPreprocessor
{
    class BlogPreprocessor
    {
        public ParsedBlog[] blogs { get; set; }
        private ArrayList blogList;
        private int numOfErrors = 0;
        private int numOfTokens = 0;
        private StopWordHandler stopWordsManager;

        public BlogPreprocessor(ParsedBlog[] blogs)
        {
            this.blogs = blogs;
            stopWordsManager = new StopWordHandler();
            blogList = new ArrayList();
        }

        public void PreprocessBlogs()
        {
            foreach (ParsedBlog blog in blogs)
            {
                //Tokenize the blog's Text and handle the tokens into a string array
                string[] tokens = Tokenizer.getTokens(blog.text, stopWordsManager);
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

                //Calculate the error rate and continue whether the rate is below the limit
                float errorRate = (float)numOfErrors / numOfTokens;
                if (errorRate > 0.4)
                    continue;
                


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

                blog.text = processedText;

                blogList.Add(blog);
            }

            blogs = blogList.ToArray(typeof(ParsedBlog)) as ParsedBlog[];
        }
    }
}
