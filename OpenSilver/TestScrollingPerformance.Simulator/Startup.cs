using OpenSilver.Simulator;
using System;

namespace TestScrollingPerformance.Simulator
{
    internal static class Startup
    {
        [STAThread]
        static int Main(string[] args)
        {
            return SimulatorLauncher.Start<App>();
        }
    }
}
