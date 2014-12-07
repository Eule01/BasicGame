#region

using System.Drawing;
using GameCore.Map;
using GameCore.Utils;

#endregion

namespace GameCore.Render
{
    public abstract class Renderer
    {
        private readonly GameStatus theGameStatus;

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
        }

        protected abstract void DrawTile(Tile aTile, Vector vector);

        private void ZoomChanged()
        {
            oneOverZoomFactor = 1.0f/zoomFactor;
            DispTileSize = GameToDisplaySize(Tile.Size);
        }

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

        internal Vector DisplayToGameSize(Vector aDisplaySize)
        {
            return aDisplaySize * oneOverZoomFactor;
        }


        internal void DrawGame()
        {
            DrawMap();
        }

        public void DrawMap()
        {
            Map.Map tempMap = theGameStatus.theMap;
            Size tempMapSize = tempMap.TheSize;
            for (int x = 0; x < tempMapSize.Width; x++)
            {
                for (int y = 0; y < tempMapSize.Height; y++)
                {
                    Tile aTile = tempMap[x, y];
                    DrawTile(aTile, new Vector(x, y));
                }
            }
        }
    }
}