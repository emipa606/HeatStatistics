﻿using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RWHS;

public class StatWorker_HeatPusher_PushedPerSecond : StatWorker
{
    private static bool isConcernedThing(Thing thing)
    {
        return !(thing.TryGetComp<CompHeatPusher>() == null && thing.TryGetComp<CompHeatPusherPowered>() == null);
    }

    public override bool IsDisabledFor(Thing thing)
    {
        if (!base.IsDisabledFor(thing))
        {
            return !isConcernedThing(thing);
        }

        return true;
    }

    public override bool ShouldShowFor(StatRequest req)
    {
        if (!base.ShouldShowFor(req))
        {
            return false;
        }

        return req.HasThing && isConcernedThing(req.Thing);
    }

    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (req.Thing != null)
        {
            return RWHS_HeatPusher.GetHeatPushedPerSecond(req, applyPostProcess);
        }

        Log.Error(
            $"Getting {GetType().FullName} for {req.Def.defName} without concrete thing. This always returns 1. This is a bug. Contact the dev.");
        return 1;
    }

    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        var comp = req.Thing.TryGetComp<CompHeatPusher>();

        var heatPerSecond = comp.Props.heatPerSecond;
        float surface = comp.parent.PositionHeld.GetRoom(comp.parent.Map).CellCount;
        var heatPushedPerSecond = heatPerSecond / surface;

        var seb = new SEB("StatsReport_RWHS");
        seb.Simple("HeatPushedPerSecond", heatPerSecond);
        seb.Simple("RoomSurface", surface);
        seb.Full("HeatOutputPerSecond", heatPushedPerSecond, heatPerSecond, surface);

        return seb.ToString();
    }

    public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense,
        StatRequest optionalReq, bool finalized = true)
    {
        return new SEB("StatsReport_RWHS", "TemperaturePerSecond")
            .ValueNoFormat(GetValueUnfinalized(optionalReq).ToString("0.###")).ToString();
    }

    public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
    {
        yield return new Dialog_InfoCard.Hyperlink();
    }
}