﻿#region

using System;
using System.Collections.Generic;
using System.Drawing;
using GameCore.GameObjects;
using GameCore.Interface;
using GameCore.Map;
using GameCore.Utils;
using OpenGL;
using Tao.FreeGlut;

#endregion

namespace GameCore.Render.OpenGl4CSharp
{
    public class RenderLayerGame : RenderLayerBase
    {
        public Camera Camera;

        private ShaderProgram program;
        private ObjLoader objectList;
        private bool wireframe;
        private bool msaa;

        private bool camLeft, camRight, camForward, camBack, space;

        private bool camUp;
        private bool camDown;

        private bool mouseDown;
        private int downX, downY;
        private int prevX, prevY;
        public Vector3 MouseWorld = Vector3.Zero;

        private List<ObjObject> theTileObjects;
        private List<RenderGameObject> theRenderGameObjects;
        private RenderGameObject playerObjObject = null;

        /// <summary>
        ///     The near clipping distance.
        /// </summary>
        private const float ZNear = 0.1f;

        /// <summary>
        ///     The far clipping distance.
        /// </summary>
        private const float ZFar = 1000f;

        /// <summary>
        ///     Field of view of the camera
        /// </summary>
        private const float Fov = 0.45f;

        private Matrix4 projectionMatrix;

        private ObjMaterial pointMaterial;
        private Dictionary<Tile.TileIds, PlainBmpTexture> tileTextures;


        public RenderLayerGame(int width, int height, GameStatus theGameStatus, UserInput theUserInput)
            : base(width, height, theGameStatus, theUserInput)
        {
        }

        public override void OnLoad()
        {
            // create our shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

            // create our camera
            Camera = new Camera(new Vector3(0, 0, 30), Quaternion.Identity);
            Camera.SetDirection(new Vector3(0, 0, -1));

            // set up the projection and view matrix
            program.Use();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, (float) Width/Height, ZNear,
                                                                    ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
            program["model_matrix"].SetValue(Matrix4.Identity);

            pointMaterial = RenderObjects.CreatPlainMaterial(new Size(10, 10), program, Color.Red);

            objectList = new ObjLoader(program);
            // objectList = new ObjLoader("enterprise.obj", program);

            ObjMaterial tempMaterial = new ObjMaterial(program)
                {
                    DiffuseMap = new Texture(BitmapHelper.CreatBitamp(new Size(20, 20), new SolidBrush(Color.Green)))
                };

            tileTextures = RenderObjects.CreateTileTextures(new Size(20, 20), program);


            ObjObject tempObj = CreateCube(program, new Vector3(1, 1, 1), new Vector3(0, 0, 0));
            tempObj.Material = tileTextures[Tile.TileIds.Desert].Material;
            objectList.AddObject(tempObj);
            tempObj = CreateCube(program, new Vector3(3, 1, 1), new Vector3(2, 0, 0));
            tempObj.Material = tileTextures[Tile.TileIds.Road].Material;

            objectList.AddObject(tempObj);

            tempObj = CreateSquare(program, new Vector3(5, 1, 1), new Vector3(4, 0, 1));
            tempObj.Material = tempMaterial;
            objectList.AddObject(tempObj);

            tempObj = CreateSquare(program, new Vector3(-1, 1, 1), new Vector3(-2, 0, 0));
            objectList.AddObject(tempObj);

            theTileObjects = GetTileObjects();
            theRenderGameObjects = GetGameObjects();
        }


        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
//            if (msaa) Gl.Enable(EnableCap.Multisample);
//            else Gl.Disable(EnableCap.Multisample);

            // update our camera by moving it camForward to 5 units per second in each direction
            if (camBack) Camera.MoveRelative(Vector3.UnitZ*deltaTime*5);
            if (camForward) Camera.MoveRelative(-Vector3.UnitZ*deltaTime*5);
            if (camLeft) Camera.MoveRelative(-Vector3.UnitX*deltaTime*5);
            if (camRight) Camera.MoveRelative(Vector3.UnitX*deltaTime*5);
            if (space || camUp) Camera.MoveRelative(Vector3.Up*deltaTime*3);
            if (camDown) Camera.MoveRelative(-Vector3.Up*deltaTime*3);


            // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            Gl.UseProgram(program);
            program["view_matrix"].SetValue(Camera.ViewMatrix);
            program["model_matrix"].SetValue(Matrix4.Identity);

            // now draw the object file
            if (wireframe) Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            if (objectList != null)
            {
                objectList.Draw();
            }
            if (theTileObjects != null)
            {
                foreach (ObjObject theTileObject in theTileObjects)
                {
                    theTileObject.Draw();
                }
            }
            if (true)
            {
                // Draw a small test grid
                double delta = 5;
                double z = 0.1;
                Gl.PointSize(10);
                // For some reason EnableCap.PointSmooth = ((int)0x0B10), was commented out in OpenGL4CSharp.
                Gl.Enable(EnableCap.PointSmooth);

                // shift the mouse point a bit toward the camera
                Vector3 mousePoint = MouseWorld + ((Camera.Position - MouseWorld).Normalize())*0.01f;

                Vector3[] vertexData = new[]
                    {
                        mousePoint, new Vector3(0, 0, z),
                        new Vector3(delta, delta, z), new Vector3(0, delta, z), new Vector3(delta, 0, z),
                    };

                VBO<Vector3> vertices = new VBO<Vector3>(vertexData);

                if (pointMaterial != null) pointMaterial.Use();

                Gl.BindBufferToShaderAttribute(vertices, program, "vertexPosition");

                Gl.DrawElements(BeginMode.Points, vertices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                vertices.Dispose();
                vertices = null;
            }

            if (theRenderGameObjects != null)
            {
                foreach (RenderGameObject renderGameObject in theRenderGameObjects)
                {
                    renderGameObject.Draw(program);
                }
            }
        }

        public override void OnReshape(int width, int height)
        {
            Width = width;
            Height = height;


            Gl.UseProgram(program.ProgramID);
            //            projection_matrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, (float) Width/Height, 0.1f,
            //                                                                     1000f);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, (float) Width/Height, ZNear,
                                                                    ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
        }


        public override void OnClose()
        {
            if (objectList != null)
            {
                objectList.Dispose();
            }
            foreach (RenderGameObject aRenderGameObject in theRenderGameObjects)
            {
                aRenderGameObject.Dispose();
            }
            foreach (ObjObject aObjObject in theTileObjects)
            {
                aObjObject.Dispose();
            }
            if (pointMaterial != null) pointMaterial.Dispose();
            foreach (KeyValuePair<Tile.TileIds, PlainBmpTexture> plainBmpTexture in tileTextures)
            {
                plainBmpTexture.Value.Material.Dispose();
            }
            program.DisposeChildren = true;
            program.Dispose();
        }

        public override bool OnMouse(int button, int state, int x, int y)
        {
            if (button == Glut.GLUT_LEFT_BUTTON && state == Glut.GLUT_DOWN)
            {
                MouseWorld = ConvertScreenToWorldCoords(x, y, Camera.ViewMatrix, projectionMatrix, Camera.Position);
                Vector2 playerMouseVec =
                    (new Vector2(MouseWorld.x, MouseWorld.y) -
                     new Vector2(TheGameStatus.ThePlayer.Location.X, TheGameStatus.ThePlayer.Location.Y)).Normalize();

                TheGameStatus.ThePlayer.Orientation = new Vector(playerMouseVec.x, playerMouseVec.y);
            }
            else if (button == Glut.GLUT_RIGHT_BUTTON)
            {
                // this method gets called whenever a new mouse button event happens
                mouseDown = (state == Glut.GLUT_DOWN);

                // if the mouse has just been clicked then we hide the cursor and store the position
                if (mouseDown)
                {
                    Glut.glutSetCursor(Glut.GLUT_CURSOR_NONE);
                    prevX = downX = x;
                    prevY = downY = y;
                }
                else // unhide the cursor if the mouse has just been released
                {
                    Glut.glutSetCursor(Glut.GLUT_CURSOR_LEFT_ARROW);
                    Glut.glutWarpPointer(downX, downY);
                }
            }
            return true;
        }

