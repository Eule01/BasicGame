#region

using GameCore.Utils;

#endregion

namespace GameCore.GameObjects
{
    public class GameObject
    {
        public enum ObjcetIds
        {
            Player,
            Zork,
            Gustav
        }

        public float Diameter = 0.4f;

        public Vector Location = new Vector(0, 0);

        private ObjcetIds theObjectId = ObjcetIds.Player;

        public GameObject(ObjcetIds aObjectId)
        {
            theObjectId = aObjectId;
        }

        public ObjcetIds TheObjectId
        {
            get { return theObjectId; }
        }
    }
}