using RimWorld;
using Verse;


namespace RWHS
{
    public static class RWHS_HeatPusher
    {
        public static float GetHeatPushedPerSecond(StatRequest req, bool applyPostProcess = true)
        {
            CompHeatPusher comp = req.Thing.TryGetComp<CompHeatPusher>();

            float heatPerSecond = comp.Props.heatPerSecond;
            float surface = comp.parent.PositionHeld.GetRoomGroup(comp.parent.Map).CellCount;
            float heatPushedPerSecond = heatPerSecond / surface;
            return heatPushedPerSecond;
        }
        public static float GetMinWorkingTemp(StatRequest req, bool applyPostProcess = true)
        {
            return req.Thing.TryGetComp<CompHeatPusher>().Props.heatPushMinTemperature;
        }
        public static float GetMaxWorkingTemp(StatRequest req, bool applyPostProcess = true)
        {
            return req.Thing.TryGetComp<CompHeatPusher>().Props.heatPushMaxTemperature;
        }
        public static float GetAmbientWorkingTemp(StatRequest req, bool applyPostProcess = true)
        {
            return req.Thing.TryGetComp<CompHeatPusher>().parent.AmbientTemperature;
        }
    }
}
