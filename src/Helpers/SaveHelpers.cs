using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using CSM.Models;
using CSM.Networking;
using CSM.Util;
using System;
using System.IO;
using System.Threading;

namespace CSM.Helpers
{
    /// <summary>
    ///     Helpers for loading and saving games, especially over network.
    /// </summary>
    public static class SaveHelpers
    {
        private const string SYNC_NAME_SERVER = "CSM_SyncSave";
        private const string SYNC_NAME_CLIENT = "CSM_SyncSave_Client";
        private const int CHUNK_SIZE = 64 * 1024; //64K bytes
        private static object _saveLock = new object();

        public static float CurrentProcess { get; private set; }
        public static float CurrentMaxProcess { get; private set; }

        private static FileStream _saveFileStream;

        /// <summary>
        ///     Save a level to the local save folder where it can then be sent to all clients.
        /// </summary>
        public static void SaveServerLevel()
        {
            SavePanel sp = UIView.library.Get<SavePanel>("SavePanel");
            if (sp != null)
            {
                sp.SaveGame(SYNC_NAME_SERVER);
            }
        }

        public static bool IsSaving()
        {
            return SavePanel.isSaving;
        }

        public static string GetSavePath()
        {
            if(MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                var fileName = Path.ChangeExtension(SYNC_NAME_SERVER, PackageManager.packageExtension);
                return Path.Combine(DataLocation.saveLocation, fileName);
            }
            else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                var fileName = Path.ChangeExtension(SYNC_NAME_CLIENT, PackageManager.packageExtension);
                return Path.Combine(DataLocation.saveLocation, fileName);
            }
            return null;
        }

        public static int GetWorldChunkCount()
        {
            string path = GetSavePath();
            var fileInfo = new FileInfo(path);
            var totalChunks = fileInfo.Length / (double)CHUNK_SIZE;
            return (int)Math.Ceiling(totalChunks);
        }

        public static byte[] GetWorldChunk(int chunkIndex)
        {
            string path = GetSavePath();
            byte[] chunk = new byte[CHUNK_SIZE];
            using (var fileStream = File.OpenRead(path))
            {
                fileStream.Position = ConvertChunkIndexToOffset(chunkIndex);
                int readed = fileStream.Read(chunk, 0, CHUNK_SIZE);
                Array.Resize(ref chunk, readed);
            }
            return chunk;
        }

        public static void StartWorldTransfer(int chunkCount)
        {
            string path = GetSavePath();
            File.Delete(path);
            _saveFileStream = File.Create(path);
            CurrentMaxProcess = chunkCount;
            CurrentProcess = 0;
        }

        public static void WriteWorldChunk(int chunkIndex, byte[] chunk)
        {
            lock (_saveLock)
            {
                _saveFileStream.Position = ConvertChunkIndexToOffset(chunkIndex);
                _saveFileStream.Write(chunk, 0, chunk.Length);

                CurrentProcess = chunkIndex;
            }
        }

        public static void FinishWorldTransfer()
        {
            _saveFileStream.Close();
            _saveFileStream.Dispose();
        }

        private static int ConvertChunkIndexToOffset(int chunkIndex)
            => chunkIndex * CHUNK_SIZE;

        /// <summary>
        ///     Load the world byte array sent by the server
        /// </summary>
        public static void LoadLevel()
        {
            Log.Info($"Preparing to load world ({GetSavePath()})...");

            // Load the package
            Package package = new Package(SYNC_NAME_CLIENT, GetSavePath());

            // Ensure that the LoadingManager is ready.
            // Don't know if thats really necessary but doesn't hurt either. - root#0042
            Log.Info($"Ensure loading manager ready");
            Singleton<LoadingManager>.Ensure();

            // Get the meta data
            Log.Info($"Get package meta data");
            Package.Asset asset = package.Find(package.packageMainAsset);

            Log.Info($"Initiate SaveGameMetaData");
            SaveGameMetaData metaData = asset.Instantiate<SaveGameMetaData>();

            // Build the simulation
            Log.Info($"Building simulation");
            SimulationMetaData simulation = new SimulationMetaData()
            {
                m_CityName = metaData.cityName,
                m_updateMode = SimulationManager.UpdateMode.LoadGame,
                m_environment = UIView.GetAView().panelsLibrary.Get<LoadPanel>("LoadPanel").m_forceEnvironment
            };

            // Load the level
            Log.Info("Telling the loading manager to load the level");
            Singleton<LoadingManager>.instance.LoadLevel(metaData.assetRef, "Game", "InGame", simulation, false);
        }
    }
}
