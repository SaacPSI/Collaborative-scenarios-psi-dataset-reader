using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample_project.specific_class
{
    internal class NewJVAData
    {
        public DateTime startTimejvainitiator;
        public DateTime endTimejvainitiator;
        public DateTime startTimejvaresponder;
        public DateTime endTimejvaresponder;
        public TimeSpan durationTime;
        public string objectID;
        public int initiator;
        public int responder;
        public bool isAlreadyAddToCurrentThreshold = false;

        public NewJVAData(DateTime stinit, DateTime etinit, DateTime stresp, DateTime etresp, TimeSpan duration, string id, int init, int resp)
        {
            startTimejvainitiator = stinit;
            endTimejvainitiator = etinit;
            startTimejvaresponder = stresp;
            endTimejvaresponder = etresp;
            durationTime = duration;
            objectID = id;
            initiator = init;
            responder = resp;
        }
    }
}
