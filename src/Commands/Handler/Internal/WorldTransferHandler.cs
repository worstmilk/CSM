using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Util;

namespace CSM.Commands.Handler.Internal
{
    public class WorldTransferHandler : CommandHandler<WorldTransferCommand>
    {
        public WorldTransferHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(WorldTransferCommand command)
        {
            if (MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Downloading)
                return;

            if(command.Index == 0)
            {
                Log.Info("World has been received, preparing to load world.");
                SaveHelpers.StartWorldTransfer(command.Count);
            }

            Log.Info(string.Format("World chunk received {0}/{1} ({2}kb)", command.Index, command.Count, command.Chunk.Length / 1024));
            SaveHelpers.WriteWorldChunk(command.Index, command.Chunk);

            if(command.Index == (command.Count - 1))
            {
                Log.Info("All chunks received, unblock game");
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Loading;
                MultiplayerManager.Instance.CurrentClient.StopMainMenuEventProcessor();
                SaveHelpers.FinishWorldTransfer();
                SaveHelpers.LoadLevel();
                MultiplayerManager.Instance.UnblockGame(true);
                // See LoadingExtension for events after level loaded
            }
        }
    }
}
