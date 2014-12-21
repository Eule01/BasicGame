namespace GameCore.Render.OpenGl4CSharp
{
    public interface IRenderLayer
    {
        void OnLoad();
        void OnDisplay();
        void OnRenderFrame(float deltaTime);
        void OnReshape(int width, int height);
        void OnClose();

        #region UI

        void OnMouse(int button, int state, int x, int y);
        void OnMove(int x, int y);
        void OnSpecialKeyboardDown(int key, int x, int y);
        void OnSpecialKeyboardUp(int key, int x, int y);
        void OnKeyboardDown(byte key, int x, int y);
        void OnKeyboardUp(byte key, int x, int y);

        #endregion
    }
}