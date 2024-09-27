using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public class RecipeWorkerCounter_DefCustom : RecipeWorkerCounter_DefAbstract {
        private const int ID = 10;
        private ThingFilter filter;

        public static FloatMenuOption MenuOption(BillState state) 
            => new(Strings.MenuCustom, () => SetFor(state, true));

        public static void SetFor(BillState state, bool edit) {
            if (state.counter is not RecipeWorkerCounter_DefCustom) {
                var counter = new RecipeWorkerCounter_DefCustom();
                var parent = counter.ParentFilter(state.bill);
                if (parent == null) return;
                counter.InitFrom(parent);
                state.counter = counter;
            }
            if (edit) state.counter.Edit(state.bill);
        }

        public static RecipeWorkerCounter_DefCustom For(int id) 
            => (id == ID) ? new() : null;

        public RecipeWorkerCounter_DefCustom() {
            filter = new ThingFilter();
            id = ID;
        }

        public RecipeWorkerCounter_DefCustom(Bill_Production bill) : this() {
            InitFrom(ParentFilter(bill));
        }

        private void InitFrom(ThingFilter parentFilter) {
            filter.CopyAllowancesFrom(parentFilter);
        }

        public override string Label => Strings.ButtonCustom;

        public override bool HasEdit => true;

        public override void Edit(Bill_Production bill) {
            var parent = ParentFilter(bill);
            if (parent != null) {
                Find.WindowStack.Add(new Dialog_Filter(filter, parent));
            }
        }

        public override void CopyTo(ref RecipeWorkerCounter_Abstract counter, Bill_Production bill) {
            if (counter is not RecipeWorkerCounter_DefCustom other) {
                counter = other = new(bill);
            }
            other.filter.CopyAllowancesFrom(filter);
        }

        protected override void ExposeData() {
            Scribe_Deep.Look(ref filter, "filter");
        }

        protected override void Update(Bill_Production bill) {
            ParentFilter(bill);
            toCount.Clear();
            toCount.AddRange(filter.AllowedThingDefs);
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
                Log.Error($"Bill {bill.LabelCap} has counter {bill.recipe.workerCounterClass.FullName}, which is not supported.");
                return null;
            }
        }
    }
}
