using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidGraphs.Models
{
    public class CovidData
    {
        public string date { get; set; }
        public int newCasesBySpecimenDate { get; set; }
    }

    public class CovidJsonRoot
    {
        public int length { get; set; }
        public int maxPageLimit { get; set; }
        public List<CovidData> data { get; set; }
    }

    public class Areas
    { 
        public int areaID { get; set; }
        public AreaList areaNames { get; set; }
    }
    public enum AreaList
    { 
        Fareham,
        Reading
    }
}