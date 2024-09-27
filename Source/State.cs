using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter; 
public static class State {
    private static readonly ConditionalWeakTable<Bill_Production, BillState> table = new();

    public static BillState For(Bill_Production bill) 
        => BillState.UseFor(bill?.recipe) ? table.GetValue(bill, BillState.Create) : null;
}
