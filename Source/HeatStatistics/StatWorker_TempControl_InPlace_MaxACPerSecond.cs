using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWHS;

public class StatWorker_TempControl_InPlace_MaxACPerSecond : StatWorker
{
    private static bool isConcernedThing(Thing thing)
    {
        return thing.TryGetComp<CompTempControl>() != null;
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
            return RWHS_TempControl_InPlace.GetMaxAcPerSecond(req, applyPostProcess);
        }

        Log.Error(
            $"Getting {GetType().FullName} for {req.Def.defName} without concrete thing. This always returns 1. This is a bug. Contact the dev.");
        return 1;
    }

    public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense,
        StatRequest optionalReq, bool finalized = true)
    {
        return new SEB("StatsReport_RWHS", "TemperaturePerSecond")
            .ValueNoFormat(GetValueUnfinalized(optionalReq).ToString("0.###")).ToString();
    }

    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        var tempControl = req.Thing.TryGetComp<CompTempControl>();
        var tempController = req.Thing;

        var roomTemp = tempController.Position.GetTemperature(tempController.Map);
        var energyPerSecond = tempControl.Props.energyPerSecond; // the power of the radiator
        float roomSurface = tempController.Position.GetRoom(tempController.Map).CellCount;
        const float coolingConversionRate = 4.16666651f; // Celsius cooled per Joules*Second*Meter^2  conversion rate
        const float minTemp = 20;
        const float maxTemp = 120;
        var efficiency = 1 - Mathf.Min(Mathf.Max((roomTemp - minTemp) / (maxTemp - minTemp), 0), 1);
        var maxAcPerSecond =
            energyPerSecond * efficiency / roomSurface * coolingConversionRate; // max cooling power possible

        var seb = new SEB("StatsReport_RWHS");
        seb.Simple("AmbientRoomTemp", roomTemp);
        seb.Simple("EnergyPerSecond", energyPerSecond);
        seb.Simple("RoomSurface", roomSurface);
        seb.Simple("ACConversionRate", coolingConversionRate);
        seb.Full("LerpEfficiency", efficiency * 100, roomTemp, minTemp, maxTemp);
        seb.Full("MaxACPerSecond", maxAcPerSecond, energyPerSecond, efficiency, roomSurface, coolingConversionRate);

        return seb.ToString();
    }

    public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
    {
        yield return new Dialog_InfoCard.Hyperlink();
    }
}