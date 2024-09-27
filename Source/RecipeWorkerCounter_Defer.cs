using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public class RecipeWorkerCounter_Defer : RecipeWorkerCounter {
        public static RecipeWorkerCounter Butcher(RecipeDef recipe) 
            => new RecipeWorkerCounter_Defer(recipe,
                                             new RecipeWorkerCounter_ButcherAnimals(),
                                             RecipeWorkerCounter_DefAuto.For(recipe));

        public static RecipeWorkerCounter Stone(RecipeDef recipe) 
            => new RecipeWorkerCounter_Defer(recipe,
                                             new RecipeWorkerCounter_MakeStoneBlocks(),
                                             RecipeWorkerCounter_DefAuto.For(recipe));

        public static RecipeWorkerCounter Other(RecipeDef recipe) 
            => new RecipeWorkerCounter_Defer(recipe,
                                             new RecipeWorkerCounter(),
                                             RecipeWorkerCounter_TagsAuto.For(recipe));

        private readonly RecipeWorkerCounter vanilla;
        private readonly RecipeWorkerCounter_Abstract initial;

        public RecipeWorkerCounter_Abstract Initial => initial;

        private RecipeWorkerCounter_Defer(RecipeDef recipe,
                                          RecipeWorkerCounter vanilla,
                                          RecipeWorkerCounter_Abstract initial) {
            this.vanilla = vanilla;
            this.initial = initial;
            vanilla.recipe = recipe;
        }

        private RecipeWorkerCounter Vanilla(Bill_Production bill) {
            vanilla.recipe = bill.recipe;
            return vanilla;
        }

        private RecipeWorkerCounter For(Bill_Production bill)
            => State.For(bill)?.counter ?? Vanilla(bill);

        public override bool CanCountProducts(Bill_Production bill) => true;

#if VERSION_1_5
        public override bool CanPossiblyStore(Bill_Production bill, ISlotGroup stockpile) 
            => For(bill).CanPossiblyStore(bill, stockpile);
#else
        public override bool CanPossiblyStoreInStockpile(Bill_Production bill, Zone_Stockpile stockpile) 
            => For(bill).CanPossiblyStoreInStockpile(bill, stockpile);
#endif

        public override int CountProducts(Bill_Production bill) 
            => For(bill).CountProducts(bill);

        public override string ProductsDescription(Bill_Production bill) 
            => For(bill).ProductsDescription(bill);
    }
}
