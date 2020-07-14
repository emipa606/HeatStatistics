using RimWorld;
using UnityEngine;
using Verse;

namespace RWHS
{
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
            CompTempControl tempControl = req.Thing.TryGetComp<CompTempControl>();
            Thing tempController = req.Thing;

            float roomTemp = tempController.Position.GetTemperature(tempController.Map);
            float targetTemp = tempControl.targetTemperature;
            float targetTempDiff = targetTemp - roomTemp;
            float maxACPerSecond = GetMaxACPerSecond(req); // max cooling power possible
            bool isHeater = tempControl.Props.energyPerSecond > 0;
            if (isHeater)
            {
                return Mathf.Max(Mathf.Min(targetTempDiff, maxACPerSecond), 0);
            }
            else
            {
                return Mathf.Min(Mathf.Max(targetTempDiff, maxACPerSecond), 0);
            }
        }

        public static float GetMaxACPerSecond(StatRequest req, bool applyPostProcess = true)
        {
            CompTempControl tempControl = req.Thing.TryGetComp<CompTempControl>();
            Thing tempController = req.Thing;

            float roomTemp = tempController.Position.GetTemperature(tempController.Map);
            float energyPerSecond = tempControl.Props.energyPerSecond; // the power of the radiator
            float roomSurface = tempController.Position.GetRoomGroup(tempController.Map).CellCount;
            float coolingConversionRate = 4.16666651f; // Celsius cooled per Joules*Second*Meter^2  conversion rate
            float efficiency = 1 - Mathf.Min(Mathf.Max((roomTemp - 20) / (120 - 20), 0), 1);
            float maxACPerSecond = energyPerSecond * efficiency / roomSurface * coolingConversionRate ; // max cooling power possible
            return maxACPerSecond;
        }
    }
}