        public override void OnMove(int x, int y)
        {
            // if the mouse move event is caused by glutWarpPointer then do nothing
            if (x == prevX && y == prevY) return;

            // move the camera when the mouse is down
            if (mouseDown)
            {
                float yaw = (prevX - x)*0.002f;
                Camera.Yaw(yaw);

                float pitch = (prevY - y)*0.002f;
                Camera.Pitch(pitch);

                prevX = x;
                prevY = y;
            }

            if (x < 0) Glut.glutWarpPointer(prevX = Width, y);
            else if (x > Width) Glut.glutWarpPointer(prevX = 0, y);

            if (y < 0) Glut.glutWarpPointer(x, prevY = Height);
            else if (y > Height) Glut.glutWarpPointer(x, prevY = 0);
        }

        public override void OnSpecialKeyboardDown(int key, int x, int y)
        {
            //            Console.WriteLine("Key: " + key);
            if (key == Glut.GLUT_KEY_UP) camForward = true;
            else if (key == Glut.GLUT_KEY_DOWN) camBack = true;
            else if (key == Glut.GLUT_KEY_RIGHT) camRight = true;
            else if (key == Glut.GLUT_KEY_LEFT) camLeft = true;
            else if (key == Glut.GLUT_KEY_PAGE_UP) camUp = true;
            else if (key == Glut.GLUT_KEY_PAGE_DOWN) camDown = true;
        }

        public override void OnSpecialKeyboardUp(int key, int x, int y)
        {
            if (key == Glut.GLUT_KEY_UP) camForward = false;
            else if (key == Glut.GLUT_KEY_DOWN) camBack = false;
            else if (key == Glut.GLUT_KEY_RIGHT) camRight = false;
            else if (key == Glut.GLUT_KEY_LEFT) camLeft = false;
            else if (key == Glut.GLUT_KEY_PAGE_UP) camUp = false;
            else if (key == Glut.GLUT_KEY_PAGE_DOWN) camDown = false;
            else if (key == Glut.GLUT_KEY_HOME)
                Camera.LookAt(new Vector3(playerObjObject.TheGameObject.Location.X,
                                          playerObjObject.TheGameObject.Location.Y, 0.0f));
            else if (key == Glut.GLUT_KEY_END)
            {
                RectangleF tempRec = TheGameStatus.TheMap.TheBoundingBox;
                Vector3 tempTopLeft = new Vector3(tempRec.Location.X, tempRec.Location.Y, 0.0f);
                Vector3 tempBottomRight = new Vector3(tempRec.Right, tempRec.Bottom, 0.0f);

                Camera.LookAtRectangle(tempTopLeft, tempBottomRight);
            }
        }

