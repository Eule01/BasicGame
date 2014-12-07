namespace GameCore.Utils.Timers
{
    public class OpStatus
    {
        public const string TEXT_FPS = "FPS: ";
        public const string TEXT_AVR_TIME = "Avr. time: ";
        public const string TEXT_MISSED_FRAMES = "Missed frames: ";
        public const string TEXT_MAX_TIME = "Max time: ";
        public const string TEXT_INTERVAL_MAX_TIME = "Int. max time: ";

        private double ops = 0.0;
        private double avrOpTime = 0.0;
        private long missedFrames = 0;
        private double maxTime = 0.0;
        private double intervalMaxTime = 0.0;
        private string name;

        public OpStatus(string aName)
        {
            this.name = aName;
        }

        public double Ops
        {
            get { return ops; }
            set { ops = value; }
        }

        public double AvrOpTime
        {
            get { return avrOpTime; }
            set { avrOpTime = value; }
        }


        public long MissedFrames
        {
            get { return missedFrames; }
            set { missedFrames = value; }
        }

        public double MaxTime
        {
            get { return maxTime; }
            set { maxTime = value; }
        }

        public double IntervalMaxTime
        {
            get { return intervalMaxTime; }
            set { intervalMaxTime = value; }
        }

        public static string GetNiceTime(double timeInSec)
        {
            if (timeInSec >= 1.0)
            {
                return timeInSec.ToString("0.0") + "s";
            }
            else if (timeInSec >= 0.001)
            {
                return (timeInSec*1000.0).ToString("0.0") + "ms";
            }
            else if (timeInSec >= 0.000001)
            {
                return (timeInSec*1000000.0).ToString("0.0") + "us";
            }
            else
                //            else if (timeInSec >= 0.000000001)
            {
                return (timeInSec*1000000000.0).ToString("0.0") + "ns";
            }
        }

        public override string ToString()
        {
            string outStr = name+":";
            outStr += " FPS: " + ops.ToString("0.0") + "Hz ";
            outStr += " avr. Time: " + GetNiceTime(avrOpTime);
            outStr += " missed frames: " + missedFrames;
            outStr += " interval max time: " + GetNiceTime(intervalMaxTime);
            outStr += " max time: " + GetNiceTime(maxTime);

            return outStr;
        }
    }
}