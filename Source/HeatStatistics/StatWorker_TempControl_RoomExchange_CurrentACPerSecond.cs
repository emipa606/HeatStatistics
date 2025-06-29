using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWHS;

public class StatWorker_TempControl_RoomExchange_CurrentACPerSecond : StatWorker
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
            return RWHS_TempControl_RoomExchange.GetCurrentAcPerSecond(req, applyPostProcess);
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

        var intVec31 = tempController.Position + IntVec3.South.RotatedBy(tempController.Rotation);

        var cooledRoomTemp = intVec31.GetTemperature(tempController.Map);
        var targetTemp = tempControl.targetTemperature;
        var targetTempDiff = targetTemp - cooledRoomTemp;
        var maxAcPerSecond =
            RWHS_TempControl_RoomExchange.GetMaxAcPerSecond(req); // max cooling power possible            
        var isHeater = tempControl.Props.energyPerSecond > 0;
        var actualAc = isHeater
            ? Mathf.Max(Mathf.Min(targetTempDiff, maxAcPerSecond), 0)
            : Mathf.Min(Mathf.Max(targetTempDiff, maxAcPerSecond), 0);

        var seb = new SEB("StatsReport_RWHS");
        seb.Simple("CooledRoomTemp", cooledRoomTemp);
        seb.Simple("TargetTemperature", targetTemp);
        seb.Full("TargetTempDiff", targetTempDiff, targetTemp, cooledRoomTemp);
        seb.Simple("MaxACPerSecond", maxAcPerSecond);
        seb.Full(isHeater ? "ActualHeaterACPerSecond" : "ActualCoolerACPerSecond", actualAc, targetTempDiff,
            maxAcPerSecond);

        return seb.ToString();
    }


    public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
    {
        yield return new Dialog_InfoCard.Hyperlink();
    }
}