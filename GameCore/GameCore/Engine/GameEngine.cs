#region

using GameCore.Utils.Timers;

#endregion

namespace GameCore.Engine
{
    public class GameEngine : IFlowControl
    {
        private TickEngine theTickEngine;

        /// <summary>
        ///     The gamer time interval in milliseconds.
        /// </summary>
        private const int timerTickIntervalMs = 10;

        public GameEngine()
        {
            Init();
        }

        /// <summary>
        ///     Initialise the game engine.
        /// </summary>
        private void Init()
        {
            theTickEngine = new TickEngine("GameEngine",GameTick, StatusTick, timerTickIntervalMs);
        }

        private void StatusTick(OpStatus opstatus)
        {
            GameCore.theGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.Status)
                {
                    TheOpStatus = opstatus
                });
        }

        #region Game Timer

        /// <summary>
        ///     Here all the game action is computed. This is called every timerTickIntervalMs
        /// </summary>
        private void GameTick()
        {
        }

        /// <summary>
        ///     Starts the game.
        /// </summary>
        public void Start()
        {
            theTickEngine.Start();
        }

        /// <summary>
        ///     Shuts down the game.
        /// </summary>
        public void Close()
        {
            theTickEngine.Close();
        }

        /// <summary>
        ///     Pauses the game.
        /// </summary>
        public void Pause()
        {
            theTickEngine.Pause();
        }


        /// <summary>
        ///     Resumes the game.
        /// </summary>
        public void Resume()
        {
            theTickEngine.Resume();
        }

        #endregion
    }
}