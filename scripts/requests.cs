using System;
using System.Net;
using System.IO;

namespace MakeAGETRequest_charp
{
	class Class1
	{
		static void Main(string[] args)
		{
			string sURL;
			sURL = "http://stoh.io/recScript.php";

			WebRequest wrGETURL;
			wrGETURL = WebRequest.Create(sURL);

			Stream objStream;
			objStream = wrGETURL.GetResponse().GetResponseStream();

			StreamReader objReader = new StreamReader(objStream);

			string sLine = "";
			int i = 0;

			while (sLine!=null)
			{
				i++;
				sLine = objReader.ReadLine();
				if (sLine!=null)
					Console.WriteLine("{0}:{1}",i,sLine);
			}
			Console.ReadLine();
		}
	}
}
