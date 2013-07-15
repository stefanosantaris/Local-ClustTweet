using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPreprocessor.model
{
    class ParsedTweet
    {
        public ParsedTweet() { }
        public ParsedTweet(string id, string text, DateTime timestamp, double geolon, double geolat, string place, string[] hashtags, string userId, ParsedBlog[] blogs)
        {
            this.id = id;
            this.text = text;
            this.timestamp = timestamp;
            this.geolon = geolon;
            this.geolat = geolat;
            this.place = place;
            this.hashtags = hashtags;
            this.userId = userId;
            this.blogs = blogs;
        }

        public string id { get; set; }
        public string text { get; set; }
        public DateTime timestamp { get; set; }
        public double geolon { get; set; }
        public double geolat { get; set; }
        public string place { get; set; }
        public string[] hashtags { get; set; }
        public string userId { get; set; }
        public ParsedBlog[] blogs { get; set; }
    }
}
