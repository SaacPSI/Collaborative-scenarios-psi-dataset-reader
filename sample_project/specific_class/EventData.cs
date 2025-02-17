using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample_project.specific_class
{
    internal class EventData
    {
        public DateTime originatingTime;
        public string type;
        public float time;
        public bool buttonValidation;
        public bool gaze;
        public string actionOnPieces;
        public int userID;
        public string objectID;
        public string blockState;
        public int step;
        public int userIDGazed;
        public int userIDGazer;
        public int interruptionNum;
        public bool interruptionSuccess;

        public EventData(string value)
        {
            var parts = value.Split(';');
            type = parts[0];
            time = float.Parse(parts[1]);
            buttonValidation = bool.Parse(parts[2]);
            gaze = bool.Parse(parts[3]);
            actionOnPieces = parts[4];
            userID = int.Parse(parts[5]);
            objectID = parts[6];
            blockState = parts[7];
            step = int.Parse(parts[8]);
            userIDGazed = int.Parse(parts[9]);
            userIDGazer = int.Parse(parts[10]);
            interruptionNum = int.Parse(parts[11]);
            interruptionSuccess = bool.Parse(parts[12]);
        }
    }
}
