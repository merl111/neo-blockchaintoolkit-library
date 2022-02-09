using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Neo;
using Neo.BlockchainToolkit.Persistence;
using Neo.Cryptography;

namespace test.bctklib
{
    public class CheckpointFixture : IDisposable
    {
        public readonly string CheckpointPath;
        public readonly uint Network = ProtocolSettings.Default.Network;
        public readonly byte AddressVersion = ProtocolSettings.Default.AddressVersion;
        public readonly UInt160 ScriptHash = new UInt160(Crypto.Hash160(Encoding.UTF8.GetBytes("sample-script-hash")));

        public CheckpointFixture()
        {
            var dbPath = RocksDbUtility.GetTempPath();
            try
            {
                using var db = RocksDbUtility.OpenDb(dbPath);
                using var store = new RocksDbStore(db);
                store.PutSeekData((0, 2), (1, 4));

                CheckpointPath = RocksDbUtility.GetTempPath();
                RocksDbUtility.CreateCheckpoint(db, CheckpointPath, Network, AddressVersion, ScriptHash);
            }
            finally
            {
                if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);
            }
        }

        public void Dispose()
        {
            if (File.Exists(CheckpointPath)) File.Delete(CheckpointPath);
        }

        public static IEnumerable<(byte[], byte[])> GetSeekData() => Utility.GetSeekData((0, 2), (1, 4));
    }
}