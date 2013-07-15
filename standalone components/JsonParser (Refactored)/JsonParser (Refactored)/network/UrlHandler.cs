using HtmlAgilityPack;
using JsonParser.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace JsonParser.network
{
    class UrlHandler
    {
        private JArray urlsArray { get; set; }
        public List<ParsedBlog> blogList { get; set; }
        public string text { get; set; }
        private int retryCounter = 0;
       

        public UrlHandler(JArray urlsArray, string text)
        {
            this.urlsArray = urlsArray;
            this.text = text;
            blogList = new List<ParsedBlog>();
        }

        public void processUrls()
        {
            //process every url
            foreach (JObject jUrl in urlsArray)
            {
                //get the expanded url if it already exists
                string url = (string)jUrl.SelectToken("expanded_url");

                string dUrl = (string)jUrl.SelectToken("url");
                string expUrl = string.Empty;


                //if the expanded url contains a zip file continue without any processing
                if (url.Contains(".zip"))
                {
                    text = text.Replace(dUrl, "");
                    continue;
                }

                
                

                if (url == null)
                {   
                    url = dUrl;
                }

                //strip paranthesis
                url = url.Replace(")", "");
                dUrl = dUrl.Replace(")", "");
                url = url.Replace("(", "");
                dUrl = dUrl.Replace("(", "");
                

                //Expand the url
                expUrl = expandShortUrl(url, false);
                expUrl = expUrl.Replace(")", "");
                expUrl = expUrl.Replace("(", "");

                if (!expUrl.Equals("noUrl"))
                {
                    Uri uri = HttpDownloader.checkUri(expUrl);
                    //remove the url reference from the tweet's text
                    if (uri == null)
                    {
                        text = text.Replace(dUrl, "");
                        continue;
                    }

                    //Retreive the html of the expanded uri
                    ///////////WebClient wc = new WebClient();
                    HttpDownloader downloader = new HttpDownloader(uri, null, null);
                    string html = downloader.GetPage();

                    if (html != null)
                    {
                        //convert string html to HtmlDocument object for further parsing
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);


                        //Check if the doc is a blogger page
                        BlogHandler blogManager = new BlogHandler(doc, text, dUrl);
                        if (blogManager.isBlog())
                        {
                            List<ParsedBlog> tempBlogList = blogManager.getParsedBlogs();
                            foreach (ParsedBlog blog in tempBlogList)
                            {
                                blogList.Add(blog);
                            }

                            text = blogManager.text;
                        }
                        else
                        {
                            processRawHtml(doc, dUrl);
                        }
                    }
                    else
                    {
                        text = text.Replace(dUrl, "");
                    }
                }
                else
                {
                    text = text.Replace(dUrl, "");
                }

            }
        }

        private void processRawHtml(HtmlDocument doc, string dUrl)
        {
            int numOfTitles = 0;
            HtmlNodeCollection titleNodes = doc.DocumentNode.SelectNodes("//title");

            if (titleNodes == null)
            {
                text = text.Replace(dUrl, "");
            }
            else
            {
                numOfTitles = titleNodes.Count;
                if (numOfTitles > 1)
                {
                    foreach (HtmlNode titleNode in titleNodes)
                    {
                        text = text.Replace(dUrl, "");
                    }
                }
                else
                {
                    string title = titleNodes.ElementAt(0).InnerText;
                    if (title.Contains("http"))
                    {
                        text = text.Replace(dUrl, "");
                    }

                    string tempText = text;
                    tempText = tempText.Replace(dUrl, "");
                    //tempText = Regex.Replace(tempText, dUrl, "");
                    if (title.EndsWith("..."))
                    {
                        title = title.Replace("...", "");
                    }

                    if (tempText.Contains(title))
                    {
                        text = tempText;
                    }
                    else if (title.Contains(tempText))
                    {
                        text = title;
                    }
                    else
                    {
                        text = text.Replace(dUrl, title);
                        //text = Regex.Replace(text, dUrl, title);
                    }
                }
            }
        }



        private string expandShortUrl(string url, bool retry)
        {
            if (!retry)
            {
                retryCounter = 0;
            }

            string longUrl = url;
            try
            {
                HttpWebRequest webReq = HttpWebRequest.Create(url) as HttpWebRequest;
                webReq.AllowAutoRedirect = true;
                if (retry)
                {
                    webReq.Method = "GET";
                }
                else
                {
                    webReq.Method = "HEAD";
                }

                webReq.MaximumAutomaticRedirections = 10;

                HttpWebResponse webResp = webReq.GetResponse() as HttpWebResponse;
                if (webResp.StatusCode == HttpStatusCode.OK)
                {
                    longUrl = webResp.ResponseUri.AbsoluteUri;
                }
                else
                {
                    webResp.Close();
                    Thread.Sleep(100);
                    if (retryCounter < 4)
                    {
                        retryCounter++;
                        return expandShortUrl(url, true);
                    }
                    else
                    {
                        retryCounter = 0;
                        return "noUrl";
                        //return shortUrl;
                    }

                }
                return longUrl;
            }
            catch (NotSupportedException e)
            {
                //  Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". NotSupportedException: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                return "noUrl";
            }
            catch (SecurityException e)
            {
                //  Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". SecurityException: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                return "noUrl";
            }
            catch (UriFormatException e)
            {
                //  Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". URIFormatException: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                return "noUrl";
            }
            catch (WebException e)
            {
                //   Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". WebException: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                //

                if (retryCounter < 4)
                {
                    retryCounter++;
                    return expandShortUrl(url, true);
                }
                else
                {
                    retryCounter = 0;
                    return "noUrl";
                    //return shortUrl;
                }
            }
            catch (ProtocolViolationException e)
            {
                //  Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". NetProtocolViolationException: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                return "noUrl";
            }
            catch (InvalidOperationException e)
            {
                //  Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". InvalidCastOperationException: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                return "noUrl";
            }
            catch (Exception e)
            {
                //  Console.Out.WriteLine("An error has occured while trying to follow url " + shortUrl + ". Exception: " + e.StackTrace + ". Inner exception: " + e.InnerException);
                return "noUrl";
            }




        }
        
    }
}
