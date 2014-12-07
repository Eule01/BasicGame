#region

using System.Drawing;
using GameCore.Map;

#endregion

namespace GameCore.Render
{
    public class Renderer
    {
        private GameStatus theGameStatus;

        public Renderer(GameStatus aGameStatus)
        {
            theGameStatus = aGameStatus;
        }

        public void DrawTile(Tile aTile)
        {

        }

        internal void DrawGame()
        {
            DrawMap();
        }

        private void DrawMap()
        {
            Map.Map tempMap = theGameStatus.theMap;
            Size tempMapSize = tempMap.TheSize;
            for (int x = 0; x < tempMapSize.Width; x++)
            {
                for (int y = 0; y < tempMapSize.Height; y++)
                {
                    Tile aTile = tempMap[x, y];
                    DrawTile(aTile);
                }
            }
        }
    }
}