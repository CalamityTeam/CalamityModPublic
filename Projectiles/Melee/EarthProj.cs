using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class EarthProj : ModProjectile, ILocalizedModType
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
            Projectile.timeLeft = 600;
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
            int dustChoice = Main.rand.Next(3);
            if (dustChoice == 0)
            {
                dustChoice = 74;
            }
            else if (dustChoice == 1)
            {
                dustChoice = 229;
            }
            else
            {
                dustChoice = 244;
            }
            if (Main.rand.NextBool(16))
            {
                Vector2 dustDirection = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int dustOnHit = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustChoice, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                Main.dust[dustOnHit].velocity = dustDirection * 0.66f;
                Main.dust[dustOnHit].position = Projectile.Center + dustDirection * 12f;
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
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustChoice, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
                if (Main.rand.NextBool(20) && Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, new Vector2(Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
                }
            }
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            for (i = 0; i < 4; i++)
            {
                int projChoice = Main.rand.Next(3);
                if (projChoice == 0)
                {
                    projChoice = ModContent.ProjectileType<Earth2>();
                }
                else if (projChoice == 1)
                {
                    projChoice = ModContent.ProjectileType<Earth3>();
                }
                else
                {
                    projChoice = ModContent.ProjectileType<Earth4>();
                }
                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), projChoice, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), projChoice, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 15; k++)
            {
                int dustChoice = Main.rand.Next(3);
                if (dustChoice == 0)
                {
                    dustChoice = 74;
                }
                else if (dustChoice == 1)
                {
                    dustChoice = 229;
                }
                else
                {
                    dustChoice = 244;
                }
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustChoice, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(BuffID.CursedInferno, 180);
        }
    }
}
