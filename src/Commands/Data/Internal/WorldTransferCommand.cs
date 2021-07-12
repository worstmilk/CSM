using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     This commands transfers a chunk of the save game.
    /// </summary>
    /// Sent by:
    /// - ConnectionRequestHandler
    [ProtoContract]
    public class WorldTransferCommand : CommandBase
    {

        [ProtoMember(1)]
        public int Index { get; set; }

        [ProtoMember(2)]
        public int Count { get; set; }

        /// <summary>
        ///     The serialized save game chunk.
        /// </summary>
        [ProtoMember(3)]
        public byte[] Chunk { get; set; }
    }
}
