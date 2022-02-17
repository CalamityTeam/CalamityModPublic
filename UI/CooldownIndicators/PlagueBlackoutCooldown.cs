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
    public class PlagueBlackoutCooldown : CooldownIndicator
    {
        public override string SyncID => "PlagueBlackout";
        public override bool DisplayMe => Duration <= 1500;
        public override string Name => "Plague Blackout Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PlagueBlackout";
        public override Color OutlineColor => new Color(174, 237, 122);
        public override Color CooldownColorStart => Color.DarkSlateGray;
        public override Color CooldownColorEnd => Color.DarkSlateGray;
        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/PlagueReaperRecharge");

        public PlagueBlackoutCooldown(int duration, Player player) : base(duration, player)
        {
            Duration = 1500;
        }

        public override void OnCooldownEnd()
        {
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(AfflictedPlayer.position, AfflictedPlayer.width, AfflictedPlayer.height, 89, 0, 0, 100, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 6.6f;
            }
        }
    }
}