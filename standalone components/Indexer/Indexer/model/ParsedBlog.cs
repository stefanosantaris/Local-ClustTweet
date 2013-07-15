using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer.model
{
    class ParsedBlog
    {
        public ParsedBlog(string postId, string blogId, string text, DateTime? timestamp, string[] tags, string title)
        {
            this.postId = postId;
            this.blogId = blogId;
            this.text = text;
            this.timestamp = timestamp;
            this.tags = tags;
            this.title = title;
        }


        public override string ToString()
        {

            return "ParsedBlog:[id:" + postId + ", blogId:" + blogId + ", time:" + timestamp + ", title:" + title + ", text:" + text + ", tags:{" + string.Join(",", tags) + "}]";
        }
        public string postId { get; set; }
        public string blogId { get; set; }
        public string text { get; set; }
        public string title { get; set; }
        public DateTime? timestamp { get; set; }
        public string[] tags { get; set; }
    }
}
