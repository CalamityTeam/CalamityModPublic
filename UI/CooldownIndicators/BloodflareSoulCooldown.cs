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
using Terraria.ModLoader;

namespace CalamityMod.UI.CooldownIndicators
{
    public class BloodflareSoulCooldown : CooldownIndicator
    {
        public override string SyncID => "BloodflareSoul";
        public override bool DisplayMe => true;
        public override string Name => "Bloodflare Soul Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BloodflareSoul";
        public override Color OutlineColor => new Color(255, 205, 219);
        public override Color CooldownColorStart => new Color(216, 60, 90);
        public override Color CooldownColorEnd => new Color(251, 106, 150);

        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/BloodflareRangerRecharge");

        public BloodflareSoulCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}