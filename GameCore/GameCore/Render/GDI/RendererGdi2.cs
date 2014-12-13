#region

using System;
using System.Drawing;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Utils;

#endregion

namespace GameCore.Render.GDI
{
    public class RendererGdi2 : RendererGdiBase
    {
        private readonly SolidBrush desertBrush = new SolidBrush(Color.Wheat);
        private readonly SolidBrush grassBrush = new SolidBrush(Color.Green);
        private readonly SolidBrush roadBrush = new SolidBrush(Color.DarkSlateGray);
        private readonly SolidBrush unknownBrush = new SolidBrush(Color.Black);

        private readonly SolidBrush brushObjectUnknown = new SolidBrush(Color.DarkSeaGreen);

        private readonly Pen tilePen = Pens.Black;

        private static Bitmap BmpDesert;
        private static Bitmap BmpGrass;
        private static Bitmap BmpRoad;
        private static Bitmap BmpUnknown;

        private static Size TileSize;

        public RendererGdi2()
            : base()
        {
            name = "RendererGdi2";
//            TileSize = new Size((int) DispTileSize.X, (int) DispTileSize.Y);
            TileSize = new Size((int)GameToDisplaySize(DispTileSize).X, (int)GameToDisplaySize(DispTileSize).Y); 

            BmpDesert = BitmapHelper.CreatBitamp(TileSize, desertBrush);
            BmpGrass = BitmapHelper.CreatBitamp(TileSize, grassBrush);
            BmpRoad = BitmapHelper.CreatBitamp(TileSize, roadBrush);
            BmpUnknown = BitmapHelper.CreatBitamp(TileSize, unknownBrush);
        }


        protected override void DrawTile(Tile aTile, Vector vector)
        {
            throw new NotImplementedException();
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

        protected override void DrawMap(Map.Map aMap)
        {
            Bitmap tempBitmap = BmpUnknown;
            RectangleF tempRec = new RectangleF(new Vector(0, 0), TileSize);

            Size tempMapSize = aMap.TheSize;
            for (int x = 0; x < tempMapSize.Width; x++)
            {
                for (int y = 0; y < tempMapSize.Height; y++)
                {
                    Tile aTile = aMap[x, y];
                    Vector dispLoc = GameToDisplay(new Vector(x, y));
                    tempRec.Location = dispLoc;
                    switch (aTile.TheTileId)
                    {
                        case Tile.TileIds.Desert:
                            tempBitmap = BmpDesert;
                            break;
                        case Tile.TileIds.Grass:
                            tempBitmap = BmpGrass;
                            break;
                        case Tile.TileIds.Road:
                            tempBitmap = BmpRoad;
                            break;
                        default:
                            tempBitmap = BmpUnknown;
                            break;
                    }
//                    TheGraphics.DrawImageUnscaled(tempBitmap, dispLoc);
                    TheGraphics.DrawImage(tempBitmap, tempRec);
                }
            }
        }
    }
}