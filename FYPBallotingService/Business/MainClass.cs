using System;
using System.ServiceProcess;
using FYPBallotingService;

namespace FYPBallotingService.Business
{
    class MainClass
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                FYPBallotingService myService = new FYPBallotingService();
                myService.OnServiceStart();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new FYPBallotingService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
