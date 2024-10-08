﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DoUntilFilter {
    [StaticConstructorOnStartup]
    public static class Textures {
        private const string Prefix = Strings.ID + "/";

        public static readonly Texture2D SliderRailAtlas = ContentFinder<Texture2D>.Get("UI/Buttons/SliderRail");
        // public static readonly Texture2D Name   = ContentFinder<Texture2D>.Get(Prefix + "Name");
    }
}
