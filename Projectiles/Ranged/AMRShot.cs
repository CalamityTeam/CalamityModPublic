using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 10;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            float num107 = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
            if (Projectile.alpha > 0)
                Projectile.alpha -= (byte)(num107 * 0.9);
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha < 140)
                return new Color(255, 255, 255, 100);

            return Color.Transparent;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig1, Projectile.position);
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
                var source = Projectile.GetSource_FromThis();
                int extraProjectileAmt = 8;
                for (int x = 0; x < extraProjectileAmt; x++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        bool fromRight = x > 3;
                        CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, fromRight, 500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<AMR2>(), (int)(Projectile.damage * 0.1), Projectile.knockBack * 0.1f, Projectile.owner);
                    }
                }
            }
        }
    }
}
