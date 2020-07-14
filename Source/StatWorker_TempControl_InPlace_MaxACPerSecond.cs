using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWHS
{
    public class StatWorker_TempControl_InPlace_MaxACPerSecond : StatWorker
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
            if (base.ShouldShowFor(req))
            {
                if (!req.HasThing)
                {
                    return false;
                }
                return IsConcernedThing(req.Thing);
            }
            return false;
        }

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (req.Thing == null)
            {
                Log.Error("Getting " + this.GetType().FullName + " for " + req.Def.defName +  " without concrete thing. This always returns 1. This is a bug. Contact the dev.");
                return 1;
            }
            return RWHS_TempControl_InPlace.GetMaxACPerSecond(req, applyPostProcess);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            return new SEB("StatsReport_RWHS", "TemperaturePerSecond").ValueNoFormat(GetValueUnfinalized(optionalReq).ToString("0.###")).ToString();
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            StringBuilder stringBuilder = new StringBuilder();
            CompTempControl tempControl = req.Thing.TryGetComp<CompTempControl>();
            Thing tempController = req.Thing;

            float roomTemp = tempController.Position.GetTemperature(tempController.Map);
            float energyPerSecond = tempControl.Props.energyPerSecond; // the power of the radiator
            float roomSurface = tempController.Position.GetRoomGroup(tempController.Map).CellCount; 
            float coolingConversionRate = 4.16666651f; // Celsius cooled per Joules*Second*Meter^2  conversion rate
            float minTemp = 20;
            float maxTemp = 120;
            float efficiency = 1 - Mathf.Min(Mathf.Max((roomTemp - minTemp) / (maxTemp - minTemp), 0), 1);
            float maxACPerSecond = energyPerSecond * efficiency / roomSurface * coolingConversionRate; // max cooling power possible

            SEB seb = new SEB("StatsReport_RWHS");
            seb.Simple("AmbientRoomTemp", roomTemp);
            seb.Simple("EnergyPerSecond", energyPerSecond);
            seb.Simple("RoomSurface", roomSurface);
            seb.Simple("ACConversionRate", coolingConversionRate);
            seb.Full("LerpEfficiency", efficiency * 100, roomTemp, minTemp, maxTemp);
            seb.Full("MaxACPerSecond", maxACPerSecond, energyPerSecond, efficiency, roomSurface, coolingConversionRate);

            return seb.ToString();
        }
        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            yield return new Dialog_InfoCard.Hyperlink();
        }
    }
}