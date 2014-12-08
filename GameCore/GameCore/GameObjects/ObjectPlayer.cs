using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Utils;

namespace GameCore.GameObjects
{
    public class ObjectPlayer : GameObject
    {
        public ObjectPlayer(ObjcetIds aObjectId) : base(aObjectId)
        {

        }

        /// <summary>
        /// The orientation of the player given by a vector.
        /// </summary>
        public Vector Orientation = new Vector(1.0f,0.0f);
    }
}
