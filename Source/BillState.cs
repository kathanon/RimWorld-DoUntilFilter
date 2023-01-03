using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DoUntilFilter {
    public class BillState {
        public const float ButtonHeight = 26f;
        public const float Margin       =  6f;

        public static BillState Create(Bill_Production bill) {
            var state = new BillState {
                bill = bill,
            };
            RecipeWorkerCounter_Auto.SetFor(state);
            return state;
        }

        public RecipeWorkerCounter_Abs counter = null;
        public Bill_Production bill;

        public string Label => counter?.Label ?? Strings.VanillaFilter;

        public void DoGUI(Listing_Standard list) {
            var row = list.GetRect(ButtonHeight);
            row.y -= list.verticalSpacing;
            
            var tooltip = row;
            tooltip.y -= tooltip.height;
            counter?.Tooltip(tooltip, bill);

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
                Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption> { 
                        // Default
                        new FloatMenuOption(Strings.VanillaFilter, () => counter = null),
                        // Auto
                        RecipeWorkerCounter_Auto.MenuOption(this),
                        // Custom...
                        RecipeWorkerCounter_Custom.MenuOption(this),
                    }));
            }

            list.Gap(Margin);
        }

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
            RecipeWorkerCounter_Abs.Look(ref counter, Strings.ID);
            this.bill ??= bill;
        }
    }
}
