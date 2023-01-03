using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public class RecipeWorkerCounter_Auto : RecipeWorkerCounter_Abs {
        public static FloatMenuOption MenuOption(BillState state) 
            => new FloatMenuOption(Strings.Automatic, () => SetFor(state));

        public static void SetFor(BillState state) 
            => state.counter = For(state.bill.recipe);

        public static RecipeWorkerCounter_Auto For(RecipeDef recipe) {
            var cls = recipe?.workerCounterClass;
            if (cls == typeof(RecipeWorkerCounter_ButcherAnimals )) return Meat;
            if (cls == typeof(RecipeWorkerCounter_MakeStoneBlocks)) return ButcherProd;
            return null;
        }

        public static RecipeWorkerCounter_Auto For(int id) {
            if (id == Meat.id       ) return Meat;
            if (id == ButcherProd.id) return ButcherProd;
            return null;
        }

        public static RecipeWorkerCounter_Auto Meat = new RecipeWorkerCounter_Auto {
            productsFor = MeatFor,
            id = 1,
        };

        public static RecipeWorkerCounter_Auto ButcherProd = new RecipeWorkerCounter_Auto {
            productsFor = ButcherProducts,
            id = 2,
        };

        public override string Label => Strings.Automatic;

        public override void CopyTo(ref RecipeWorkerCounter_Abs counter, Bill_Production bill) 
            => counter = this;

        protected override void ExposeData() {}
    }
}
