#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using CodeToast;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Utils;
using OpenGL;
using Tao.FreeGlut;

#endregion

namespace GameCore.Render.OpenGl4CSharp
{
    public class RendererOpenGl4CSharp : RendererBase
    {
        private int width = 1280, height = 720;
        private Stopwatch watch;

        private ShaderProgram program;
        private ObjLoader objectList;
        private bool fullscreen;
        private bool wireframe;
        private bool msaa;
        private bool showInfo = true;

        private bool camLeft, camRight, camForward, camBack, space;

        private bool camUp;
        private bool camDown;

        private bool mouseDown;
        private int downX, downY;
        private int prevX, prevY;
        private Vector3 mouseWorld = Vector3.Zero;

        private Camera camera;

        private BMFont font;
        private ShaderProgram fontProgram;
        private FontVAO information;

        private bool exit;


        private List<ObjObject> theTileObjects;
        private List<RenderGameObject> theRenderGameObjects;
        private float fps = 30;

        public RendererOpenGl4CSharp()
        {
            name = "RendererOpenGl4CSharp";
        }

        private Matrix4 projection_matrix;
        private ObjMaterial pointMaterial;

        public override void Start()
        {
            Async.Do(delegate { StartOpenGl(); });
        }

        public void StartOpenGl()
        {
            exit = false;
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH | Glut.GLUT_ALPHA | Glut.GLUT_STENCIL |
                                     Glut.GLUT_MULTISAMPLE);

            // http://www.lighthouse3d.com/cg-topics/glut-and-freeglut/
            // Note: glutSetOption is only available with freeglut
            Glut.glutSetOption(Glut.GLUT_ACTION_ON_WINDOW_CLOSE, Glut.GLUT_ACTION_GLUTMAINLOOP_RETURNS);

            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL RendererOpenGl4CSharp");

            Gl.Enable(EnableCap.DepthTest);

            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);

            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutSpecialFunc(OnSpecialKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutSpecialUpFunc(OnSpecialKeyboardUp);

            Glut.glutCloseFunc(OnClose);
            Glut.glutReshapeFunc(OnReshape);

            // add our mouse callbacks for this tutorial
            Glut.glutMouseFunc(OnMouse);
            Glut.glutMotionFunc(OnMove);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // create our shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

            // create our camera
            camera = new Camera(new Vector3(0, 0, 30), Quaternion.Identity);
            camera.SetDirection(new Vector3(0, 0, -1));

            // set up the projection and view matrix
            program.Use();
            projection_matrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, (float) width/height, 0.1f,
                                                                     1000f);
            program["projection_matrix"].SetValue(projection_matrix);
            program["model_matrix"].SetValue(Matrix4.Identity);

            pointMaterial = RenderObjects.CreatPlainMaterial(new Size(10, 10), program, Color.Red);

            objectList = new ObjLoader(program);
            // objectList = new ObjLoader("enterprise.obj", program);

            ObjMaterial tempMaterial = new ObjMaterial(program)
                {
                    DiffuseMap = new Texture(BitmapHelper.CreatBitamp(new Size(20, 20), new SolidBrush(Color.Green)))
                };

            Dictionary<Tile.TileIds, PlainBmpTexture> tempTileList =
                RenderObjects.CreateTileTextures(new Size(20, 20), program);


            ObjObject tempObj = CreateCube(program, new Vector3(1, 1, 1), new Vector3(0, 0, 0));
            tempObj.Material = tempTileList[Tile.TileIds.Desert].Material;
            objectList.AddObject(tempObj);
            tempObj = CreateCube(program, new Vector3(3, 1, 1), new Vector3(2, 0, 0));
            tempObj.Material = tempTileList[Tile.TileIds.Road].Material;

            objectList.AddObject(tempObj);

            tempObj = CreateSquare(program, new Vector3(5, 1, 1), new Vector3(4, 0, 1));
            tempObj.Material = tempMaterial;
            objectList.AddObject(tempObj);

            tempObj = CreateSquare(program, new Vector3(-1, 1, 1), new Vector3(-2, 0, 0));
            objectList.AddObject(tempObj);

            theTileObjects = GetTileObjects();
            theRenderGameObjects = GetGameObjects();

            // load the bitmap font for this tutorial
            font = new BMFont(@".\Render\OpenGl4CSharp\font24.fnt", @".\Render\OpenGl4CSharp\font24.png");
            fontProgram = new ShaderProgram(BMFont.FontVertexSource, BMFont.FontFragmentSource);

            fontProgram.Use();
            fontProgram["ortho_matrix"].SetValue(Matrix4.CreateOrthographic(width, height, 0, 1000));
            fontProgram["color"].SetValue(new Vector3(1, 1, 1));

            information = font.CreateString(fontProgram, "OpenGL 4 C Sharp");

