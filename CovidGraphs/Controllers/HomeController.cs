using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using CovidGraphs.Models;
using System.IO;
using System.Threading.Tasks;

namespace CovidGraphs.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            List<DataPoint> graphData = new List<DataPoint>();

            string url = "https://api.coronavirus.data.gov.uk/v1/data?filters=areaName=Eastleigh&structure={%22date%22:%22date%22,%22newCasesBySpecimenDate%22:%22newCasesBySpecimenDate%22}";
            //string url = "https://api.coronavirus.data.gov.uk/v1/data?filters=areaName=Reading&structure={%22date%22:%22date%22,%22newCasesBySpecimenDate%22:%22newCasesBySpecimenDate%22}";

            var data = GetData(url);

            var covidData = JsonConvert.DeserializeObject<CovidJsonRoot>(data);

            foreach (CovidData figures in covidData.data)
            {
                graphData.Add(new DataPoint(Convert.ToDateTime(figures.date), figures.newCasesBySpecimenDate));
            }

            ViewBag.DataPoints = JsonConvert.SerializeObject(graphData);
            ViewBag.Url = url;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private string GetData(string url) 
        {

            var client = new MyWebClient();
            var json = client.DownloadString(url);

            return json;

        }
    }

    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }

}

