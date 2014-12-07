namespace GameCore.Map
{
    /// <summary>
    ///     This represents a tile object, the map is made up of tiles.
    /// </summary>
    public class Tile
    {
        public enum TileIds
        {
            Desert,
            Grass,
            Road
        }

        private TileIds theTileId = TileIds.Desert;

        public Tile(TileIds aTileId)
        {
            theTileId = aTileId;
        }
    }
}