        public override void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 'w') TheUserInput.Forward = true;
            else if (key == 's') TheUserInput.Backward = true;
            else if (key == 'd') TheUserInput.Right = true;
            else if (key == 'a') TheUserInput.Left = true;
        }

        public override void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == 'w') TheUserInput.Forward = false;
            else if (key == 's') TheUserInput.Backward = false;
            else if (key == 'd') TheUserInput.Right = false;
            else if (key == 'a') TheUserInput.Left = false;
            else if (key == ' ') space = false;
            else if (key == 'q') wireframe = !wireframe;
            else if (key == 'm') msaa = !msaa;
        }

        #region Game objects

        private List<RenderGameObject> GetGameObjects()
        {
            List<RenderGameObject> tempTileObj = CreateRenderGameObjects();
            return tempTileObj;
        }

        private List<ObjObject> CreateGameObjects()
        {
            Dictionary<GameObject.ObjcetIds, PlainBmpTexture> gameObjectsTextures =
                RenderObjects.CreateGameObjectsTextures(new Size(20, 20), program);
            List<ObjObject> tempObjList = new List<ObjObject>();
            List<GameObject> gameObjects = TheGameStatus.GameObjects;


            foreach (GameObject gameObject in gameObjects)
            {
                Vector tempLoc = gameObject.Location;
                tempLoc -= new Vector(gameObject.Diameter*0.5f, gameObject.Diameter*0.5f);
                ObjObject tempObjObject = CreateCube(program, new Vector3(tempLoc.X, tempLoc.Y, 0),
                                                     new Vector3(tempLoc.X + gameObject.Diameter,
                                                                 tempLoc.Y + gameObject.Diameter, 1));
                tempObjObject.Material = gameObjectsTextures[gameObject.TheObjectId].Material;


                tempObjList.Add(tempObjObject);
            }
            return tempObjList;
        }

        private List<RenderGameObject> CreateRenderGameObjects()
        {
            Dictionary<GameObject.ObjcetIds, PlainBmpTexture> gameObjectsTextures =
                RenderObjects.CreateGameObjectsTextures(new Size(20, 20), program);
            List<RenderGameObject> tempObjList = new List<RenderGameObject>();
            List<GameObject> gameObjects = TheGameStatus.GameObjects;


            foreach (GameObject gameObject in gameObjects)
            {
                Vector tempLoc = new Vector(0.0f, 0.0f);
                tempLoc -= new Vector(gameObject.Diameter*0.5f, gameObject.Diameter*0.5f);
                RenderGameObject tempObjObject = CreateCube(program, new Vector3(tempLoc.X, tempLoc.Y, 0),
                                                            new Vector3(tempLoc.X + gameObject.Diameter,
                                                                        tempLoc.Y + gameObject.Diameter, 1));
                tempObjObject.Material = gameObjectsTextures[gameObject.TheObjectId].Material;

                tempObjObject.TheGameObject = gameObject;
                if (gameObject.TheObjectId == GameObject.ObjcetIds.Player)
                {
                    playerObjObject = tempObjObject;
                }
                tempObjList.Add(tempObjObject);
            }
            return tempObjList;
        }


        private List<ObjObject> GetTileObjects()
        {
            List<ObjObject> tempTileObj = CreateTiles();
            return tempTileObj;
        }

        private List<ObjObject> CreateTiles()
        {
            Dictionary<Tile.TileIds, PlainBmpTexture> tempTiletypeList =
                RenderObjects.CreateTileTextures(new Size(20, 20), program);
            List<ObjObject> tempObjList = new List<ObjObject>();
            IEnumerable<Tile> tempTiles = TheGameStatus.TheMap.Tiles;
            foreach (Tile tempTile in tempTiles)
            {
                Vector tempLoc = tempTile.Location;

                ObjObject tempObjObject = CreateSquare(program, new Vector3(tempLoc.X, tempLoc.Y, 0),
                                                       new Vector3(tempLoc.X + Tile.Size.X, tempLoc.Y + Tile.Size.Y, 0));
                tempObjObject.Material = tempTiletypeList[tempTile.TheTileId].Material;

                tempObjList.Add(tempObjObject);
            }
            return tempObjList;
        }

        #endregion

        #region Basic Objects

        /// <returns></returns>
        public static RenderGameObject CreateCube(ShaderProgram program, Vector3 min, Vector3 max)
        {
            RenderGameObject tempObj;
            Vector3[] vertex = new[]
                {
                    new Vector3(min.x, min.y, max.z),
                    new Vector3(max.x, min.y, max.z),
                    new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, max.y, max.z),
                    new Vector3(max.x, min.y, min.z),
                    new Vector3(max.x, max.y, min.z),
                    new Vector3(min.x, max.y, min.z),
                    new Vector3(min.x, min.y, min.z)
                };

            int[] element = new[]
                {
                    0, 1, 2, 1, 3, 2,
                    1, 4, 3, 4, 5, 3,
                    4, 7, 5, 7, 6, 5,
                    7, 0, 6, 0, 2, 6,
                    7, 4, 0, 4, 1, 0,
                    2, 3, 6, 3, 5, 6
                };

            tempObj = new RenderGameObject(vertex, element);
            return tempObj;
            //            return new VAO(program, new VBO<Vector3>(vertex), new VBO<int>(element, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead));
        }

        /// <returns></returns>
        public static ObjObject CreateSquare(ShaderProgram program, Vector3 min, Vector3 max)
        {
            ObjObject tempObj;
            Vector3[] vertex = new[]
                {
                    new Vector3(min.x, min.y, min.z),
                    new Vector3(max.x, min.y, min.z),
                    new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, max.y, max.z),
                };

            int[] element = new[]
                {
                    0, 1, 3,
                    0, 2, 3,
                };

            tempObj = new ObjObject(vertex, element);
            return tempObj;
        }

        /// <returns></returns>
        public static ObjHudButton CreateSquareHudButton(ShaderProgram program, Vector3 min, Vector3 max, ObjHudButton.Anchors anAnchor,
                                             Vector2 aPosition, Size aSize)
        {
            ObjHudButton tempObj;
            Vector3[] vertex = new[]
                {
                    new Vector3(min.x, min.y, min.z),
                    new Vector3(max.x, min.y, min.z),
                    new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, max.y, max.z),
                };

            int[] element = new[]
                {
                    0, 1, 3,
                    0, 2, 3,
                };

            tempObj = new ObjHudButton(vertex, element) { Anchor = anAnchor, Position = aPosition, Size = aSize };
            return tempObj;
        }

       /// <returns></returns>
        public static ObjHudPanel CreateSquareHudPanel(ShaderProgram program, Vector3 min, Vector3 max, ObjHudPanel.Anchors anAnchor,
                                             Vector2 aPosition, Size aSize)
        {
            ObjHudPanel tempObj;
            Vector3[] vertex = new[]
                {
                    new Vector3(min.x, min.y, min.z),
                    new Vector3(max.x, min.y, min.z),
                    new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, max.y, max.z),
                };

            int[] element = new[]
                {
                    0, 1, 3,
                    0, 2, 3,
                };

            tempObj = new ObjHudPanel(vertex, element) { Anchor = anAnchor, Position = aPosition, Size = aSize };
            return tempObj;
        }

        #endregion

        // functions:
        #region Mouse to World

        /// <summary>
        ///     http://gamedev.stackexchange.com/questions/51820/how-can-i-convert-screen-coordinatess-to-world-coordinates-in-opentk
        ///     https://sites.google.com/site/vamsikrishnav/gluunproject
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 ConvertScreenToWorldCoords(int x, int y)
        {
            int[] viewport = new int[4];
            Matrix4 modelViewMatrix, projectionMatrix;
            float[] modelViewMatrixF = new float[16];
            float[] projectionMatrixF = new float[16];
            Gl.GetFloatv(GetPName.ModelviewMatrix, modelViewMatrixF);
            modelViewMatrix = new Matrix4(modelViewMatrixF);
            Gl.GetFloatv(GetPName.ProjectionMatrix, projectionMatrixF);
            projectionMatrix = new Matrix4(projectionMatrixF);

            Gl.GetIntegerv(GetPName.Viewport, viewport);

            //Read the window z co-ordinate 
            //(the z value on that point in unit cube)		
            //            glReadPixels(x, viewport[3] - y, 1, 1,
            //     GL_DEPTH_COMPONENT, GL_FLOAT, &z);
            //
            //            float[] z = new float[1];
            int[] zInt = new int[1];
            Gl.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.DepthComponent,
                          PixelType.Float, zInt);
            byte[] bytes = BitConverter.GetBytes(zInt[0]);
            float z = BitConverter.ToSingle(bytes, 0);

            //            Gl.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
            //            Gl.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
            //            Gl.GetInteger(GetPName.Viewport, viewport);
            Vector3 mouse;
            mouse.x = x;
            //            mouse.y = viewport[3] - y;
            mouse.y = y;
            mouse.z = z;
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);
            Vector3 coords = new Vector3(vector.x, vector.y, vector.z);
            return coords;
        }

        /// <summary>
        ///     This one is nearly working perfectly. There is a small error which I think can be corrected using something like this
        ///     mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="modelViewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        public static Vector3 ConvertScreenToWorldCoordsNoDepth(int x, int y, Matrix4 modelViewMatrix,
                                                                Matrix4 projectionMatrix,
                                                                Vector3 cameraPosition)
        {
            int[] viewport = new int[4];
            Gl.GetIntegerv(GetPName.Viewport, viewport);

            int[] zInt = new int[1];
//            Gl.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.DepthComponent,
//                          PixelType.Float, zInt);
//            byte[] bytes = BitConverter.GetBytes(zInt[0]);
//            float z = BitConverter.ToSingle(bytes, 0);
            // http://www.songho.ca/opengl/gl_projectionmatrix.html
            // http://web.archive.org/web/20130416194336/http://olivers.posterous.com/linear-depth-in-glsl-for-real
            // The depth stored in the buffer [0 1].
//            float z_b = z;
            float z_b = 0.0f;

            // The depth in the normalized device coordinates [-1 1].
            float z_n = 2.0f*z_b - 1.0f;

            // The distance to the camera plane in grid units.
            float z_e = 2.0f*ZFar*ZNear/(ZFar + ZNear - (ZFar - ZNear)*(2.0f*z_b - 1.0f));

            Vector3 mouse;
            //            mouse.y = viewport[3] - y;
            //                        mouse.y =-( viewport[3] - y );
            //            mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);

            mouse.x = x;
            mouse.y = y; //B
            mouse.z = z_n; //C
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);

            Vector3 distanceVec = -cameraPosition + vector.Xyz;
            if (distanceVec.Length > ZFar)
            {
                Vector3 distNormVec = distanceVec.Normalize();
                vector.Xyz = cameraPosition + (distNormVec*ZFar*0.99f);
            }

            Vector3 coords = new Vector3(vector.x, vector.y, vector.z);

            return coords;
        }

        /// <summary>
        ///     This one is nearly working perfectly. There is a small error which I think can be corrected using something like this
        ///     mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="modelViewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        public static Vector3 ConvertScreenToWorldCoords(int x, int y, Matrix4 modelViewMatrix, Matrix4 projectionMatrix,
                                                         Vector3 cameraPosition)
        {
            int[] viewport = new int[4];
            Gl.GetIntegerv(GetPName.Viewport, viewport);

            int[] zInt = new int[1];
            Gl.ReadPixels(x, viewport[3] - y, 1, 1, PixelFormat.DepthComponent,
                          PixelType.Float, zInt);
            byte[] bytes = BitConverter.GetBytes(zInt[0]);
            float z = BitConverter.ToSingle(bytes, 0);

            // http://www.songho.ca/opengl/gl_projectionmatrix.html
            // http://web.archive.org/web/20130416194336/http://olivers.posterous.com/linear-depth-in-glsl-for-real
            // The depth stored in the buffer [0 1].
            float z_b = z;

            // The depth in the normalized device coordinates [-1 1].
            float z_n = 2.0f*z_b - 1.0f;

            // The distance to the camera plane in grid units.
            float z_e = 2.0f*ZFar*ZNear/(ZFar + ZNear - (ZFar - ZNear)*(2.0f*z_b - 1.0f));

            Vector3 mouse;
            //            mouse.y = viewport[3] - y;
            //                        mouse.y =-( viewport[3] - y );
            //            mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);

            mouse.x = x;
            mouse.y = y; //B
            mouse.z = z_n; //C
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);

            Vector3 distanceVec = -cameraPosition + vector.Xyz;
            if (distanceVec.Length > ZFar)
            {
                Vector3 distNormVec = distanceVec.Normalize();
                vector.Xyz = cameraPosition + (distNormVec*ZFar*0.99f);
            }

            Vector3 coords = new Vector3(vector.x, vector.y, vector.z);

            return coords;
        }

        /// <summary>
        ///     mouse.z has to be in normalized form [-1 1]
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="view"></param>
        /// <param name="viewport"></param>
        /// <param name="mouse"></param>
        /// <returns></returns>
        private static Vector4 UnProject(Matrix4 projection, Matrix4 view, Size viewport, Vector3 mouse)
        {
            Vector4 vec;

            vec.x = 2.0f*mouse.x/viewport.Width - 1; //B
            vec.y = -(2.0f*mouse.y/viewport.Height - 1); //B
            vec.z = mouse.z; //B
            vec.w = 1.0f;

            Matrix4 viewInv = view.Inverse();
            Matrix4 projInv = projection.Inverse();

            vec = vec*projInv*viewInv; //B

            if (vec.w > float.Epsilon || vec.w < float.Epsilon)
            {
                vec.x /= vec.w;
                vec.y /= vec.w;
                vec.z /= vec.w;
            }

            return vec;
        }

        #endregion


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

    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
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
}
";
    }
}