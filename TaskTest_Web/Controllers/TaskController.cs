using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskTest_Web.Controllers
{
    public class TaskController : Controller
    {
        public string Test()
        {
            return "{ \"PagePart\": { \"TotoalPages\": 5, \"PageSize\": 10, \"PageIndex\": 1 }, \"Tasks\": [{ \"title\":\"John\", \"priority\":6, \"dueTime\":\"2015-06-05\", \"executer\":\"John Doe\" },{ \"title\":\"his\", \"priority\":7, \"dueTime\":\"2015-02-05\", \"executer\":\"John Kate\" }]}";
            //return "{ \"Result\": true, \"intArray\": [3,4,1,2,5],	\"employees\": [{ \"firstName\":\"John\" , \"lastName\":\"Doe\" },{ \"firstName\":\"Anna\" , \"lastName\":\"Smith\" }]}";
        }
    }
}