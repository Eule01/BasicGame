#region

using System.Drawing;
using GameCore.Map;
using GameCore.Utils;

#endregion

namespace GameCore.Render
{
    public class RendererGdi : Renderer
    {
        public Graphics TheGraphics;

        private readonly SolidBrush desertBrush = new SolidBrush(Color.Wheat);
        private readonly SolidBrush grassBrush = new SolidBrush(Color.Green);
        private readonly SolidBrush roadtBrush = new SolidBrush(Color.DarkSlateGray);
        private readonly SolidBrush unknownBrush = new SolidBrush(Color.Black);

        private readonly Pen tilePen = Pens.Black;

        public RendererGdi(GameStatus aGameStatus) : base(aGameStatus)
        {
        }

        protected override void DrawTile(Tile aTile, Vector atLocation)
        {
            Vector dispLoc = GameToDisplay(atLocation);
            RectangleF tempRec = new RectangleF(dispLoc, DispTileSize);
            SolidBrush tempBrush;
            switch (aTile.TheTileId)
            {
                case Tile.TileIds.Desert:
                    tempBrush = desertBrush;
                    break;
                case Tile.TileIds.Grass:
                    tempBrush = grassBrush;
                    break;
                case Tile.TileIds.Road:
                    tempBrush = roadtBrush;
                    break;
                default:
                    tempBrush = unknownBrush;
                    break;
            }

            TheGraphics.FillRectangle(tempBrush, tempRec);
            TheGraphics.DrawRectangles(tilePen, new RectangleF[] {tempRec});
        }
    }
}