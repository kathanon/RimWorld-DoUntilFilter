using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DoUntilFilter;
public abstract class RecipeWorkerCounter_TagsAbstract : RecipeWorkerCounter_Abstract {
    protected HashSet<string> require = [];
    protected HashSet<string> exclude = [];

    protected static readonly Dictionary<string, ThingDef[]> thingsWithTag =
        DefDatabase<ThingDef>.AllDefs
            .SelectMany(x => x.ingredient?.mergeCompatibilityTags.Select(y => (tag: y, thing: x)) ?? [])
            .GroupBy(x => x.tag)
            .ToDictionary(x => x.Key,
                          x => x.Select(x => x.thing)
                                .ToArray());

    public override int CountProducts(Bill_Production bill) {
        UpdateIfNeeded(bill);
        return bill.Map.listerThings
            .ThingsOfDef(bill.recipe.ProducedThingDef)
            .Where(StoreUtility.IsInAnyStorage)
            .Select(x => x.TryGetComp<CompIngredients>())
            .Where(Filter)
            .Sum(x => x.parent.stackCount);

        bool Filter(CompIngredients comp) {
            if (comp == null) return false;
            int nRequire = 0;
            var tags = comp.MergeCompatibilityTags
                .Concat(comp.ingredients.SelectMany(SpecialTags))
                .Distinct();
            foreach (string tag in tags) {
                if (exclude.Contains(tag)) return false;
                if (require.Contains(tag)) nRequire++;
            }
            return nRequire == require.Count;
        }
    }

    protected IEnumerable<string> SpecialTags(ThingDef ingredient) {
        switch (FoodUtility.GetFoodKind(ingredient)) {
            case FoodKind.Meat:    yield return Strings.TagMeat;   break;
            case FoodKind.Any:     yield return Strings.TagAnimal; break;
            case FoodKind.NonMeat: yield return Strings.TagVeg;    break;
            default: break;
        }
    }

    protected IEnumerable<string> Tags(ThingDef ingredient) 
        => SpecialTags(ingredient)
            .Concat(ingredient.ingredient?.mergeCompatibilityTags ?? []);

    protected override bool Active
        => require.Count + exclude.Count > 0;

    protected override string Tooltip(Bill_Production bill) {
        UpdateIfNeeded(bill);
        StringBuilder buf = new(bill.recipe.ProducedThingDef.LabelCap);
        bool useAnd = AddSet(buf, Strings.ThatContains, require);
        AddSet(buf, useAnd ? Strings.AndNotContains : Strings.NotContains, exclude);
        return buf.ToString();

        bool AddSet(StringBuilder buf, string title, HashSet<string> tags) {
            if (tags.Count == 0) return false;
            buf.Append(title);
            foreach (var tag in tags.OrderBy(x => x)) {
                buf.AppendLine();
                buf.Append("  ");
                buf.Append(IngredientsWith(tag, bill));
            }
            return true;
        }
    }

    protected static string IngredientsWith(string tag, Bill_Production bill = null) {
        if (tag == Strings.TagMeat  ) return Strings.Meat;
        if (tag == Strings.TagAnimal) return Strings.AnimalProducts;
        if (tag == Strings.TagVeg   ) return Strings.Vegetables;
        return thingsWithTag[tag]
            .Where(x => bill?.recipe.fixedIngredientFilter.Allows(x) ?? true)
            .Select(x => x.label)
            .ToCommaListOr();
    }

    public override string ProductsDescription(Bill_Production bill) {
        UpdateIfNeeded(bill);
        return Active ? Strings.Filtered(bill.recipe.ProducedThingDef.LabelCap) : null;
    }

    public override bool CanPossiblyStore(Bill_Production bill, ISlotGroup slotGroup) {
        recipe = bill.recipe;
        return base.CanPossiblyStore(bill, slotGroup);
    }
}
