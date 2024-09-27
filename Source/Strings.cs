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

        public const string TagMeat   = "$Meat";
        public const string TagAnimal = "$AnimalProducts";
        public const string TagVeg    = "$Vegetables";

        // UI
        public static readonly string CountingFilter   = (ID + ".CountingFilter"  ).Translate();
        public static readonly string VanillaFilter    = (ID + ".VanillaFilter"   ).Translate();
        public static readonly string Edit             = (ID + ".Edit"            ).Translate();
        public static readonly string CountedItems     = (ID + ".CountedItems"    ).Translate();
        public static readonly string Nothing          = (ID + ".Nothing"         ).Translate();
        public static readonly string Mixed            = (ID + ".Mixed"           ).Translate();
        public static readonly string Automatic        = (ID + ".Automatic"       ).Translate();
        public static readonly string ButtonCustom     = (ID + ".ButtonCustom"    ).Translate();
        public static readonly string MenuCustom       = (ID + ".MenuCustom"      ).Translate();
        public static readonly string RequireHeader    = (ID + ".RequireHeader"   ).Translate();
        public static readonly string ExcludeHeader    = (ID + ".ExcludeHeader"   ).Translate();
        public static readonly string IngredientHeader = (ID + ".IngredientHeader").Translate();
        public static readonly string RequireTooltip   = (ID + ".RequireTooltip"  ).Translate();
        public static readonly string ExcludeTooltip   = (ID + ".ExcludeTooltip"  ).Translate();
        public static readonly string ThatContains     = (ID + ".ThatContains"    ).Translate();
        public static readonly string NotContains      = (ID + ".NotContains"     ).Translate();
        public static readonly string AndNotContains   = (ID + ".AndNotContains"  ).Translate();
        public static readonly string Meat             = (ID + ".Meat"            ).Translate();
        public static readonly string AnimalProducts   = (ID + ".AnimalProducts"  ).Translate();
        public static readonly string Vegetables       = (ID + ".Vegetables"      ).Translate();

        // Parameterized
        public static string AndMore(int n) => (ID + ".AndMore").Translate(n);

        public static string Filtered(string item) => (ID + ".Filtered").Translate(item);
    }
}
