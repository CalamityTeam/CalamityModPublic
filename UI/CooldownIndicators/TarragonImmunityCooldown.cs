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
    public class TarragonImmunityCooldown : CooldownIndicator
    {
        public override string SyncID => "TarraImmune";
        public override bool DisplayMe => true;
        public override string Name => "Tarragon Immunity Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/TarragonImmunity";
        public override Color OutlineColor => new Color(215, 182, 82);
        public override Color CooldownColorStart => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(171, 106, 49), new Color(215, 182, 82), 1 - Completion);

        public TarragonImmunityCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}