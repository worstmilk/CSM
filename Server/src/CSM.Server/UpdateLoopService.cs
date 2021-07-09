﻿using CSM.Helpers;
using CSM.Networking;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace CSM.Server
{
    class UpdateLoopService : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                SpeedPauseHelper.SimulationStep();
                MultiplayerManager.Instance.ProcessEvents();

                // Keep processing events while there is anything in the queue
                if(!MultiplayerManager.Instance.CurrentServer.HasAnyEventsToProcess())
                    await Task.Delay(MultiplayerManager.Instance.CurrentServer.UpdateTime);
            }
        }
    }
}
