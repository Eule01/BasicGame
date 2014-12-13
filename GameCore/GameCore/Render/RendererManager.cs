#region

using System.Collections.Generic;
using GameCore.Interface;
using GameCore.Render.GDI;
using GameCore.Render.OpenGl4CSharp;

#endregion

namespace GameCore.Render
{
    public class RendererManager
    {
        private readonly List<RendererBase> theRenderers = new List<RendererBase>();

        private RendererBase theRenderer;

        public RendererManager()
        {
            RendererGdi tempRenderer = new RendererGdi();
            theRenderers.Add(tempRenderer);

            RendererGdi2 tempRenderer2 = new RendererGdi2();
            theRenderers.Add(tempRenderer2);

            RendererOpenGl4CSharp tempRendererOpenGl4 = new RendererOpenGl4CSharp();
            theRenderers.Add(tempRendererOpenGl4);
        }

        public List<RendererBase> TheRenderers
        {
            get { return theRenderers; }
        }

        public RendererBase TheRenderer
        {
            get { return theRenderer; }
        }

        public void SetRenderer(int aIndex, GameStatus aGameStatus, UserInput aUserInput)
        {
            if (theRenderer != null)
            {
                theRenderer.Close();
            }
            theRenderer = theRenderers[aIndex];
            theRenderer.TheGameStatus = aGameStatus;
            theRenderer.TheUserInput = aUserInput;
            theRenderer.Start();
        }

        public void Close()
        {
            if (theRenderer != null)
            {
                theRenderer.Close();
            }          
        }
    }
}