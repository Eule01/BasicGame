namespace GameCore
{
    public class GameStatus
    {
        /// <summary>
        ///     The tile map of this status.
        /// </summary>
        private Map.Map theMap;

        /// <summary>
        ///     The millisecond run from start for this status.
        /// </summary>
        private ulong theMilliSeconds = 0;

        public GameStatus()
        {
            Init();
        }

        private void Init()
        {
            theMap = new Map.Map();
            theMilliSeconds = 0;
        }


        /// <summary>
        ///     Loads a GameStatus from file
        /// </summary>
        /// <param name="aGamePath"></param>
        /// <returns></returns>
        public static GameStatus Load(string aGamePath)
        {
            return null;
        }

        /// <summary>
        ///     Saves a GameStatus to file
        /// </summary>
        /// <param name="aGamePath"></param>
        public static void Save(string aGamePath)
        {
        }


        /// <summary>
        /// Closes this object and disposes everything.
        /// </summary>
        public void Close()
        {
            theMap.Close();

        }
    }
}