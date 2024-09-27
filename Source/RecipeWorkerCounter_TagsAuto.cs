using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DoUntilFilter;

public class RecipeWorkerCounter_TagsAuto : RecipeWorkerCounter_TagsAbstract {
    private const int ID = 11;

    private readonly HashSet<string> allowed = [];

    public RecipeWorkerCounter_TagsAuto() {
        id = ID;
    }

    public static FloatMenuOption MenuOption(BillState state)
        => new(Strings.Automatic, () => SetFor(state));

    public static void SetFor(BillState state)
        => state.counter = For(state.bill.recipe);

    public static RecipeWorkerCounter_TagsAuto For(int id) 
        => (id == ID) ? new() : null;

    public static RecipeWorkerCounter_TagsAuto For(RecipeDef _) 
        => new();

    public override string Label => Strings.Automatic;

    public override void CopyTo(ref RecipeWorkerCounter_Abstract counter, Bill_Production bill) 
        => counter = this;

    protected override void Update(Bill_Production bill) {
        allowed.Clear();

        var separateRequire = bill.recipe.ingredients
            .Select(x => (x.filter, set: (HashSet<string>) null))
            .ToArray();
        foreach (var thing in bill.ingredientFilter.AllowedThingDefs) {
            var tags = Tags(thing);
            allowed.AddRange(tags);
            for (int i = 0; i < separateRequire.Length; i++) {
                if (separateRequire[i].filter.Allows(thing)) {
                    if (separateRequire[i].set == null) {
                        separateRequire[i].set = [.. tags];
                    } else {
                        separateRequire[i].set.RemoveWhere(x => !tags.Contains(x));
                    }
                }
            }
        }

        require.Clear();
        require.AddRange(separateRequire.SelectMany(x => x.set ?? []));

        exclude.Clear();
        exclude.AddRange(
            bill.recipe.fixedIngredientFilter.AllowedThingDefs
                .SelectMany(Tags));
        exclude.RemoveWhere(allowed.Contains);

        allowed.Clear();
    }
}