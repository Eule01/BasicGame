﻿#region

using System.IO;
using GameCore.Engine;
using GameCore.Interface;
using GameCore.Render;
using GameCore.Utils.Timers;

#endregion

namespace GameCore
{
    /// <summary>
    ///     The core of the game. This is the main class that creates the game.
    /// </summary>
    public class GameCore : IFlowControl
    {
        private GameStatus theGameStatus;

        private GameEngine theGameEngine;

        private RendererBase theRenderer;

        private RendererManager theRendererManager;

        /// <summary>
        ///     This is holding the game core so it can be seen from all other classes. This is not best practice I guess.
        /// </summary>
        internal static GameCore TheGameCore;

        public GameCore()
        {
            Init();
        }

        private void Init()
        {
            TheGameEventHandler += GameCore_TheGameEventHandler;
            TheGameCore = this;
            theGameEngine = new GameEngine();
            theRendererManager = new RendererManager();

//            TheGameStatus = new GameStatus();
            theGameStatus = GameStatus.CreatTestGame();
        }

        public GameStatus TheGameStatus
        {
            get { return theGameStatus; }
        }

        public RendererManager TheRendererManager
        {
            get { return theRendererManager; }
        }

        public RendererBase TheRenderer
        {
            get { return theRenderer; }
            set { theRenderer = value; }
        }

        public UserInput TheUserInput
        {
            get { return theGameEngine.TheUserInput; }
        }

        private void GameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
        }

        /// <summary>
        ///     Raise a message.
        /// </summary>
        /// <param name="aMessage"></param>
        internal void RaiseMessage(string aMessage)
        {
            OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.Message));
        }

        #region Game flow

        public void Start()
        {
            theGameEngine.Start();
            ChangeRenderer(2);
        }

        public void ChangeRenderer(int aRendererIndex)
        {
            theGameEngine.Pause();
            theRendererManager.SetRenderer(aRendererIndex, theGameStatus, TheUserInput);
            theRenderer = theRendererManager.TheRenderer;
            theGameEngine.Resume();
        }


        public void Pause()
        {
            theGameEngine.Pause();
        }

        public void Resume()
        {
            theGameEngine.Resume();
        }

        public void Close()
        {
            theGameEngine.Close();
            theRendererManager.Close();
//            theRenderer.Close();
            theGameStatus.Close();
        }

        #endregion

        public void SaveMap(string aFilePath)
        {
            string tempFilePath = FileNameToMapFileName(aFilePath);
            theGameStatus.SaveMap(tempFilePath);
            TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.MapSaved)
                {
                    Message = tempFilePath
                });
        }

        public void LoadMap(string aFilePath)
        {
            Pause();
            string tempFilePath = FileNameToMapFileName(aFilePath);
            theGameStatus.LoadMap(tempFilePath);
            theRenderer.MapLoaded();
            TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.MapLoaded)
                {
                    Message = tempFilePath
                });
            Resume();
        }

        private static string FileNameToMapFileName(string aFilePath)
        {
            string tempPath = Path.GetDirectoryName(aFilePath);
            string aFileName = Path.GetFileNameWithoutExtension(aFilePath);
            aFileName = "Map_" + aFileName + ".xml";
            string tempFilePath = Path.Combine(tempPath, aFileName);
            return tempFilePath;
        }

        #region Game Events

        public delegate void GameEventHandlerDel(object sender, GameEventArgs args);


        private GameEventHandlerDel gameEventHandlerDel;

        private readonly object eventLock = new object();

        public event GameEventHandlerDel TheGameEventHandler
        {
            add
            {
                lock (eventLock)
                {
                    // First try to remove the handler, then re-add it
                    gameEventHandlerDel -= value;
                    gameEventHandlerDel += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    gameEventHandlerDel -= value;
                }
            }
        }


        internal void OnGameEventHandler(GameEventArgs args)
        {
            GameEventHandlerDel handler = gameEventHandlerDel;
            if (handler != null) handler(this, args);
        }

        #endregion
    }
}