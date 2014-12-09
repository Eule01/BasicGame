﻿#region

using System.Drawing;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Utils;
using GameCore.Utils.Timers;

#endregion

namespace GameCore.Render
{
    public abstract class Renderer
    {
        private readonly GameStatus theGameStatus;

        private ITickEngine theTickEngine;

        /// <summary>
        ///     The renderer time interval in milliseconds.
        /// </summary>
//        private const int refreshIntervalMs = 33;
        private const int refreshIntervalMs = 5;


        private float zoomFactor = 20.0f;
        private float oneOverZoomFactor;

        private Vector origin = new Vector(0, 0);

        internal Vector DispTileSize;


        protected Renderer()
        {
            Init();
        }


        public Renderer(GameStatus aGameStatus)
        {
            theGameStatus = aGameStatus;
            Init();
        }

        private void Init()
        {
            ZoomChanged();
            theTickEngine = new TickEngineThread();
            theTickEngine.Setup("Renderer", UpdateRender, StatusTick, refreshIntervalMs);
            theTickEngine.Start();
        }

        public void Close()
        {
            theTickEngine.Close();
        }


        /// <summary>
        ///     Here all the game action is computed. This is called every refreshIntervalMs
        /// </summary>
        protected abstract void UpdateRender();


        private void StatusTick(OpStatus opstatus)
        {
            GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.Status)
                {
                    TheOpStatus = opstatus
                });
        }

        private void ZoomChanged()
        {
            oneOverZoomFactor = 1.0f/zoomFactor;
            DispTileSize = GameToDisplaySize(Tile.Size);
        }

        #region Coordinate transformations

        internal Vector GameToDisplay(Vector aGameVector)
        {
            Vector disVec = (aGameVector + origin)*zoomFactor;
            return disVec;
        }

        internal Vector DisplayToGame(Vector aDisVector)
        {
            Vector gameVec = aDisVector*oneOverZoomFactor - origin;
            return gameVec;
        }

        internal Vector GameToDisplaySize(Vector aGameSize)
        {
            return aGameSize*zoomFactor;
        }

        internal float GameToDisplayDistance(float aGameDistance)
        {
            return aGameDistance*zoomFactor;
        }

        internal Vector DisplayToGameSize(Vector aDisplaySize)
        {
            return aDisplaySize*oneOverZoomFactor;
        }

        #endregion

        public void DrawGame()
        {
            DrawMap(theGameStatus.TheMap);
            DrawGameObjects();
        }

        private void DrawGameObjects()
        {
            foreach (GameObject aGameObject in theGameStatus.GameObjects)
            {
                DrawGameObject(aGameObject);
            }
        }

        protected virtual void DrawTile(Tile aTile, Vector vector)
        {
            
        }

        protected virtual void DrawGameObject(GameObject aGameObject)
        {
            
        }

        protected virtual void DrawMap(Map.Map aMap)
        {
            Size tempMapSize = aMap.TheSize;
            for (int x = 0; x < tempMapSize.Width; x++)
            {
                for (int y = 0; y < tempMapSize.Height; y++)
                {
                    Tile aTile = aMap[x, y];
                    DrawTile(aTile, new Vector(x, y));
                }
            }
        }
    }
}