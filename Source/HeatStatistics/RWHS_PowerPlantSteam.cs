using RimWorld;
using Verse;

namespace RWHS;

public static class RWHS_PowerPlantSteam
{
    public static float GetHeatPushedPerSecond(StatRequest req, bool applyPostProcess = true)
    {
        var heatPerSecond = GetHeatPerSecond();
        if (req.Thing == null)
        {
            Log.Error($"{req.Def.label} has null object");
        }

        float surface = 25;
        var roomGroup = req.Thing?.Position.GetRoom(req.Thing.Map);
        if (roomGroup != null)
        {
            surface = roomGroup.CellCount; // TODO DEBUG THIS LINE
        }

        var heatPushedPerSecond = heatPerSecond / surface;
        return heatPushedPerSecond;
    }

    public static float GetHeatPerSecond()
    {
        // See decompiled : RimWorld.IntermittentSteamSprayer.SteamSprayerTick()
        // here we take the worst case: continuous long sprays
        // with this we calculate the amount of time between those sprays we are spraying
        // we then calculate the chance of having heat pushed on those ticks between.
        //float minIntervalBetweenSprays = 500;
        //float maxSprayTicks = 500;

        float heatPushDelay = 20;
        float heatPerPush = 40;
        float ticksPerSecond = 60;

        return heatPerPush / heatPushDelay * ticksPerSecond;
    }
}