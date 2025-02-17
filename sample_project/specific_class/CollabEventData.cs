using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample_project.specific_class
{
    internal class CollabEventData
    {
        public DateTime originatingTime;
        public float time;
        public int firstUser;
        public int secondUser;
        public string collabAction;
        public bool isBegin;
        public bool requireSpecificCue;
        public string objectIfexist;

        public CollabEventData(string value)
        {
            var parts = value.Split(';');
            time = float.Parse(parts[0]);
            secondUser = int.Parse(parts[1]);
            collabAction = parts[2];
            isBegin = bool.Parse(parts[3]);
            requireSpecificCue = bool.Parse(parts[4]);
            objectIfexist = parts[5];
        }
    }
}
