using RimWorld;
using UnityEngine;
using Verse;

namespace RWHS;
/* First step: see the patch which adds data to the xml comp data
 *
 * Second step of this mod: adding this
 * Displaying this information
 */

[StaticConstructorOnStartup]
public static class RWHS_TempControl_RoomExchange
{
    private static float getCurrentEfficiency(StatRequest req, bool applyPostProcess = true)
    {
        var tempController = req.Thing;

        var intVec32 = tempController.Position + IntVec3.North.RotatedBy(tempController.Rotation);
        var intVec31 = tempController.Position + IntVec3.South.RotatedBy(tempController.Rotation);

        var cooledRoomTemp = intVec31.GetTemperature(tempController.Map);
        var extRoomTemp = intVec32.GetTemperature(tempController.Map);
        var efficiencyLossPerDegree =
            1.0f / 130.0f; // SOS2 internal value, means loss of efficiency for each degree above targettemp, lose 50% at 65C above targetTemp, 100% at 130+
        var sidesTempGradient = extRoomTemp - (cooledRoomTemp < 40 ? cooledRoomTemp : 40);
        var efficiency = 1f - (sidesTempGradient * efficiencyLossPerDegree);
        return efficiency;
    }


    public static float GetCurrentAcPerSecond(StatRequest req, bool applyPostProcess = true)
    {
        var tempControl = req.Thing.TryGetComp<CompTempControl>();
        var tempController = req.Thing;

        var intVec31 = tempController.Position + IntVec3.South.RotatedBy(tempController.Rotation);

        var cooledRoomTemp = intVec31.GetTemperature(tempController.Map);
        var targetTemp = tempControl.targetTemperature;
        var targetTempDiff = targetTemp - cooledRoomTemp;
        var maxAcPerSecond = GetMaxAcPerSecond(req); // max cooling power possible
        var isHeater = tempControl.Props.energyPerSecond > 0;
        return isHeater
            ? Mathf.Max(Mathf.Min(targetTempDiff, maxAcPerSecond), 0)
            : Mathf.Min(Mathf.Max(targetTempDiff, maxAcPerSecond), 0);
    }

    public static float GetMaxAcPerSecond(StatRequest req, bool applyPostProcess = true)
    {
        var tempControl = req.Thing.TryGetComp<CompTempControl>();
        var tempController = req.Thing;

        var intVec31 = tempController.Position + IntVec3.South.RotatedBy(tempController.Rotation);

        var energyPerSecond = tempControl.Props.energyPerSecond; // the power of the radiator
        float roomSurface = intVec31.GetRoom(tempController.Map).CellCount;
        const float coolingConversionRate = 4.16666651f; // Celsius cooled per Joules*Second*Meter^2  conversion rate
        var efficiency = getCurrentEfficiency(req);
        var maxAcPerSecond =
            energyPerSecond * efficiency / roomSurface * coolingConversionRate; // max cooling power possible
        return maxAcPerSecond;
    }
}