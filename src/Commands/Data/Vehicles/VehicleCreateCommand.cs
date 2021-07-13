using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Vehicles
{
    /// <summary>
    ///     This command is sent when a vehicle is created (VehicleManager).
    /// </summary>
    /// Sent by:
    /// - VehicleHandler
    [ProtoContract]
    public class VehicleCreateCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        /// <summary>
        ///     The list of generated Array32 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(2)]
        public uint[] Array32Ids { get; set; }

        /// <summary>
        ///     The info index of the vehicle's prefab.
        /// </summary>
        [ProtoMember(3)]
        public ushort InfoIndex { get; set; }

        [ProtoMember(4)]
        public byte Type { get; set; }

        [ProtoMember(5)]
        public bool TransferToSource { get; set; }

        [ProtoMember(6)]
        public bool TransferToTarget { get; set; }

        [ProtoMember(7)]
        public Vector3 Position { get; set; }
    }
}
