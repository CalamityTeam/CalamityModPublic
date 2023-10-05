using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class AMRShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
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
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
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
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target.Center, hit.Crit);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);

            target.Calamity().miscDefenseLoss = 25;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects(target.Center, true);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
        }

        private void OnHitEffects(Vector2 targetPos, bool crit)
        {
            int extraProjectileAmt;
            bool fromRight;
            if (crit)
            {
                var source = Projectile.GetSource_FromThis();
                extraProjectileAmt = 5;
                for (int x = 0; x < extraProjectileAmt; x++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        fromRight = x > 3;
                        CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, fromRight, 500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<AMR2>(), (int)(Projectile.damage * 0.15), Projectile.knockBack * 0.1f, Projectile.owner);
                    }
                }
            }
            else
            {
                var source = Projectile.GetSource_FromThis();
                extraProjectileAmt = 2;
                for (int x = 0; x < extraProjectileAmt; x++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        fromRight = x > 0;
                        CalamityUtils.ProjectileBarrage(source, Projectile.Center, targetPos, fromRight, 500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<AMR2>(), (int)(Projectile.damage * 0.15), Projectile.knockBack * 0.1f, Projectile.owner);
                    }
                }
            }
        }
    }
}
