using System;
using UnityEngine;
using Verse;

namespace DoUntilFilter;
public class GenericDialog : Window {
    private readonly Vector2 size;
    private readonly Action<Rect> doUI;
    private readonly Vector2 deltaPos;

    private GenericDialog(Vector2 size, Action<Rect> doUI, Vector2 deltaPos) {
        this.size = size;
        this.doUI = doUI;
        this.deltaPos = deltaPos;
    }
    public override Vector2 InitialSize => size;

    protected override void SetInitialSizeAndPosition() {
        base.SetInitialSizeAndPosition();
        windowRect.position += deltaPos;
    }

    public static void Open(Vector2 size, Action<Rect> doUI, Vector2 deltaPos = default) 
        => Find.WindowStack.Add(new GenericDialog(size, doUI, deltaPos) {
            doCloseX = true,
            closeOnClickedOutside = true,
        });

    public override void DoWindowContents(Rect inRect) 
        => doUI(inRect);
}
