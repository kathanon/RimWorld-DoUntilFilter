using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DoUntilFilter;
public class Dialog_Filter : Window {
    private static readonly List<ThingDef> small = 
        DefDatabase<ThingDef>.AllDefs.Where(x => x.smallVolume).ToList();
    private static readonly IEnumerable<SpecialThingFilterDef> specialFilters =
        DefDatabase<SpecialThingFilterDef>.AllDefs;

    private readonly ThingFilter filter;
    private readonly ThingFilter parent;
    private readonly QuickSearchWidget search = new();
    private Listing_TreeThingFilter list;
    private Vector2 scroll;
    private float viewHeight;

    public Dialog_Filter(ThingFilter filter, ThingFilter parent) {
        this.filter = filter;
        this.parent = parent;

        list = new(filter, parent, null, specialFilters, small, search.filter);

        doCloseX = true;
        closeOnClickedOutside = true;
        layer = WindowLayer.SubSuper;
    }

    public override Vector2 InitialSize => new(320f, 500f);

    protected override void SetInitialSizeAndPosition() {
        base.SetInitialSizeAndPosition();
        windowRect.x -= 250f;
    }

    public override void DoWindowContents(Rect rect) {
        // Modified from ThingFilterUI

        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.UpperCenter;
        Widgets.Label(rect, Strings.CountedItems);
        rect.yMin += Text.CalcSize(Strings.CountedItems).y + 4f;
        GenUI.ResetLabelAlign();
        Widgets.DrawMenuSection(rect);

        Text.Font = GameFont.Tiny;
        rect = rect.ContractedBy(2f);
        Rect button = new Rect(rect.x, rect.y, rect.width / 2f, 24f);
        if (Widgets.ButtonText(button, "ClearAll".Translate())) {
            filter.SetDisallowAll(exceptedFilters: specialFilters);
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        }
        button.x += button.width + 1f;
        if (Widgets.ButtonText(button, "AllowAll".Translate())) {
            filter.SetAllowAll(parent);
            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
        }

        Text.Font = GameFont.Small;
        rect.yMin = button.yMax + 2f;
        Rect rect3 = new(rect.x, rect.y, rect.width, 24f);
        search.OnGUI(rect3);
        rect.yMin = rect3.yMax;

        Vector2 topMargin = new(0f, 2f);
        Rect view = new(0f, 0f, rect.width - 16f, viewHeight);
        Rect visible = new(scroll + topMargin, new Vector2(view.width, rect.height) - topMargin);
        Widgets.BeginScrollView(rect, ref scroll, view);

        list.Begin(new(0f, 2f, view.width, 9999f));
        list.ListCategoryChildren(parent.DisplayRootCategory, 512, null, visible);
        list.End();
        search.noResultsMatched = list.matchCount == 0;
        if (Event.current.type == EventType.Layout) {
            viewHeight = list.CurHeight + 4f;
        }

        Widgets.EndScrollView();
    }
}
