using ProtoBuf;

namespace CSM.Commands.Data.Vehicles
{
    /// <summary>
    ///     This command is sent when a vehicle is removed (VehicleManager).
    /// </summary>
    /// Sent by:
    /// - VehicleHandler
    [ProtoContract]
    public class VehicleRemoveCommand : CommandBase
    {
        /// <summary>
        ///     The id of the vehicle to be removed
        /// </summary>
        [ProtoMember(1)]
        public ushort VehicleId { get; set; }
    }
}
