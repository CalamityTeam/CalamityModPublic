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
    public class PotionSicknessDisplay : CooldownIndicator
    {
        public override bool DisplayMe => CalamityConfig.Instance.VanillaCooldownDisplay && AfflictedPlayer.potionDelay > 0;
        public override string Name => "Healing Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PotionSickness";
        public override Color OutlineColor => new Color(255, 142, 165);
        public override Color CooldownColorStart => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), Completion);


        public PotionSicknessDisplay(int duration, Player player) : base(duration, player)
        {
        }
    }
}