﻿using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DoUntilFilter {
    public class BillState {
        private static HashSet<Type> defWorkers =
            [ typeof(RecipeWorkerCounter_MakeStoneBlocks), typeof(RecipeWorkerCounter_ButcherAnimals) ];

        public const float ButtonHeight = 26f;
        public const float Margin       =  6f;

        public static BillState Create(Bill_Production bill) {
            var state = new BillState {
                bill = bill,
                useDefCounter = UsesDefCounter(bill.recipe),
            };
            RecipeWorkerCounter_Abstract.SetDefaultFor(state);
            return state;
        }

        public static bool UseFor(RecipeDef recipe) 
            => recipe != null && (UsesDefCounter(recipe) || UsesTagsCounter(recipe));

        private static bool UsesDefCounter(RecipeDef recipe) 
            => defWorkers.Contains(recipe.workerCounterClass);

        private static bool UsesTagsCounter(RecipeDef recipe) 
            => recipe.ProducedThingDef?.HasComp<CompIngredients>() ?? false;

        public RecipeWorkerCounter_Abstract counter = null;
        public Bill_Production bill;

        private bool useDefCounter;

        public string Label => counter?.Label ?? Strings.VanillaFilter;

        public void DoGUI(Listing_Standard list) {
            var row = list.GetRect(ButtonHeight);
            row.y -= list.verticalSpacing;
            
            var tooltip = row;
            tooltip.y -= tooltip.height;
            counter?.DoTooltip(tooltip, bill);

            Text.Anchor = TextAnchor.MiddleLeft;
            float width = Text.CalcSize(Strings.CountingFilter).x;
            Widgets.Label(row.LeftPartPixels(width), Strings.CountingFilter);
            GenUI.ResetLabelAlign();
            row.xMin += width + Margin;

            if (counter?.HasEdit ?? false) {
                row = row.RightHalf();
                if (Widgets.ButtonText(row, Strings.Edit)) {
                    counter.Edit(bill);
                }
                row.x -= row.width;
            }

            if (Widgets.ButtonText(row, Label)) {
                Find.WindowStack.Add(new FloatMenu([ 
                    // Default
                    new FloatMenuOption(Strings.VanillaFilter, () => counter = null),
                    // Auto
                    AutoMenuOption,
                    // Custom...
                    CustomMenuOption,
                ]));
            }

            list.Gap(Margin);
        }

        private FloatMenuOption AutoMenuOption
            => useDefCounter ? RecipeWorkerCounter_DefAuto.MenuOption(this) : RecipeWorkerCounter_TagsAuto.MenuOption(this);

        private FloatMenuOption CustomMenuOption
            => useDefCounter ? RecipeWorkerCounter_DefCustom.MenuOption(this) : RecipeWorkerCounter_TagsCustom.MenuOption(this);

        public void CopyTo(Bill_Production bill) {
            var other = State.For(bill);
            if (other == null) return;

            if (counter == null) {
                other.counter = null;
            } else {
                counter.CopyTo(ref other.counter, bill);
            }
        }

        public void ExposeData(Bill_Production bill) {
            RecipeWorkerCounter_Abstract.Look(ref counter, Strings.ID);
            this.bill ??= bill;
            useDefCounter = UsesDefCounter(bill.recipe);
        }
    }
}
