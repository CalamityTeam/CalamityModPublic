using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AMRShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AMR");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 10;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            float num107 = (float)Math.Sqrt(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y);
            if (projectile.alpha > 0)
                projectile.alpha -= (byte)(num107 * 0.9);
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.alpha < 140)
                return new Color(255, 255, 255, 100);

            return Color.Transparent;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(target.Center, crit);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);

            target.Calamity().miscDefenseLoss = 25;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center, crit);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
        }

        private void OnHitEffects(Vector2 targetPos, bool crit)
        {
            if (crit)
            {
                int extraProjectileAmt = 8;
                for (int x = 0; x < extraProjectileAmt; x++)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        bool fromRight = x > 3;
                        CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, fromRight, 500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<AMR2>(), (int)(projectile.damage * 0.1), projectile.knockBack * 0.1f, projectile.owner);
                    }
                }
            }
        }
    }
}
