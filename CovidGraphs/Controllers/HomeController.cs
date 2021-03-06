﻿using System;
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

        [HttpPost]
        public ActionResult Index(string area)
        {
            if (area == "") 
            {
                area = "Eastleigh";
            }
            string url = "https://api.coronavirus.data.gov.uk/v1/data?filters=areaName=" + area + "&structure={%22date%22:%22date%22,%22newCasesBySpecimenDate%22:%22newCasesBySpecimenDate%22}";

            CovidData covidData = GetGraphData(url);
            ViewBag.CasesInLastDays = CasesInLastDays(covidData, -7);
            ViewBag.DataPoints = ConvertDataToGraphFormat(covidData);
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