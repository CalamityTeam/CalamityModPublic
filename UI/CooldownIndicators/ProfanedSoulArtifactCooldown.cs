using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using CalamityMod.CalPlayer;

namespace CalamityMod.UI.CooldownIndicators
{
    public class ProfanedSoulArtifactCooldown : CooldownIndicator
    {
        public override string SyncID => "PSABurnout";
        public override bool DisplayMe => true;
        public override string Name => "Soul Artifact Burn Out";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/ProfanedSoulArtifact";
        public override Color OutlineColor => new Color(255, 191, 73);
        public override Color CooldownColorStart => new Color(181, 136, 177);
        public override Color CooldownColorEnd => new Color(255, 194, 161);

        public ProfanedSoulArtifactCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}