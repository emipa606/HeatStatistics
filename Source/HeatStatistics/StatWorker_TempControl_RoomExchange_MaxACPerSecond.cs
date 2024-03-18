using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RWHS;

public class StatWorker_TempControl_RoomExchange_MaxACPerSecond : StatWorker
{
    private bool IsConcernedThing(Thing thing)
    {
        return thing.TryGetComp<CompTempControl>() != null;
    }

    public override bool IsDisabledFor(Thing thing)
    {
        if (!base.IsDisabledFor(thing))
        {
            return !IsConcernedThing(thing);
        }

        return true;
    }

    public override bool ShouldShowFor(StatRequest req)
    {
        if (!base.ShouldShowFor(req))
        {
            return false;
        }

        return req.HasThing && IsConcernedThing(req.Thing);
    }

    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (req.Thing != null)
        {
            return RWHS_TempControl_RoomExchange.GetMaxACPerSecond(req, applyPostProcess);
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

        var intVec3_2 = tempController.Position + IntVec3.North.RotatedBy(tempController.Rotation);
        var intVec3_1 = tempController.Position + IntVec3.South.RotatedBy(tempController.Rotation);

        var cooledRoomTemp = intVec3_1.GetTemperature(tempController.Map);
        var extRoomTemp = intVec3_2.GetTemperature(tempController.Map);
        var efficiencyLossPerDegree =
            1.0f / 130.0f; // SOS2 internal value, means loss of efficiency for each degree above targettemp, lose 50% at 65C above targetTemp, 100% at 130+
        var energyPerSecond = tempControl.Props.energyPerSecond; // the power of the radiator
        float roomSurface = intVec3_1.GetRoom(tempController.Map).CellCount;
        var coolingConversionRate = 4.16666651f; // Celsius cooled per Joules*Second*Meter^2  conversion rate
        var sidesTempGradient = extRoomTemp - (cooledRoomTemp < 40 ? cooledRoomTemp : 40);
        var efficiency =
            1f - (sidesTempGradient * efficiencyLossPerDegree); // a negative value indicates heat generation
        var maxACPerSecond =
            energyPerSecond * efficiency / roomSurface *
            coolingConversionRate; // max cooling power possible, positive value indicates heat generation

        var seb = new SEB("StatsReport_RWHS");
        seb.Simple("CooledRoomTemp", cooledRoomTemp);
        seb.Simple("ExteriorRoomTemp", extRoomTemp);
        seb.Simple("EfficiencyLossPerDegree", efficiencyLossPerDegree);
        seb.Simple("EnergyPerSecond", energyPerSecond);
        seb.Simple("CooledRoomSurface", roomSurface);
        seb.Simple("ACConversionRate", coolingConversionRate);
        seb.Full("SidesTempGradient", sidesTempGradient, extRoomTemp, cooledRoomTemp);
        seb.Full("RelativeEfficiency", efficiency * 100, sidesTempGradient, efficiencyLossPerDegree);
        seb.Full("MaxACPerSecond", maxACPerSecond, energyPerSecond, efficiency, roomSurface, coolingConversionRate);

        return seb.ToString();
    }

    public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
    {
        yield return new Dialog_InfoCard.Hyperlink();
    }
}