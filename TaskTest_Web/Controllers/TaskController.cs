using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskTest_Web.Controllers
{
    public class TaskController : Controller
    {

        public string Test(int? pageIndex)
        {
            int index = pageIndex.HasValue == true ? (int)pageIndex : 1;

            var sbText = new System.Text.StringBuilder("{ \"PagePart\": { \"TotoalPages\": 5, \"PageSize\": 10, \"PageIndex\": ");
            //
            sbText.Append(index).Append(" }, ").Append("\"Tasks\": [{ \"id\": 2, \"title\":\"John\", \"priority\":6, \"dueTime\":\"2015-06-05\", \"executer\":\"John Doe\", \"memo\": \"hello world\",  \"doActions\" : [ {\"doAction\": 2, \"actionMemo\" : \"Accept\" }, {\"doAction\": 3, \"actionMemo\" : \"Reject\" }, {\"doAction\": 4, \"actionMemo\" : \"Done\" } ] }, { \"id\": 9, \"title\":\"Stars Plan\", \"priority\":4, \"dueTime\":\"2015-06-15\", \"executer\":\"Wesley Gibson\", \"memo\": \"what's your plan?\", \"doActions\" : [ { \"doAction\": 0, \"actionMemo\" : \"Create New\" }, { \"doAction\": 5, \"actionMemo\" : \"Edit Task\" }, { \"doAction\": 1, \"actionMemo\" : \"Assign\" } ] }]}");

            return sbText.ToString(); 
        }

        public string AcceptTask(int id)
        {
            return "{ \"result\" : 0, \"message\" : \"ok\" }";
        }

        public string RejectTask(int id)
        {
            return "{ \"result\" : 2, \"message\" : \"no database!\" }";
        }

        public string TaskDone(int id)
        {
            return "{ \"result\" : 1, \"message\" : \"some one has not done yet\" }";
        }
    }
}