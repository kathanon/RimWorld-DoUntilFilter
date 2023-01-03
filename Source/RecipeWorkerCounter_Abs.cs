using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DoUntilFilter {
    using ProductFunc = Func<ThingDef, IEnumerable<ThingDef>>;

    public abstract class RecipeWorkerCounter_Abs : RecipeWorkerCounter {
        protected ProductFunc productsFor;
        protected int id = -1;

        protected virtual HashSet<ThingDef> ToCountFor(Bill_Production bill)
            => Products(bill.ingredientFilter).ToHashSet();

        protected IEnumerable<ThingDef> Products(ThingFilter filter)
            => filter.AllowedThingDefs.SelectMany(productsFor);


        protected abstract void ExposeData();

        public abstract void CopyTo(ref RecipeWorkerCounter_Abs counter, Bill_Production bill);

        public abstract string Label { get; }

        public virtual bool HasEdit => false;

        public virtual void Edit(Bill_Production bill) {}


        protected static ProductFunc ProductsFuncFor(RecipeDef recipe) {
            var cls = recipe?.workerCounterClass;
            if (cls == typeof(RecipeWorkerCounter_ButcherAnimals)) return MeatFor;
            if (cls == typeof(RecipeWorkerCounter_MakeStoneBlocks)) return ButcherProducts;
            return null;
        }

        protected static IEnumerable<ThingDef> MeatFor(ThingDef def) 
            => Enumerate(def.ingestible?.sourceDef?.race?.meatDef);

        protected static IEnumerable<ThingDef> ButcherProducts(ThingDef def) 
            => def.butcherProducts?.Select(y => y.thingDef) ?? Enumerable.Empty<ThingDef>();


        public void Tooltip(Rect tooltip, Bill_Production bill) {
            var list = ToCountFor(bill).Select(x => x.LabelCap).ToList();
            if (list.Count > 1) {
                TooltipHandler.TipRegion(tooltip, GenTip, 65419419);
            }

            string GenTip() {
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
        }

        public static void Look(ref RecipeWorkerCounter_Abs counter, string name) {
            if (Scribe.EnterNode(name)) {
                try {
                    int id = counter?.id ?? 0;
                    Scribe_Values.Look(ref id, "id");
                    if (id > 0 && counter == null) {
                        RecipeWorkerCounter_Abs res = null;
                        res ??= RecipeWorkerCounter_Auto.For(id);
                        res ??= RecipeWorkerCounter_Custom.For(id);
                        counter = res;
                    }
                    counter?.ExposeData();
                } finally {
                    Scribe.ExitNode();
                }
            }
        }

        public override bool CanCountProducts(Bill_Production bill) {
            return true;
        }

        public override int CountProducts(Bill_Production bill) {
            int num = 0;
            foreach (var thing in ToCountFor(bill)) {
                num += bill.Map.resourceCounter.GetCount(thing);
            }

            return num;
        }

        public override string ProductsDescription(Bill_Production bill) {
            var things = ToCountFor(bill);
            if (things.Count == 0) {
                return Strings.Nothing;
            }
            if (things.Count == 1) {
                return things.First().LabelCap;
            }

            var cats = things.Select(x => x.FirstThingCategory).Distinct().ToList();
            if (cats.Count > 1) {
                cats = cats.Select(x => x.Parents).Aggregate((a, b) => a.Intersect(b)).ToList();
            }
            var cat = cats.FirstOrDefault();
            if ((cat?.ThisAndChildCategoryDefs
                    .SelectMany(x => x.childThingDefs)
                    .All(things.Contains) ?? false)
                && cat != ThingCategoryDefOf.Root) {
                return cat.LabelCap;
            }

            return Strings.Mixed;
        }

        public override bool CanPossiblyStoreInStockpile(Bill_Production bill, Zone_Stockpile stockpile) {
            var store = stockpile.GetStoreSettings();
            return ToCountFor(bill).All(store.AllowedToAccept);
        }

        private static IEnumerable<T> Enumerate<T>(T item) {
            if (item != null) yield return item;
        }
    }
}
