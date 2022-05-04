using RimWorld;
using Verse;

namespace RWHS;

public static class RWHS_HeatPusher
{
    public static float GetHeatPushedPerSecond(StatRequest req, bool applyPostProcess = true)
    {
        var comp = req.Thing.TryGetComp<CompHeatPusher>();

        var heatPerSecond = comp.Props.heatPerSecond;
        float surface = comp.parent.PositionHeld.GetRoom(comp.parent.Map).CellCount;
        var heatPushedPerSecond = heatPerSecond / surface;
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