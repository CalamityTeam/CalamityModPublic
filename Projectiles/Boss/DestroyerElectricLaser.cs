using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class DestroyerElectricLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Laser");
        }

        public override void SetDefaults()
        {
            projectile.ignoreWater = true;
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.scale = 1.2f;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 2;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
            Lighting.AddLight(projectile.Center, 0f, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0.35f / 255f);
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 125;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.localAI[1] == 0f)
            {
                Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
                projectile.localAI[1] = 1f;
            }
            if (projectile.velocity.Length() < 12f)
            {
                projectile.velocity *= 1.0025f;
            }
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
            target.AddBuff(BuffID.Electrified, 60);
        }
    }
}
