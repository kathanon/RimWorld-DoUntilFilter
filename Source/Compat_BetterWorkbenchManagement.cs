using HarmonyLib;
using ImprovedWorkbenches;
using RimWorld;
using System.Linq;
using System.Reflection;
using Verse;

namespace DoUntilFilter {
    [HarmonyPatch]
    public static class Compat_BetterWorkbenchManagement {
        public static bool Prepare() {
            return BetterWorkbenchManagement.Active;
        }

        public static MethodBase TargetMethod() {
            return typeof(ExtendedBillDataStorage).GetMethod("MirrorBills");
        }

        public static void Postfix(Bill_Production sourceBill, Bill_Production destinationBill) {
            Patches.Clone(sourceBill, destinationBill);
        }
    }

    public static class BetterWorkbenchManagement {
        public static readonly bool Active =
            ModLister.AllInstalledMods.Any(m => m.PackageIdNonUnique == Strings.BWM_ID && m.Active);
    }
}
