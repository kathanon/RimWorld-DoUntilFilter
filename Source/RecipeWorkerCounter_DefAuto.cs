using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public class RecipeWorkerCounter_DefAuto : RecipeWorkerCounter_DefAbstract {
        public static FloatMenuOption MenuOption(BillState state) 
            => new(Strings.Automatic, () => SetFor(state));

        public static void SetFor(BillState state) 
            => state.counter = For(state.bill.recipe);

        public static RecipeWorkerCounter_DefAuto For(RecipeDef recipe) {
            var cls = recipe?.workerCounterClass;
            if (cls == typeof(RecipeWorkerCounter_ButcherAnimals )) return Meat;
            if (cls == typeof(RecipeWorkerCounter_MakeStoneBlocks)) return ButcherProd;
            return null;
        }

        public static RecipeWorkerCounter_DefAuto For(int id) {
            if (id == Meat.id       ) return Meat;
            if (id == ButcherProd.id) return ButcherProd;
            return null;
        }

        public static RecipeWorkerCounter_DefAuto Meat = new() {
            productsFor = MeatFor,
            id = 1,
        };

        public static RecipeWorkerCounter_DefAuto ButcherProd = new() {
            productsFor = ButcherProducts,
            id = 2,
        };

        public override string Label => Strings.Automatic;

        public override void CopyTo(ref RecipeWorkerCounter_Abstract counter, Bill_Production bill) 
            => counter = this;
    }
}
