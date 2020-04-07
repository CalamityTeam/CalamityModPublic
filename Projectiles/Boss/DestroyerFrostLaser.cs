using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.Projectiles.Boss
{
    public class DestroyerFrostLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Laser");
        }

        public override void SetDefaults()
        {
			projectile.ignoreWater = true;
			projectile.width = 4;
			projectile.height = 4;
			projectile.aiStyle = 1;
			aiType = ProjectileID.FrostBeam;
			projectile.hostile = true;
			projectile.penetrate = -1;
			projectile.light = 0.75f;
			projectile.alpha = 255;
			projectile.scale = 1.2f;
			projectile.timeLeft = 600;
			projectile.magic = true;
			projectile.coldDamage = true;
			projectile.extraUpdates = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.alpha < 200)
            {
                return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
            }
            return Color.Transparent;
        }

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (Main.rand.NextBool(5) && !target.Calamity().gState && !target.frozen && target.chilled)
				target.AddBuff(ModContent.BuffType<GlacialState>(), 20);
			target.AddBuff(BuffID.Frostburn, 240);
			target.AddBuff(BuffID.Chilled, 120);
		}
    }
}
