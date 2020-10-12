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
using System.Web.UI.WebControls;

namespace CovidGraphs.Controllers
{
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index()
        {
            string area = "Eastleigh";
            string url = "https://api.coronavirus.data.gov.uk/v1/data?filters=areaName=" + area + "&structure={%22date%22:%22date%22,%22newCasesBySpecimenDate%22:%22newCasesBySpecimenDate%22}";

            string graphData = GetGraphData(url);
            ViewBag.DataPoints = graphData;
            ViewBag.Area = area;
            ViewBag.Url = url;

            return View();
        }

        private string GetGraphData(string url)
        {
            List<DataPoint> graphData = new List<DataPoint>();

            String data = GetData(url);

            var covidData = JsonConvert.DeserializeObject<CovidJsonRoot>(data);

            foreach (CovidData figures in covidData.data)
            {
                graphData.Add(new DataPoint(Convert.ToDateTime(figures.date), figures.newCasesBySpecimenDate));
            }
            return JsonConvert.SerializeObject(graphData);
        }

        [HttpPost]
        public ActionResult Index(string area)
        {
            string url = "https://api.coronavirus.data.gov.uk/v1/data?filters=areaName=" + area + "&structure={%22date%22:%22date%22,%22newCasesBySpecimenDate%22:%22newCasesBySpecimenDate%22}";

            string graphData = GetGraphData(url);

            ViewBag.DataPoints = graphData;
            ViewBag.Url = url;
            ViewBag.Area = area;

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
            string json = client.DownloadString(url);

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

