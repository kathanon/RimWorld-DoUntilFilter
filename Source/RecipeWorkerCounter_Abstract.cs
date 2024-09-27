using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DoUntilFilter;
public abstract class RecipeWorkerCounter_Abstract : RecipeWorkerCounter {
    protected int id = -1;
    private Bill_Production lastBill;
    private int lastFrame = -1;

    public abstract string Label { get; }

    public virtual bool HasEdit => false;

    protected abstract bool Active { get; }

    public abstract void CopyTo(ref RecipeWorkerCounter_Abstract counter, Bill_Production bill);

    public virtual void Edit(Bill_Production bill) {}

    protected virtual void ExposeData() {}

    protected abstract string Tooltip(Bill_Production bill);

    protected virtual void Update(Bill_Production bill) {}

    protected void UpdateIfNeeded(Bill_Production bill) { 
        int frame = Time.frameCount;
        if (frame == lastFrame && ReferenceEquals(lastBill, bill)) return;
        lastFrame = frame;
        lastBill = bill;
        Update(bill);
    }

    public void DoTooltip(Rect rect, Bill_Production bill) {
        UpdateIfNeeded(bill);
        if (Active) {
            TooltipHandler.TipRegion(rect, () => Tooltip(bill), 65419419);
        }
    }

    public static void SetDefaultFor(BillState state) {
        var recipe = state.bill.recipe;
        if (recipe.WorkerCounter is RecipeWorkerCounter_Defer defer) {
            state.counter = defer.Initial;
        }
    }

    public static void Look(ref RecipeWorkerCounter_Abstract counter, string name) {
        if (Scribe.EnterNode(name)) {
            try {
                int id = counter?.id ?? 0;
                Scribe_Values.Look(ref id, "id");
                if (Scribe.mode == LoadSaveMode.LoadingVars) {
                    RecipeWorkerCounter_Abstract res = null;
                    if (id > 0) {
                        res ??= RecipeWorkerCounter_DefAuto.For(id);
                        res ??= RecipeWorkerCounter_DefCustom.For(id);
                        res ??= RecipeWorkerCounter_TagsAuto.For(id);
                        res ??= RecipeWorkerCounter_TagsCustom.For(id);
                    }
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
}
