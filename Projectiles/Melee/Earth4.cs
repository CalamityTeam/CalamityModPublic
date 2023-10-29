using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class Earth4 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int noTileHitCounter = 120;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int randomToSubtract = Main.rand.Next(1, 4);
            noTileHitCounter -= randomToSubtract;
            if (noTileHitCounter == 0)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }
            Projectile.alpha -= 15;
            int alphaControl = 150;
            if (Projectile.Center.Y >= Projectile.ai[1])
            {
                alphaControl = 0;
            }
            if (Projectile.alpha < alphaControl)
            {
                Projectile.alpha = alphaControl;
            }
            Projectile.localAI[0] += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;
            if (Main.rand.NextBool(16))
            {
                Vector2 dustDirection = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int brightGreenDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                Main.dust[brightGreenDust].velocity = dustDirection * 0.66f;
                Main.dust[brightGreenDust].position = Projectile.Center + dustDirection * 12f;
            }
            if (Main.rand.NextBool(48) && Main.netMode != NetmodeID.Server)
            {
                int gored = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), 16, 1f);
                Main.gore[gored].velocity *= 0.66f;
                Main.gore[gored].velocity += Projectile.velocity * 0.3f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.light = 0.9f;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
                if (Main.rand.NextBool(20) && Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }
            Lighting.AddLight(Projectile.Center, 0f, 1.5f, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 15; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 74, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item4, Projectile.position);

            float healAmt = hit.Damage * 0.075f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<EarthHealOrb>(), 1200f, 3f);
        }
    }
}
