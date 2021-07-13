using ColossalFramework;
using CSM.Commands;
using CSM.Commands.Data.Vehicles;
using CSM.Commands.Data.Zones;
using CSM.Helpers;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSM.Injections
{
    /*
     * Notes:
     * - ZoneBlocks are created/destroyed by node segments, so we don't need to sync that
     * - We also don't need to sync the id, because the generated id is deterministic (the seed for the randomizer is the segment id)
     */

    //[HarmonyPatch]
    //public class VehicleAI
    //{

    //    public static IEnumerable<MethodBase> TargetMethods()
    //    {
    //        return typeof(VehicleAI)
    //            .Assembly
    //            .GetTypes()
    //            .Where(x => typeof(VehicleAI).IsAssignableFrom(x))
    //            .Select(x => x.GetMethod("SetSource"))
    //            .Cast<MethodBase>();
    //    }

    //    /// <summary>
    //    ///     This method is executed after ZoneBlock::RefreshZoning is called.
    //    ///     RefreshZoning is called after the player changed any of the zones in the block.
    //    /// </summary>
    //    /// <param name="blockID">The id of the modified block.</param>
    //    /// <param name="___m_zone1">Zone storage attribute 1 (three underscores to access an attribute of the class)</param>
    //    /// <param name="___m_zone2">Zone storage attribute 2</param>
    //    public static void Postfix(ushort vehicleID, ref Vehicle data, ushort sourceBuilding)
    //    {
    //        if (IgnoreHelper.IsIgnored())
    //            return;

    //        Command.SendToAll(new ZoneUpdateCommand
    //        {
    //            ZoneId = blockID,
    //            Zone1 = ___m_zone1,
    //            Zone2 = ___m_zone2
    //        });
    //    }


    //}


    [HarmonyPatch(typeof(VehicleManager))]
    [HarmonyPatch(nameof(VehicleManager.CreateVehicle))]
    public class CreateVehicle
    {
        public static void Prefix(out bool __state)
        {
            if (IgnoreHelper.IsIgnored())
            {
                __state = false;
                return;
            }

            __state = true;

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartCollecting();
        }

        public static void Postfix(bool __result, ref ushort vehicle, ref bool __state)
        {
            if (!__state)
                return;

            IgnoreHelper.EndIgnore();
            ArrayHandler.StopCollecting();

            if (__result)
            {
                Vehicle v = VehicleManager.instance.m_vehicles.m_buffer[vehicle];

                bool transferToSource = v.m_flags.IsFlagSet(Vehicle.Flags.TransferToSource);
                bool transferToTarget = v.m_flags.IsFlagSet(Vehicle.Flags.TransferToTarget);

                Command.SendToAll(new VehicleCreateCommand
                {
                    Array16Ids = ArrayHandler.Collected16,
                    Array32Ids = ArrayHandler.Collected32,
                    Position = v.m_frame0.m_position,
                    Type = v.m_transferType,
                    TransferToSource = transferToSource,
                    TransferToTarget = transferToTarget,
                    InfoIndex = v.m_infoIndex,
                });
            }
        }
    }

    [HarmonyPatch(typeof(VehicleManager))]
    [HarmonyPatch(nameof(VehicleManager.ReleaseVehicle))]
    public class ReleaseVehicle
    {
        public static void Postfix(ushort vehicle)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            Command.SendToAll(new VehicleRemoveCommand
            {
                VehicleId = vehicle
            });
        }
    }
}
