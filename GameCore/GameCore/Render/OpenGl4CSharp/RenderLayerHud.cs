#region

using System.Collections.Generic;
using System.Drawing;
using GameCore.Interface;
using GameCore.Map;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.Render.OpenGl4CSharp
{
    public class RenderLayerHud : RenderLayerBase
    {
        private ShaderProgram hudProgram;
        private ObjLoader objectList;
        private List<ObjObject> theTileObjects;
        private List<ObjHud> theHudObjects = new List<ObjHud>();


        public RenderLayerHud(int width, int height, GameStatus theGameStatus, UserInput theUserInput)
            : base(width, height, theGameStatus, theUserInput)
        {
            theTileObjects = new List<ObjObject>();
        }

        public override void OnLoad()
        {
            hudProgram = new ShaderProgram(VertexShader, FragmentShader);
//            hudProgram = new ShaderProgram(vertexShader2Source, fragmentShader2Source);

            hudProgram.Use();
            hudProgram["projection_matrix"].SetValue(Matrix4.CreateOrthographic(Width, Height, 0, 1000));
            hudProgram["model_matrix"].SetValue(Matrix4.Identity);

//            hudProgram["color"].SetValue(new Vector3(1, 1, 1));

            Dictionary<Tile.TileIds, PlainBmpTexture> tempTiletypeList =
                RenderObjects.CreateTileTextures(new Size(20, 20), hudProgram);
            List<ObjObject> tempObjList = new List<ObjObject>();
            int counter = 2;
            int zeroX = -Width/2;
            int zeroY = Height/2;
            Size tempSize = new Size(50, 50);
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> tempTile in tempTiletypeList)
            {
                Vector tempLoc = new Vector(zeroX + 10, zeroY - 10 - counter*(tempSize.Height + 10));

                ObjObject tempObjObject = RenderLayerGame.CreateSquare(hudProgram, new Vector3(tempLoc.X, tempLoc.Y, 0),
                                                                       new Vector3(tempLoc.X + tempSize.Width,
                                                                                   tempLoc.Y + tempSize.Height, 0));
                tempObjObject.Material = tempTiletypeList[tempTile.Key].Material;

                tempObjList.Add(tempObjObject);
                counter++;
            }
            theTileObjects.AddRange(tempObjList);

            List<ObjHud> tempHudObjList = new List<ObjHud>();
            counter = 2;
            tempSize = new Size(50, 50);
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> tempTile in tempTiletypeList)
            {
                Vector2 tempLoc = new Vector2(10, 10 + counter*(tempSize.Height + 10));

                ObjHud tempObjObject = RenderLayerGame.CreateSquareHud(hudProgram, new Vector3(0, 0, 0),
                                                                       new Vector3(tempSize.Width, tempSize.Height, 0),
                                                                       ObjHud.Anchors.BottomRight, tempLoc, tempSize);
                tempObjObject.Size = tempSize;
                tempObjObject.UpdatePosition(Width, Height);
                tempObjObject.Material = tempTiletypeList[tempTile.Key].Material;


                tempHudObjList.Add(tempObjObject);
                counter++;
            }
            theHudObjects.AddRange(tempHudObjList);
        }

        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // bind the font program as well as the font texture
            Gl.UseProgram(hudProgram.ProgramID);
//            Gl.BindTexture(font.FontTexture);

//            if (objectList != null)
//            {
//                objectList.Draw();
//            }


//            if (theTileObjects != null)
//            {
//                foreach (ObjObject theTileObject in theTileObjects)
//                {
//                    theTileObject.Draw();
//                }
//            }

            foreach (ObjHud theHudObject in theHudObjects)
            {
                theHudObject.Draw(hudProgram);
            }
        }

        public override void OnReshape(int width, int height)
        {
            Width = width;
            Height = height;

            Gl.UseProgram(hudProgram.ProgramID);
            hudProgram["projection_matrix"].SetValue(Matrix4.CreateOrthographic(width, height, 0, 1000));

            foreach (ObjHud theHudObject in theHudObjects)
            {
                theHudObject.UpdatePosition(Width, Height);
            }


//
//            information.Position = new Vector2(-width / 2 + 10, height / 2 - font.Height - 10);
        }

        public override void OnClose()
        {
            if (theTileObjects != null)
            {
                foreach (ObjObject aObjObject in theTileObjects)
                {
                    aObjObject.Dispose();
                }
            }
            foreach (ObjHud aHudObject in theHudObjects)
            {
                aHudObject.Dispose();
            }

            if (objectList != null)
            {
                objectList.Dispose();
            }
            hudProgram.DisposeChildren = true;
            hudProgram.Dispose();
        }

        public override void OnMouse(int button, int state, int x, int y)
        {
        }

        public override void OnMove(int x, int y)
        {
        }

        public override void OnSpecialKeyboardDown(int key, int x, int y)
        {
        }

        public override void OnSpecialKeyboardUp(int key, int x, int y)
        {
        }

        public override void OnKeyboardDown(byte key, int x, int y)
        {
        }

        public override void OnKeyboardUp(byte key, int x, int y)
        {
        }

        #region Sample Shader

        private const string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    normal = (length(vertexNormal) == 0 ? vec3(0, 0, 0) : normalize((model_matrix * vec4(vertexNormal, 0)).xyz));
    uv = vertexUV;

   // gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
    gl_Position = projection_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        private const string FragmentShader = @"
#version 130

in vec3 normal;
in vec2 uv;

out vec4 fragment;

uniform vec3 diffuse;
uniform sampler2D texture;
uniform float transparency;
uniform bool useTexture;

void main(void)
{
    vec3 light_direction = normalize(vec3(1, 1, 0));
    float light = max(0.5, dot(normal, light_direction));
    vec4 sample = (useTexture ? texture2D(texture, uv) : vec4(1, 1, 1, 1));
    fragment = vec4(light * diffuse * sample.xyz, transparency * sample.a);
//    fragment = vec4(diffuse * sample.xyz, transparency * sample.a);
}
";

        #endregion
    }
}