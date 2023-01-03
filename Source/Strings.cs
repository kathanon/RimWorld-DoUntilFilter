using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DoUntilFilter {
    public static class Strings {
        public const string ID     = "kathanon.DoUntilFilter";
        public const string Name   = "\"Do Until\" Filter";

        public const string BWM_ID = "falconne.bwm";

        // UI
        public static readonly string CountingFilter = (ID + ".CountingFilter").Translate();
        public static readonly string VanillaFilter  = (ID + ".VanillaFilter" ).Translate();
        public static readonly string Edit           = (ID + ".Edit"          ).Translate();
        public static readonly string CountedItems   = (ID + ".CountedItems"  ).Translate();
        public static readonly string Nothing        = (ID + ".Nothing"       ).Translate();
        public static readonly string Mixed          = (ID + ".Mixed"         ).Translate();
        public static readonly string Automatic      = (ID + ".Automatic"     ).Translate();
        public static readonly string ButtonCustom   = (ID + ".ButtonCustom"  ).Translate();
        public static readonly string MenuCustom     = (ID + ".MenuCustom"    ).Translate();

        // Parameterized
        public static string AndMore(int n) => (ID + ".AndMore").Translate(n);
    }
}
