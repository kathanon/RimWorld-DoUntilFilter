using RimWorld;
using System;
using Verse.Noise;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoUntilFilter;
using RelationDict = Dictionary<string, HashSet<string>>;

public class RecipeWorkerCounter_TagsCustom : RecipeWorkerCounter_TagsAbstract {
    private const int ID = 12;

    private Vector2 scroll;
    private Vector2 dialogSize;
    private string[] relevantTags;
    private readonly RelationDict tagRelationsRequire = [];
    private readonly RelationDict tagRelationsExclude = [];

    public RecipeWorkerCounter_TagsCustom() {
        id = ID;
    }

    public static FloatMenuOption MenuOption(BillState state)
        => new(Strings.MenuCustom, () => SetFor(state, true));

    public static void SetFor(BillState state, bool edit) {
        if (state.counter is not RecipeWorkerCounter_TagsCustom counter) {
            counter = new RecipeWorkerCounter_TagsCustom();
            state.counter = counter;
        }
        counter.InitUI(state.bill);
        if (edit) state.counter.Edit(state.bill);
    }

    private void InitUI(Bill_Production bill) {
        var things = bill.recipe.fixedIngredientFilter.AllowedThingDefs;
        HashSet<string> tags = [];
        HashSet<string> relevant = [];
        tagRelationsRequire.Clear();
        tagRelationsExclude.Clear();
        foreach (var thing in things) {
            tags.AddRange(Tags(thing));
            relevant.AddRange(tags);
            foreach (var tag in tags) {
                if (tagRelationsRequire.ContainsKey(tag)) {
                    tagRelationsRequire[tag].RemoveWhere(x => !tags.Contains(x));
                } else {
                    tagRelationsRequire[tag] = [.. tags];
                }
                if (!tagRelationsExclude.ContainsKey(tag)) {
                    tagRelationsExclude[tag] = [];
                }
            }
            tags.Clear();
        }
        relevantTags = [.. relevant.OrderBy(x => x)];
        foreach (var tag in relevantTags) {
            foreach (var related in tagRelationsRequire[tag]) {
                tagRelationsExclude[related].Add(tag);
            }
        }

        Text.Font = GameFont.Small;
        float lineSpace = Text.LineHeight + 4f;
        float checkColumnWidth = Widgets.CheckboxSize + 8f;
        float ingredientsWidth = relevantTags
            .Select(x => IngredientsWith(x, bill))
            .Append(Strings.IngredientHeader)
            .Max(str => Text.CalcSize(str).x);
        float x = Adjust(2 * checkColumnWidth + ingredientsWidth + 2f);
        float y = Adjust((relevantTags.Length + 1) * lineSpace);
        dialogSize = new(x, y);

        static float Adjust(float x) 
            => Mathf.Clamp(x + Window.StandardMargin * 2, 150f, 500f);
    }

    public static RecipeWorkerCounter_TagsCustom For(int id) 
        => (id == ID) ? new() : null;

    public override string Label => Strings.ButtonCustom;

    public override bool HasEdit => true;

    public override void Edit(Bill_Production bill) {
        scroll = Vector2.zero;
        GenericDialog.Open(
            dialogSize,
            rect => DoUI(rect, bill),
            new(-(150f + dialogSize.x / 2), 0f));
    }

    public void DoUI(Rect rect, Bill_Production bill) {
        Text.Font = GameFont.Small;
        var item    = rect.TopPartPixels(Text.LineHeight);
        var yesHead = item.LeftPartPixels(Widgets.CheckboxSize + 4f);
        var noHead  = yesHead;
        noHead.x = yesHead.xMax + 4f;
        item.xMin = noHead.xMax + 4f;
        var checkAdjust = Vector2.one * (Widgets.CheckboxSize / 2);
        var yes = yesHead.center - checkAdjust;
        var no  = noHead .center - checkAdjust;

        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(yesHead, Strings.RequireHeader);
        Widgets.Label(noHead,  Strings.ExcludeHeader);
        TooltipHandler.TipRegion(yesHead, Strings.RequireTooltip);
        TooltipHandler.TipRegion(noHead,  Strings.ExcludeTooltip);
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(item,    Strings.IngredientHeader);
        Widgets.DrawAtlas(new(rect.x, item.yMax, rect.width, 2f), Textures.SliderRailAtlas);
        Step(4f);

        foreach (var tag in relevantTags) {
            Step(item.height + 4f);

            Checkbox(yes, tag, require, exclude, tagRelationsRequire);
            Checkbox(no,  tag, exclude, require, tagRelationsExclude);
            string ingredient = IngredientsWith(tag, bill);
            item.width = Text.CalcSize(ingredient).x;
            Widgets.Label(item, ingredient);
            if (item.xMax > rect.xMax) {
                TooltipHandler.TipRegion(item, ingredient);
            }
        }

        GenUI.ResetLabelAlign();

        void Step(float step) {
            yes.y  += step;
            no.y   += step;
            item.y += step;
        }
    }

    private void Checkbox(Vector2 pos, string tag, HashSet<string> set, HashSet<string> other, RelationDict relations) {
        bool on = set.Contains(tag), old = on;
        Widgets.Checkbox(pos, ref on);
        if (old == on) return;
        if (on) {
            set.Add(tag);
            other.RemoveWhere(relations[tag].Contains);
        } else {
            set.Remove(tag);
        }
    }

    protected override void ExposeData() {
        Scribe_Collections.Look(ref require, "require", LookMode.Value);
        Scribe_Collections.Look(ref exclude, "exclude", LookMode.Value);
        require ??= [];
        exclude ??= [];
    }

    public override void CopyTo(ref RecipeWorkerCounter_Abstract counter, Bill_Production bill) {
        if (counter is not RecipeWorkerCounter_TagsCustom other) {
            counter = other = new();
        }
        other.require = [.. require];
        other.exclude = [.. exclude];
    }
}