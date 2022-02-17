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
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.UI.CooldownIndicators
{
    public class DivineBlessCooldown : CooldownIndicator
    {
        public override string SyncID => "DivineBless";
        public override bool DisplayMe => true;
        public override string Name => "Divine Bless Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/DivineRecharge";
        public override Color OutlineColor =>  new Color(233, 192, 68);
        public override Color CooldownColorStart => new Color(177, 105, 33);
        public override Color CooldownColorEnd => new Color(233, 192, 68);


        public DivineBlessCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCooldownEnd()
        {
            if (AfflictedPlayer.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(AfflictedPlayer.Center, Vector2.Zero, ProjectileType<AllianceTriangle>(), 0, 0f, AfflictedPlayer.whoAmI);
        }
    }
}