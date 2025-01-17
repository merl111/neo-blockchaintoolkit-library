using System.Collections.Generic;
using Neo.SmartContract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neo.BlockchainToolkit.Models
{
    public readonly record struct ContractInfo(
        int Id,
        UInt160 Hash,
        string Name);

    public readonly record struct BranchInfo(
        uint Network,
        byte AddressVersion,
        uint Index,
        UInt256 IndexHash,
        UInt256 RootHash,
        IReadOnlyList<ContractInfo> Contracts)
    {
        public ProtocolSettings ProtocolSettings => ProtocolSettings.Default with
        {
            AddressVersion = AddressVersion,
            Network = Network,
        };

        public static BranchInfo Load(JObject json)
        {
            var network = json.Value<uint>("network");
            var addressVersion = json.Value<byte>("address-version");
            var index = json.Value<uint>("index");
            var indexHash = UInt256.Parse(json.Value<string>("index-hash"));
            var rootHash = UInt256.Parse(json.Value<string>("root-hash"));

            var contracts = new List<ContractInfo>();
            var contractsJson = json["contracts"] as JArray;
            if (contractsJson is not null)
            {
                foreach (var value in contractsJson)
                {
                    var id = value.Value<int>("id");
                    var hash = UInt160.Parse(value.Value<string>("hash"));
                    var name = value.Value<string>("name") ?? "";
                    contracts.Add(new ContractInfo(id, hash, name));
                }
            }
            return new BranchInfo(network, addressVersion, index, indexHash, rootHash, contracts);
        }

        public void WriteJson(JsonWriter writer)
        {
            using var _ = writer.WriteObject();
            writer.WriteProperty("network", Network);
            writer.WriteProperty("address-version", AddressVersion);
            writer.WriteProperty("index", Index);
            writer.WriteProperty("index-hash", $"{IndexHash}");
            writer.WriteProperty("root-hash", $"{RootHash}");
            using var __ = writer.WritePropertyArray("contracts");
            foreach (var contract in Contracts)
            {
                using var _c = writer.WriteObject();
                writer.WriteProperty("id", contract.Id);
                writer.WriteProperty("hash", $"{contract.Hash}");
                writer.WriteProperty("name", contract.Name);
            }
        }
    }
}
