using HtmlAgilityPack;
using JsonParser.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonParser.network
{
    class BlogHandler
    {
        private string dUrl;
        private HtmlDocument doc;

        public string text { get; set; }


        public BlogHandler(HtmlDocument doc, string text, string dUrl)
        {
            this.doc = doc;
            this.text = text;
            this.dUrl = dUrl;
        }

        public bool isBlog()
        {
            HtmlNode node1 = doc.DocumentNode.SelectSingleNode("//meta[@name='generator']");
            HtmlNode node2 = doc.DocumentNode.SelectSingleNode("//meta[@content='blogger']");
            if (node1 != null && node2 != null)
            {
                return true;
            }
            return false;
        }


        public List<ParsedBlog> getParsedBlogs()
        {
            List<ParsedBlog> parsedBlogList = new List<ParsedBlog>();
            HtmlNodeCollection posts = doc.DocumentNode.SelectNodes("//div[@class='post hentry']");
            if (posts != null)
            {
                string docStr = doc.DocumentNode.InnerHtml;
                Match match = Regex.Match(docStr, "http://.*/feeds/[0-9]+/posts/default");
                string blogId = Regex.Replace(match.Value, "http://.*/feeds/", "");
                blogId = Regex.Replace(blogId, "/posts/default", "");
                if (posts.Count == 1)
                {
                    HtmlNode node = posts.ElementAt(0);
                    string postId = node.SelectSingleNode("./a[@name]").GetAttributeValue("name", string.Empty);
                    IEnumerable<HtmlNode> tagEls = node.Descendants("a");
                    ArrayList tags = new ArrayList();
                    foreach (var tagEl in tagEls)
                    {
                        if (tagEl.Attributes["rel"] != null)
                        {
                            string relVal = tagEl.Attributes["rel"].Value;
                            if (relVal == "tag")
                            {
                                tags.Add(relVal);
                            }
                        }
                    }

                    HtmlNode titleEl = node.SelectSingleNode("./*[@class='post-title entry-title']");
                    string title = string.Empty;
                    if (titleEl != null)
                    {
                        title = titleEl.InnerText;
                    }
                    text = text.Replace(dUrl, title);
                    //text = Regex.Replace(text, dUrl, title);

                    string txt = string.Empty;
                    HtmlNode txtNode = node.SelectSingleNode("./*[@class='post-body entry-content']");
                    if (txtNode != null)
                    {
                        txt = txtNode.InnerText;
                    }

                    string postText = TextSanitizer.stripTags(txt);

                    string[] tagsArray = tags.ToArray(typeof(string)) as string[];
                    if (postText != string.Empty)
                    {
                        parsedBlogList.Add(new ParsedBlog(postId, blogId, postText, null, tagsArray, title));
                    }
                    

                }
            }
            text = text.Replace(dUrl, "");
            return parsedBlogList;
        }


    }
}
