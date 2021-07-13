using CSM.Commands.Data.Vehicles;
using CSM.Helpers;

namespace CSM.Commands.Handler.Vehicles
{
    public class VehicleRemoveHandler : CommandHandler<VehicleRemoveCommand>
    {
        protected override void Handle(VehicleRemoveCommand command)
        {
            IgnoreHelper.StartIgnore();
            VehicleManager.instance.ReleaseVehicle(command.VehicleId);
            IgnoreHelper.EndIgnore();
        }
    }
}
