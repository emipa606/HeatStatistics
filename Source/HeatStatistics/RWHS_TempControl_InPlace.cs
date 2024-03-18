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
public static class RWHS_TempControl_InPlace
{
    public static float GetCurrentACPerSecond(StatRequest req, bool applyPostProcess = true)
    {
        var tempControl = req.Thing.TryGetComp<CompTempControl>();
        var tempController = req.Thing;

        var roomTemp = tempController.Position.GetTemperature(tempController.Map);
        var targetTemp = tempControl.targetTemperature;
        var targetTempDiff = targetTemp - roomTemp;
        var maxACPerSecond = GetMaxACPerSecond(req); // max cooling power possible
        var isHeater = tempControl.Props.energyPerSecond > 0;
        return isHeater
            ? Mathf.Max(Mathf.Min(targetTempDiff, maxACPerSecond), 0)
            : Mathf.Min(Mathf.Max(targetTempDiff, maxACPerSecond), 0);
    }

    public static float GetMaxACPerSecond(StatRequest req, bool applyPostProcess = true)
    {
        var tempControl = req.Thing.TryGetComp<CompTempControl>();
        var tempController = req.Thing;

        var roomTemp = tempController.Position.GetTemperature(tempController.Map);
        var energyPerSecond = tempControl.Props.energyPerSecond; // the power of the radiator
        float roomSurface = tempController.Position.GetRoom(tempController.Map).CellCount;
        var coolingConversionRate = 4.16666651f; // Celsius cooled per Joules*Second*Meter^2  conversion rate
        var efficiency = 1 - Mathf.Min(Mathf.Max((roomTemp - 20) / (120 - 20), 0), 1);
        var maxACPerSecond =
            energyPerSecond * efficiency / roomSurface * coolingConversionRate; // max cooling power possible
        return maxACPerSecond;
    }
}