using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RWHS
{
    public class StatWorker_TempControl_InPlace_CurrentACPerSecond : StatWorker
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
                Log.Error("Getting " + this.GetType().FullName + " for " + req.Def.defName + " without concrete thing. This always returns 1. This is a bug. Contact the dev.");
                return 1;
            }
            return RWHS_TempControl_InPlace.GetCurrentACPerSecond(req, applyPostProcess);
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            return new SEB("StatsReport_RWHS", "TemperaturePerSecond").ValueNoFormat(GetValueUnfinalized(optionalReq).ToString("0.###")).ToString();
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            CompTempControl tempControl = req.Thing.TryGetComp<CompTempControl>();
            Thing tempController = req.Thing;
            
            float roomTemp = tempController.Position.GetTemperature(tempController.Map);
            float targetTemp = tempControl.targetTemperature;
            float targetTempDiff = targetTemp - roomTemp;
            float maxACPerSecond = RWHS_TempControl_InPlace.GetMaxACPerSecond(req); // max cooling power possible
            bool isHeater = tempControl.Props.energyPerSecond > 0;
            float actualAC;
            if (isHeater)
            {
                actualAC = Mathf.Max(Mathf.Min(targetTempDiff, maxACPerSecond), 0);
            }
            else
            {
                actualAC = Mathf.Min(Mathf.Max(targetTempDiff, maxACPerSecond), 0);
            }

            SEB seb = new SEB("StatsReport_RWHS");
            seb.Simple("AmbientRoomTemp", roomTemp);
            seb.Simple("TargetTemperature", targetTemp);
            seb.Full("TargetTempDiff", targetTempDiff, targetTemp, roomTemp);
            seb.Simple("MaxACPerSecond", maxACPerSecond);
            if (isHeater)
            {
                seb.Full("ActualHeaterACPerSecond", actualAC, targetTempDiff, maxACPerSecond);
            }
            else
            {
                seb.Full("ActualCoolerACPerSecond", actualAC, targetTempDiff, maxACPerSecond);
            }

            return seb.ToString();
        }


        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            yield return new Dialog_InfoCard.Hyperlink();
        }
    }
}