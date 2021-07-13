using CSM.Commands.Data.Vehicles;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Vehicles
{
    public class VehicleCreateHandler : CommandHandler<VehicleCreateCommand>
    {
        protected override void Handle(VehicleCreateCommand command)
        {
            VehicleInfo info = PrefabCollection<VehicleInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            VehicleManager.instance.CreateVehicle(out _, ref SimulationManager.instance.m_randomizer, info,
                command.Position, (TransferManager.TransferReason)command.Type, command.TransferToSource, command.TransferToTarget);

            ArrayHandler.StopApplying();
            IgnoreHelper.EndIgnore();
        }
    }
}
