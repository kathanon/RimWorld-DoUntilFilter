using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DoUntilFilter; 
using ProductFunc = Func<ThingDef, IEnumerable<ThingDef>>;

public abstract class RecipeWorkerCounter_DefAbstract : RecipeWorkerCounter_Abstract {
    protected ProductFunc productsFor;
    protected List<ThingDef> toCount = [];

    protected override void Update(Bill_Production bill) {
        toCount.Clear();
        toCount.AddRange(Products(bill.ingredientFilter).Distinct());
    }

    protected IEnumerable<ThingDef> Products(ThingFilter filter)
        => filter.AllowedThingDefs.SelectMany(productsFor);

    protected static ProductFunc ProductsFuncFor(RecipeDef recipe) {
        var cls = recipe?.workerCounterClass;
        if (typeof(RecipeWorkerCounter_ButcherAnimals ).IsAssignableFrom(cls)) return MeatFor;
        if (typeof(RecipeWorkerCounter_MakeStoneBlocks).IsAssignableFrom(cls)) return ButcherProducts;
        return null;
    }

    protected static IEnumerable<ThingDef> MeatFor(ThingDef def) 
        => Enumerate(def.ingestible?.sourceDef?.race?.meatDef);

    protected static IEnumerable<ThingDef> ButcherProducts(ThingDef def) 
        => def.butcherProducts?.Select(y => y.thingDef) ?? Enumerable.Empty<ThingDef>();

    protected override bool Active 
        => toCount.Count > 0;

    protected override string Tooltip(Bill_Production bill) {
        UpdateIfNeeded(bill);
        var list = toCount.Select(x => x.LabelCap).ToList();
        list.SortBy(x => (string) x);
        int n = list.Count - 10;
        StringBuilder buf = new StringBuilder();
        bool sep = false;
        foreach (var item in list.Take(10)) {
            if (sep) {
                buf.AppendLine();
            }
            sep = true;
            buf.Append(item.Resolve());
        }
        if (n > 0) {
            buf.AppendLine();
            buf.Append(Strings.AndMore(n));
        }
        return buf.ToString();
    }

    public override int CountProducts(Bill_Production bill) {
        UpdateIfNeeded(bill);
        return toCount.Sum(bill.Map.resourceCounter.GetCount);
    }

    public override string ProductsDescription(Bill_Production bill) {
        UpdateIfNeeded(bill);
        if (toCount.Count == 0) {
            return Strings.Nothing;
        }
        if (toCount.Count == 1) {
            return toCount.First().LabelCap;
        }

        var cats = toCount.Select(x => x.FirstThingCategory).Distinct().ToList();
        if (cats.Count > 1) {
            cats = cats.Select(x => x.Parents).Aggregate((a, b) => a.Intersect(b)).ToList();
        }
        var cat = cats.FirstOrDefault();
        if ((cat?.ThisAndChildCategoryDefs
                .SelectMany(x => x.childThingDefs)
                .All(toCount.Contains) ?? false)
            && cat != ThingCategoryDefOf.Root) {
            return cat.LabelCap;
        }

        return Strings.Mixed;
    }

#if VERSION_1_5
    public override bool CanPossiblyStore(Bill_Production bill, ISlotGroup stockpile) {
        UpdateIfNeeded(bill);
        var store = stockpile.Settings;
#else
    public override bool CanPossiblyStoreInStockpile(Bill_Production bill, Zone_Stockpile stockpile) {
        var store = stockpile.GetStoreSettings();
#endif
        return toCount.All(store.AllowedToAccept);
    }

    private static IEnumerable<T> Enumerate<T>(T item) {
        if (item != null) yield return item;
    }
}
