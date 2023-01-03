using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public static class State {
        private static readonly ConditionalWeakTable<Bill_Production, BillState> table 
            = new ConditionalWeakTable<Bill_Production, BillState>();

 
        public static bool DoFunc<T>(Bill_Production bill,
                                     Func<RecipeWorkerCounter, T> func,
                                     ref T result) {
            var worker = For(bill)?.counter;
            if (worker != null) {
                result = func(worker);
            }
            return worker == null;
        }

        public static BillState For(Bill_Production bill) 
            => bill != null && bill.recipe?.workerCounterClass != typeof(RecipeWorkerCounter)
                ? table.GetValue(bill, BillState.Create) 
                : null;
    }
}
