#region

using System;
using GameCore;

#endregion

namespace TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameCore.GameCore theGameCore = new GameCore.GameCore();
            theGameCore.TheGameEventHandler += theGameCore_TheGameEventHandler;
            theGameCore.Start();
            Console.ReadLine();

        }

        private static void theGameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
            Console.WriteLine(args.ToString());
        }
    }
}