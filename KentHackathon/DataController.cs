using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
// useage: 
// string domain = "stoh.io/recScript.php?";
// string getParam; 
// can only be get_suggests or recalculate or update
// DataController.DataController myController = new DataController();
// string responseUrl = myController.MakeRequest(domain + getParam);
// IList<SearchResult> myList = ParseJson(responseUrl);
// now you have a list of all the results. 
// to access: myList.url;
//            myList.id;
//--if !null--myList.response;
// for update: 
// string getParam = "update&id=" + myList.id + "&response=" + myList.response; 
// string responseUrl = myController.MakeRequest(domain + getParam);

namespace KentHackathon
{
    class DataController
    {
        string MakeRequest(string link)
        {
            HttpWebRequest request = WebRequest.Create(link) as HttpWebRequest;

            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();

            return responseFromServer;
        }
        IList<SearchResult> ParseJson(string response)
        {
            JObject suggestionData = JObject.Parse(response);

            // get JSON result objects into a list
            IList<JToken> results = suggestionData["suggestionData"]["results"].Children().ToList();

            // serialize JSON results into .NET objects
            IList<SearchResult> searchResults = new List<SearchResult>();
            foreach (JToken result in results)
            {
                SearchResult searchResult = JsonConvert.DeserializeObject<SearchResult>(result.ToString());
                searchResults.Add(searchResult);
            }
            return searchResults;
        }



    }
}
