using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample_project.specific_class
{
    class TakenAndPositionnedPieceData
    {
        public DateTime originatingTimeTaken;
        public DateTime originatingTimePositionned;
        string objectID;
        public bool taken;
        public bool positionned;

        public TakenAndPositionnedPieceData(DateTime ott, DateTime otp, string id, bool _isTaken, bool _isPositionned)
        {
            originatingTimeTaken = ott;
            originatingTimeTaken = otp;
            objectID = id;
            taken = _isTaken;
            positionned = _isPositionned;

        }

    }
}
