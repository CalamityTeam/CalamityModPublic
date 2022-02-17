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
    public class InkBombCooldown : CooldownIndicator
    {
        public override string SyncID => "InkBomb";
        public override bool DisplayMe => true;
        public override string Name => "Ink Bomb Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/InkBomb";
        public override Color OutlineColor => new Color(205, 182, 137);
        public override Color CooldownColorStart => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);


        public InkBombCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
}