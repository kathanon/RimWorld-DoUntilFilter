﻿using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace DoUntilFilter {
    [StaticConstructorOnStartup]
    public class Main : Mod {
        public static Main Instance { get; private set; }

        static Main() {
            var harmony = new Harmony(Strings.ID);
            harmony.PatchAll();
        }

        // TODO: translation support

        public Main(ModContentPack content) : base(content) {
            Instance = this; 
        }
    }
}