            watch = Stopwatch.StartNew();

            Glut.glutMainLoop();
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
//                Vector tempLoc = gameObject.Location;
                Vector tempLoc = new Vector(0.0f, 0.0f);
                tempLoc -= new Vector(gameObject.Diameter*0.5f, gameObject.Diameter*0.5f);
                RenderGameObject tempObjObject = CreateCube(program, new Vector3(tempLoc.X, tempLoc.Y, 0),
                                                            new Vector3(tempLoc.X + gameObject.Diameter,
                                                                        tempLoc.Y + gameObject.Diameter, 1));
                tempObjObject.Material = gameObjectsTextures[gameObject.TheObjectId].Material;

                tempObjObject.TheGameObject = gameObject;


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
            List<Tile> tempTiles = TheGameStatus.TheMap.Tiles;
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
//            return new VAO(program, new VBO<Vector3>(vertex), new VBO<int>(element, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead));
        }

        #endregion

        public override void Close()
        {
            exit = true;
            Thread.Sleep(100);
//            OnClose();
//            Glut.glutLeaveMainLoop();
        }

        private void OnDisplay()
        {
        }

        private void OnRenderFrame()
        {
            if (exit)
            {
                Glut.glutLeaveMainLoop();
            }
            else
            {
                watch.Stop();
                float deltaTime = (float) watch.ElapsedTicks/Stopwatch.Frequency;
                float tempfps = 1.0f/deltaTime;
                fps = fps*0.9f + tempfps*0.1f;
                // linear interpolate retained fps with this frames fps with a strong weighting to former.
                watch.Restart();

                if (msaa) Gl.Enable(EnableCap.Multisample);
                else Gl.Disable(EnableCap.Multisample);

                // update our camera by moving it camForward to 5 units per second in each direction
                if (camBack) camera.MoveRelative(Vector3.UnitZ*deltaTime*5);
                if (camForward) camera.MoveRelative(-Vector3.UnitZ*deltaTime*5);
                if (camLeft) camera.MoveRelative(-Vector3.UnitX*deltaTime*5);
                if (camRight) camera.MoveRelative(Vector3.UnitX*deltaTime*5);
                if (space || camUp) camera.MoveRelative(Vector3.Up*deltaTime*3);
                if (camDown) camera.MoveRelative(-Vector3.Up*deltaTime*3);


                // set camForward the viewport and clear the previous depth and color buffers
                Gl.Viewport(0, 0, width, height);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
                Gl.UseProgram(program);
                program["view_matrix"].SetValue(camera.ViewMatrix);
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
                    double delta = 5;
                    double z = 0.1;
//                    Gl.Enable(GetPName.PointSmooth);
//                    Gl
                    Gl.PointSize(10);


                    Vector3[] vertexData = new[] { mouseWorld, new Vector3(0, 0, z), new Vector3(delta, delta, z), new Vector3(0, delta, z), new Vector3(delta, 0, z), };
                    VBO<Vector3> vertices = new VBO<Vector3>(vertexData);

                    if (pointMaterial != null) pointMaterial.Use();

                    Gl.BindBufferToShaderAttribute(vertices, program, "vertexPosition");

                    Gl.DrawElements(BeginMode.Points, vertices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }

                if (theRenderGameObjects != null)
                {
                    foreach (RenderGameObject renderGameObject in theRenderGameObjects)
                    {
                        renderGameObject.Draw(program);
                    }
                }


                Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                // bind the font program as well as the font texture
                Gl.UseProgram(fontProgram.ProgramID);
                Gl.BindTexture(font.FontTexture);

                if (showInfo)
                {
                    // build this string every frame, since theta and phi can change
                    FontVAO vao = font.CreateString(fontProgram,
                                                    string.Format(
                                                        "FPS:   {0:0.00}, [{1:0.0},{2:0.0},{3:0.0}] cam [{4:0.0},{5:0.0},{6:0.0}]",
                                                        fps, mouseWorld.x, mouseWorld.y, mouseWorld.z, camera.Position.x,
                                                        camera.Position.y, camera.Position.z),
                                                    BMFont.Justification.Right);
                    vao.Position = new Vector2(width/2 - 10, height/2 - font.Height - 10);
                    vao.Draw();
                    vao.Dispose();
                }

                // draw the tutorial information, which is static
                information.Draw();

                Glut.glutSwapBuffers();
            }
        }


