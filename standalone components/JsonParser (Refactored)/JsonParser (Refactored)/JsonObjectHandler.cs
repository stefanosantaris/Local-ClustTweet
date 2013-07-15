using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonParser
{
    class JsonObjectHandler
    {

        public JsonObjectHandler(JObject jsonObject)
        {
            this.jsonObject = jsonObject;
            hashtagTexts = new ArrayList();
            urlTexts = new ArrayList();
            parseObjects();
            parseValues();

        }

        private void parseValues()
        {
            

            //retrieve the source element of the json
            source = (JValue)status["source"];

            //retrieve the date information of the tweet
            string when = (string)status.SelectToken("created_at");
            createdDateTime = DateTime.ParseExact(when, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);

            //retrieve the json id
            id = (string)status.SelectToken("id_str");

            //retrieve the tweet's text
            text = (string)status.SelectToken("text");
            text = Regex.Replace(text, @"(?<!\w)@\w+", "");


            //retrieve the user's id
            userId = (string)user.SelectToken("id_str");

            retrievePlace();

            retrieveGeolocation();

            urlsArray = getUrlsArray();
            //urlsArray = (JArray)entitesObject["urls"];
            
            //Retrieve the links that have not been placed into the urls entity
            //if ((urlsArray.Count == 0) && (text.Contains("http") || text.Contains("https")))
            //{
            //    string jsonUrl = "[";
            //    string[] textSplit = text.Split(' ');
            //    for (int i = 0; i < textSplit.Length; i++)
            //    {
            //        if (textSplit[i].Contains("http") || textSplit[i].Contains("https"))
            //        {
            //            jsonUrl += "{\"url\": \"" + textSplit[i] + "\",\"expanded_url\": \"" + textSplit[i] + "\", \"display_url\": \" null \", \" indices\": [ 00,00 ] } , ";
                        
            //        }
            //    }
            //    jsonUrl = jsonUrl.Remove(jsonUrl.Length - 2, 1);
            //    jsonUrl += "]";
            //    urlsArray = (JArray)JsonConvert.DeserializeObject(jsonUrl);
            //}
            hashtagsArray = (JArray)entitesObject["hashtags"];

        }

        private JArray getUrlsArray()
        {
            JArray tempUrlsArray = (JArray)entitesObject["urls"];
            string[] textSplit = text.Split(' ');
            string jsonUrl;
            for (int i = 0; i < textSplit.Length; i++)
            {
                if (textSplit[i].Contains("http") || textSplit[i].Contains("https"))
                {
                    if (!urlRetrieved(tempUrlsArray, textSplit[i]))
                    {
                        tempUrlsArray.Add(new JObject(new JProperty("url", textSplit[i]), new JProperty("expanded_url", textSplit[i]), new JProperty("display_url", "null"), new JProperty("indices", "[00,00]")));
                        //jsonUrl = "{\"url\": \"" + textSplit[i] + "\",\"expanded_url\": \"" + textSplit[i] + "\", \"display_url\": \" null \", \" indices\": [ 00,00 ] } ";
                        //tempUrlsArray.Add(jsonUrl);
                    }
                }
            }


            return tempUrlsArray;
        }

        

        private bool urlRetrieved(JArray tempUrlsArray, string p)
        {
            for (int i = 0; i < tempUrlsArray.Count; i++)
            {
                string url = (string)tempUrlsArray.ElementAt(i).SelectToken("url");
                if (url.Equals(p))
                {
                    return true;
                }
            }

            return false;
        }



        private void retrieveGeolocation()
        {
            //initialize the geolocation coordinates with the default value
            geolon = nonExistentGeocoordinateValue;
            geolat = nonExistentGeocoordinateValue;


            //recognize the type of geo attribute
            var temp = status.SelectToken("geo");
            if (temp.GetType() == typeof(JObject))
            {
                //retrieve the geo object
                JObject geoObject = (JObject)status["geo"];
                if (geoObject.Count != 0)
                {
                    //retrieve the coordinates
                    JArray co = (JArray)geoObject["coordinates"];
                    var latTest = co.ElementAt(0);
                    int aux;
                    if (latTest.Type == JTokenType.Integer)
                    {
                        aux = (int)latTest;
                        geolat = System.Convert.ToDouble(aux);
                    }
                    else
                    {
                        geolat = (double)co.ElementAt(0);
                    }
                    var lonTest = co.ElementAt(1);
                    if (lonTest.Type == JTokenType.Integer)
                    {
                        aux = (int)lonTest;
                        geolon = System.Convert.ToDouble(aux);
                    }
                    else
                    {
                        geolon = (double)co.ElementAt(1);
                    }
                }
            }
        }

        private void retrievePlace()
        {
            //initiate the place string. We are not sure whether the json contains the location or not
            var temp = status["place"];
            place = string.Empty;

            if (temp.GetType() == typeof(JObject))
            {
                JObject placeObject = (JObject)status["place"];
                place = (string)placeObject.SelectToken("full_name");
            }
            else
            {
                JValue placeValue = (JValue)status["place"];
                
                if (placeValue.Value != null)
                {
                    place = placeValue.Value.ToString();
                }
            }
        }

        

        private void parseObjects()
        {
            //retrieve the embed data object
            embedData = (JObject)jsonObject["embedData"];


            //retrieve the status object
            status = (JObject)embedData["status"];

            //retrieve the user object
            user = (JObject)status["user"];

            //retrieve the entites object
            entitesObject = (JObject)status["entities"];
        }


        public void processHashTags()
        {
            foreach (JObject tag in hashtagsArray)
            {
                string tagText = tag.SelectToken("text").ToString();
                hashtagTexts.Add(tagText);
                text = Regex.Replace(text, "#" + tagText, "");
            }
        }


        #region jobject fields 

        private JObject jsonObject { get; set; }
        private JObject embedData { get; set; }
        private JObject status { get; set; }
        private JObject user { get; set; }
        private JObject entitesObject { get; set; }

        #endregion

        #region jvalue fields
        public JValue source { get; set; }
        public JArray urlsArray { get; set; }
        public JArray hashtagsArray { get; set; }
        #endregion


        #region extracted fields
        public DateTime createdDateTime { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public double geolat { get; set; }
        public double geolon { get; set; }
        public string userId { get; set; }
        public string place { get; set; }
        #endregion

        #region public arrays 
        public ArrayList hashtagTexts;
        public ArrayList urlTexts;
        #endregion

        #region read only fields

        private readonly double nonExistentGeocoordinateValue = 200;

        #endregion

    }
}
