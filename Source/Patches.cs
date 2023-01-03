using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DoUntilFilter {
    [HarmonyPatch]
    public static class Patches {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeWorkerCounter_ButcherAnimals), nameof(RecipeWorkerCounter.CountProducts))]
        public static bool CountProducts_Butcher(Bill_Production bill, ref int __result)
            => State.DoFunc(bill, x => x.CountProducts(bill), ref __result);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeWorkerCounter_MakeStoneBlocks), nameof(RecipeWorkerCounter.CountProducts))]
        public static bool CountProducts_Blocks(Bill_Production bill, ref int __result)
            => State.DoFunc(bill, x => x.CountProducts(bill), ref __result);


        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeWorkerCounter_ButcherAnimals), nameof(RecipeWorkerCounter.ProductsDescription))]
        public static bool ProductsDescription_Butcher(Bill_Production bill, ref string __result)
            => State.DoFunc(bill, x => x.ProductsDescription(bill), ref __result);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeWorkerCounter_MakeStoneBlocks), nameof(RecipeWorkerCounter.ProductsDescription))]
        public static bool ProductsDescription_Blocks(Bill_Production bill, ref string __result)
            => State.DoFunc(bill, x => x.ProductsDescription(bill), ref __result);


        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeWorkerCounter_ButcherAnimals), nameof(RecipeWorkerCounter.CanPossiblyStoreInStockpile))]
        public static bool CanPossiblyStoreInStockpile_Butcher(
                Bill_Production bill, Zone_Stockpile stockpile, ref bool __result)
            => State.DoFunc(bill, x => x.CanPossiblyStoreInStockpile(bill, stockpile), ref __result);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RecipeWorkerCounter_MakeStoneBlocks), nameof(RecipeWorkerCounter.CanPossiblyStoreInStockpile))]
        public static bool CanPossiblyStoreInStockpile_Blocks(
                Bill_Production bill, Zone_Stockpile stockpile, ref bool __result)
            => State.DoFunc(bill, x => x.CanPossiblyStoreInStockpile(bill, stockpile), ref __result);



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
