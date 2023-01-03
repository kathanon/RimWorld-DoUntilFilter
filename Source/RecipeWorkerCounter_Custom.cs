using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public class RecipeWorkerCounter_Custom : RecipeWorkerCounter_Abs {
        private const int ID = 10;
        private ThingFilter filter;

        public static FloatMenuOption MenuOption(BillState state) 
            => new FloatMenuOption(Strings.MenuCustom, () => SetFor(state, true));

        public static void SetFor(BillState state, bool edit) {
            if (!(state.counter is RecipeWorkerCounter_Custom)) {
                state.counter = new RecipeWorkerCounter_Custom(state.bill);
            }
            if (edit) state.counter.Edit(state.bill);
        }

        public static RecipeWorkerCounter_Custom For(int id) {
            if (id == ID) return new RecipeWorkerCounter_Custom();
            return null;
        }

        public RecipeWorkerCounter_Custom() {
            filter = new ThingFilter();
            id = ID;
        }

        public RecipeWorkerCounter_Custom(Bill_Production bill) : this() {
            filter.CopyAllowancesFrom(ParentFilter(bill));
        }

        public override string Label => Strings.ButtonCustom;

        public override bool HasEdit => true;

        public override void Edit(Bill_Production bill) {
            var parent = ParentFilter(bill);
            if (parent != null) {
                Find.WindowStack.Add(new Dialog_Filter(filter, parent));
            }
        }

        public override void CopyTo(ref RecipeWorkerCounter_Abs counter, Bill_Production bill) {
            if (!(counter is RecipeWorkerCounter_Custom other)) {
                counter = other = new RecipeWorkerCounter_Custom(bill);
            }
            other.filter.CopyAllowancesFrom(filter);
        }

        protected override void ExposeData() {
            Scribe_Deep.Look(ref filter, "filter");
        }

        protected override HashSet<ThingDef> ToCountFor(Bill_Production bill) {
            ParentFilter(bill);
            return filter.AllowedThingDefs.ToHashSet();
        }

        private ThingFilter ParentFilter(Bill_Production bill) {
            productsFor = ProductsFuncFor(bill.recipe);
            if (productsFor != null) {
                ThingFilter parent = new ThingFilter();
                foreach (var thing in Products(bill.recipe.fixedIngredientFilter)) {
                    parent.SetAllow(thing, true);
                }

                var remove = filter.AllowedThingDefs.Where(x => !parent.Allows(x)).ToList();
                foreach (var thing in remove) {
                    filter.SetAllow(thing, false);
                }

                return parent;
            } else {
                Log.Error($"Bill {bill.LabelCap} has counter {bill.recipe.workerCounterClass.Name}, which is not supported.");
                return null;
            }
        }
    }
}