//GLvoid displayFPS(GLvoid)
//{
//	static long lastTime = SDL_GetTicks();
//	static long frames = 0;
//	static GLfloat fps = 0.0f;
//
//	int newTime = SDL_GetTicks();
//
//	if (newTime - lastTime > 100)
//	{
//		float newFPS = (float)frames / float(newTime - lastTime) * 1000.0f;
//
//		fps = (fps + newFPS) / 2.0f;
//
//		//Show FPS in window title
//		char title[80];
//		sprintf(title, "OpenGl Demo - %.2f", fps);
//		SDL_WM_SetCaption(title, NULL);
//		
//		lastTime = newTime;
//		frames = 0;
//	}
//	frames++;
//}


        private void OnReshape(int width, int height)
        {
            this.width = width;
            this.height = height;

            Gl.UseProgram(program.ProgramID);
            projection_matrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, (float) width/height, 0.1f,
                                                                     1000f);
            program["projection_matrix"].SetValue(projection_matrix);

            Gl.UseProgram(fontProgram.ProgramID);
            fontProgram["ortho_matrix"].SetValue(Matrix4.CreateOrthographic(width, height, 0, 1000));

            information.Position = new Vector2(-width/2 + 10, height/2 - font.Height - 10);
        }

        private void OnClose()
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
            program.DisposeChildren = true;
            program.Dispose();
            fontProgram.DisposeChildren = true;
            fontProgram.Dispose();
            font.FontTexture.Dispose();
            information.Dispose();
        }

        #region Controls

        private void OnMouse(int button, int state, int x, int y)
        {
            if (button == Glut.GLUT_LEFT_BUTTON && state == Glut.GLUT_DOWN)
            {
                mouseWorld = ConvertScreenToWorldCoords(x, y, camera.ViewMatrix, projection_matrix, camera.Position);
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
        }

        private void OnMove(int x, int y)
        {
            // if the mouse move event is caused by glutWarpPointer then do nothing
            if (x == prevX && y == prevY) return;

            // move the camera when the mouse is down
            if (mouseDown)
            {
                float yaw = (prevX - x)*0.002f;
                camera.Yaw(yaw);

                float pitch = (prevY - y)*0.002f;
                camera.Pitch(pitch);

                prevX = x;
                prevY = y;
            }

            if (x < 0) Glut.glutWarpPointer(prevX = width, y);
            else if (x > width) Glut.glutWarpPointer(prevX = 0, y);

            if (y < 0) Glut.glutWarpPointer(x, prevY = height);
            else if (y > height) Glut.glutWarpPointer(x, prevY = 0);
        }

        private void OnSpecialKeyboardDown(int key, int x, int y)
        {
//            Console.WriteLine("Key: " + key);
            if (key == Glut.GLUT_KEY_UP) camForward = true;
            else if (key == Glut.GLUT_KEY_DOWN) camBack = true;
            else if (key == Glut.GLUT_KEY_RIGHT) camRight = true;
            else if (key == Glut.GLUT_KEY_LEFT) camLeft = true;
            else if (key == Glut.GLUT_KEY_PAGE_UP) camUp = true;
            else if (key == Glut.GLUT_KEY_PAGE_DOWN) camDown = true;
        }

        private void OnSpecialKeyboardUp(int key, int x, int y)
        {
            if (key == Glut.GLUT_KEY_UP) camForward = false;
            else if (key == Glut.GLUT_KEY_DOWN) camBack = false;
            else if (key == Glut.GLUT_KEY_RIGHT) camRight = false;
            else if (key == Glut.GLUT_KEY_LEFT) camLeft = false;
            else if (key == Glut.GLUT_KEY_PAGE_UP) camUp = false;
            else if (key == Glut.GLUT_KEY_PAGE_DOWN) camDown = false;
        }

        private void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 'w') TheUserInput.Forward = true;
            else if (key == 's') TheUserInput.Backward = true;
            else if (key == 'd') TheUserInput.Right = true;
            else if (key == 'a') TheUserInput.Left = true;
