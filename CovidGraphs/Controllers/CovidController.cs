using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CovidGraphs.Models;
using Newtonsoft.Json;

namespace CovidGraphs.Controllers
{
    public class CovidController : Controller
    {
        // GET: Home
        public ActionResult Index(string area)
        {
            //string area = "Eastleigh";
            string url = "https://api.coronavirus.data.gov.uk/v1/data?filters=areaName=" + area + "&structure={%22date%22:%22date%22,%22newCasesBySpecimenDate%22:%22newCasesBySpecimenDate%22}";

            CovidData covidData = GetGraphData(url);

            ViewBag.CasesInLastDays = CasesInLastDays(covidData, -7);
            ViewBag.DataPoints = ConvertDataToGraphFormat(covidData);
            ViewBag.Area = area;
            ViewBag.Url = url;

            return View();
        }

        private int CasesInLastDays(CovidData covidData, int numerOfDays)
        {
            int cases = 0;
            TimeSpan lastSevenDays = new TimeSpan(numerOfDays, 0, 0, 0);
            //DateTime[] dates = new DateTime[7];
            List<DateTime> dates = new List<DateTime>();

            foreach (CovidDataItem figures in covidData.data)
            {
                if (Convert.ToDateTime(figures.date) >= DateTime.Today.Add(lastSevenDays) && (!dates.Contains(Convert.ToDateTime(figures.date))))
                {
                    cases += figures.newCasesBySpecimenDate;
                    dates.Add(Convert.ToDateTime(figures.date));
                }
            }

            return cases;
        }

        private String ConvertDataToGraphFormat(CovidData covidData)
        {
            List<DataPoint> graphData = new List<DataPoint>();

            foreach (CovidDataItem figures in covidData.data)
            {
                graphData.Add(new DataPoint(Convert.ToDateTime(figures.date), figures.newCasesBySpecimenDate));
            }
            return JsonConvert.SerializeObject(graphData);
        }

        private CovidData GetGraphData(string url)
        {
            String data = GetData(url);

            CovidData covidData = JsonConvert.DeserializeObject<CovidData>(data);

            return covidData;

        }
        private string GetData(string url)
        {

            var client = new MyWebClient();
            string json = client.DownloadString(url);

            return json;

        }
    }

    //class MyWebClient : WebClient
    //{
    //    protected override WebRequest GetWebRequest(Uri address)
    //    {
    //        HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
    //        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    //        return request;
    //    }
    //}
}