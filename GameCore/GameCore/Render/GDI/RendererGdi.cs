#region

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using CodeToast;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Utils;

#endregion

namespace GameCore.Render.GDI
{
    public class RendererGdi : RendererGdiBase
    {
        private readonly SolidBrush desertBrush = new SolidBrush(Color.Wheat);
        private readonly SolidBrush grassBrush = new SolidBrush(Color.Green);
        private readonly SolidBrush roadtBrush = new SolidBrush(Color.DarkSlateGray);
        private readonly SolidBrush unknownBrush = new SolidBrush(Color.Black);

        private readonly SolidBrush brushObjectUnknown = new SolidBrush(Color.DarkSeaGreen);

        private readonly Pen tilePen = Pens.Black;

 
        public RendererGdi() : base()
        {
            name = "RendererGdi";
        }

        public override void Start()
        {
            base.Start();
        }

        protected override void DrawTile(Tile aTile, Vector atLocation)
        {
//            return;
//            TheGraphics.ToLowQuality();
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
//            TheGraphics.DrawRectangles(tilePen, new RectangleF[] {tempRec});
        }

        protected override void DrawGameObject(GameObject aGameObject)
        {
//            TheGraphics.ToLowQuality();
            Vector dispLoc = GameToDisplay(aGameObject.Location);
            float dispDiameter = GameToDisplayDistance(aGameObject.Diameter);
            float dispLocOffset = dispDiameter*0.5f;
            RectangleF tempRec = new RectangleF(dispLoc.X - dispLocOffset, dispLoc.Y - dispLocOffset, dispDiameter,
                                                dispDiameter);
            TheGraphics.FillEllipse(brushObjectUnknown, tempRec);
        }
    }

    public static class GraphicsExtensions
    {
        public static void ToHighQuality(this Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public static void ToLowQuality(this Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }
    }
}