//            else if (key == 27) Glut.glutLeaveMainLoop();
//            else
//            {
//                //                char c = Convert.ToChar(key);
//                //                string b = Encoding.ASCII.GetString(new byte[] { (byte)key });
//                //                Console.WriteLine("Key: " + key + " : " + b);
//                //                Console.WriteLine("Key: " + key);
//            }
        }

        private void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == 'w') TheUserInput.Forward = false;
            else if (key == 's') TheUserInput.Backward = false;
            else if (key == 'd') TheUserInput.Right = false;
            else if (key == 'a') TheUserInput.Left = false;
            else if (key == ' ') space = false;
            else if (key == 'q') wireframe = !wireframe;
            else if (key == 'm') msaa = !msaa;
            else if (key == 'i') showInfo = !showInfo;
            else if (key == 'f')
            {
                fullscreen = !fullscreen;
                if (fullscreen) Glut.glutFullScreen();
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                }
            }
        }

        #endregion

        // functions:
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


        public static Vector3 ConvertScreenToWorldCoords(int x, int y, Matrix4 modelViewMatrix, Matrix4 projectionMatrix,
                                                         Vector3 viewPosition)
        {
            int[] viewport = new int[4];

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
//                        mouse.y =-( viewport[3] - y );
//            mouse.Y = y + (ClientRectangle.Height - glview.Size.Height);
            mouse.y = y; //B
//            mouse.z = 0;


            mouse.z = z; // B
            Vector4 vector = UnProject(projectionMatrix, modelViewMatrix, new Size(viewport[2], viewport[3]), mouse);
//            Vector3 coords = new Vector3(vector.x , vector.y , vector.z);
//            Vector3 coords = new Vector3(vector.x , vector.y , -vector.z);
            Vector3 coords = new Vector3(vector.x - viewPosition.x, vector.y - viewPosition.y, vector.z + viewPosition.z);
//            Vector3 coords = new Vector3(vector.x - viewPosition.x, vector.y - viewPosition.y, vector.z );
            return coords;
        }

        private static Vector4 UnProject(Matrix4 projection, Matrix4 view, Size viewport, Vector3 mouse)
        {
            Vector4 vec;

            vec.x = 2.0f*mouse.x/viewport.Width - 1; //B
//            vec.x = mouse.x/viewport.Width - 1;
//            vec.y = (mouse.y/(float) viewport.Height - 1);
//            vec.y = -(mouse.y/(float) viewport.Height - 1);
//            vec.y = 2.0f * mouse.y / (float)viewport.Height + 1;
//            vec.x = 2.0f*mouse.x/(float) viewport.Width - 1;
            vec.y = -(2.0f*mouse.y/viewport.Height - 1); //B
//            vec.y = -(2.0f*mouse.y/viewport.Height + 1); 
//            vec.y = -(2.0f*mouse.y/viewport.Height + 1);
//            vec.y = -(mouse.y/viewport.Height - 1);
//            vec.y = -(vec.y = 2.0f*mouse.y/(float) viewport.Height) + 1;

//            vec.y = -(2.0f*mouse.y/(float) viewport.Height) + 1;
            vec.z = mouse.z;
            vec.w = 1.0f;

            Matrix4 viewInv = view.Inverse();
            Matrix4 projInv = projection.Inverse();

//            vec = projInv*vec;
//            vec = viewInv*vec;
//            vec = vec*projInv; //B
//            vec = vec*viewInv; //B
            vec = vec*projInv*viewInv; //B

//            Matrix4 viewInv = Matrix4.Invert(view);
//            Matrix4 projInv = Matrix4.Invert(projection);
//
//            Vector4.Transform(ref vec, ref projInv, out vec);
//            Vector4.Transform(ref vec, ref viewInv, out vec);
//
            if (vec.w > float.Epsilon || vec.w < float.Epsilon)
            {
                vec.x /= vec.w;
                vec.y /= vec.w;
                vec.z /= vec.w;
            }

            return vec;
        }


//        // http://www.opentk.com/node/1276
//        public static Vector4 UnProject(ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse)
//        {
//            Vector4 vec;
//
//            vec.x = 2.0f * mouse.x / (float)viewport.Width - 1;
//            vec.y = -(2.0f * mouse.y / (float)viewport.Height - 1);
//            vec.z = 0;
//            vec.w = 1.0f;
//
//            Matrix4 viewInv = view.Inverse();
//            Matrix4 projInv = projection.Inverse();
//
////            Matrix4 viewInv = Matrix4.Invert(view);
////            Matrix4 projInv = Matrix4.Invert(projection);
////
//  
//            Gl.GetDoublev();
//            vec = vec*projInv;
//            Vector4.(ref vec, ref projInv, out vec);
//            Vector4.Transform(ref vec, ref viewInv, out vec);
////            Vector4.Transform(ref vec, ref projInv, out vec);
////            Vector4.Transform(ref vec, ref viewInv, out vec);
//
//            if (vec.w > float.Epsilon || vec.w < float.Epsilon)
//            {
//                vec.x /= vec.w;
//                vec.y /= vec.w;
//                vec.z /= vec.w;
//            }
//
//            return vec;
//        }


        private static string VertexShader = @"
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

        private static string FragmentShader = @"
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

        #region Sample Shader

        public static string vertexShaderSource = @"
#version 330

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
uniform mat4 view_matrix;
uniform float animation_factor;

in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv;

out vec2 uv;

void main(void)
{
  vec4 pos2 = projection_matrix * modelview_matrix * vec4(in_normal, 1);
  vec4 pos1 = projection_matrix * modelview_matrix * vec4(in_position, 1);

  uv = in_uv;
  
  gl_Position = mix(pos2, pos1, animation_factor);
}";

        public static string fragmentShaderSource = @"
#version 330

uniform sampler2D active_texture;

in vec2 uv;

out vec4 out_frag_color;

void main(void)
{
  out_frag_color = mix(texture2D(active_texture, uv), vec4(1, 1, 1, 1), 0.05);
}";

        #endregion
    }
}