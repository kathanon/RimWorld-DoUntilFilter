using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DoUntilFilter {
    [HarmonyPatch]
    public static class Patches {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeDef), nameof(RecipeDef.WorkerCounter), MethodType.Getter)]
        public static void WorkerCounter(RecipeDef __instance, ref RecipeWorkerCounter ___workerCounterInt) {
            if (___workerCounterInt != null || !BillState.UseFor(__instance)) return;
            var clazz = __instance.workerCounterClass;
            if (clazz == typeof(RecipeWorkerCounter_MakeStoneBlocks)) {
                ___workerCounterInt = RecipeWorkerCounter_Defer.Stone(__instance);
            } else if (clazz == typeof(RecipeWorkerCounter_ButcherAnimals)) { 
                ___workerCounterInt = RecipeWorkerCounter_Defer.Butcher(__instance);
            } else if (clazz == typeof(RecipeWorkerCounter)) {
                ___workerCounterInt = RecipeWorkerCounter_Defer.Other(__instance);
            }
        }


        private static BillState currentGUIState = null;
        private static Listing_Standard currentListing = null;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Dialog_BillConfig), nameof(Dialog_BillConfig.DoWindowContents))]
        public static void DoWindowContents(Bill_Production ___bill) {
            if (___bill.repeatMode == BillRepeatModeDefOf.TargetCount) {
                currentGUIState = State.For(___bill);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Listing_Standard), nameof(Listing_Standard.BeginSection))]
        public static void BeginSection(Listing_Standard __result) {
            if (currentGUIState != null) {
                currentListing = __result;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Listing_Standard), nameof(Listing_Standard.Label), 
            typeof(string), typeof(float), typeof(string))]
        public static void Label() {
            if (currentListing != null) {
                var state = currentGUIState;
                var listing = currentListing;
                currentGUIState = null;
                currentListing = null;
                state.DoGUI(listing);
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bill_Production), nameof(Bill_Production.ExposeData))]
        public static void ExposeData(Bill_Production __instance) {
            State.For(__instance)?.ExposeData(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Bill_Production), nameof(Bill_Production.Clone))]
        public static void Clone(Bill_Production __instance, Bill __result) {
            State.For(__instance)?.CopyTo(__result as Bill_Production);
        }
    }
}
