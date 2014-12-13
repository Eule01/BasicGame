using GameCore.Interface;

namespace GameCore.Render
{
    public abstract class RendererBase
    {
        public GameStatus TheGameStatus;

        public UserInput TheUserInput;

        protected string name = "RendererBase";

        public RendererBase()
        {
        }

        public abstract void Start();

        public abstract void Close();

        public string Name
        {
            get { return name; }
        }

        public override string ToString()
        {
            return name;
        }
    }
}