using HarmonyLib;
using RimWorld;
using Verse;

namespace ShorterXenogermComa
{
    public class ShorterXenogermComaMod : Mod
    {
        public ShorterXenogermComaMod(ModContentPack content) : base(content)
        {
            new Harmony("codexvn.ShorterXenogermComa").PatchAll();
        }
    }

    [HarmonyPatch(typeof(SurgeryOutcome_HediffWithDuration), nameof(SurgeryOutcome_HediffWithDuration.Apply))]
    public static class Patch_SurgeryOutcome_XenogermComa
    {
        static void Postfix(bool __result, HediffDef ___hediff, Pawn patient)
        {
            if (!__result || ___hediff != HediffDefOf.XenogerminationComa)
                return;
            var coma = patient.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogerminationComa);
            if (coma != null)
                coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = 1;
        }
    }

    [HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ReimplantXenogerm))]
    public static class Patch_ReimplantXenogerm_Coma
    {
        static void Postfix(Pawn recipient)
        {
            var coma = recipient.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogerminationComa);
            if (coma != null)
                coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = 1;
        }
    }
}
