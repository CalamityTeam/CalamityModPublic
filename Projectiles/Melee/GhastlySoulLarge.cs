using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GhastlySoulLarge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        private const int TimeLeft = 660;
        private float HomingBuff = 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = TimeLeft;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (HomingBuff > 0)
                HomingBuff -= 0.01f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);

            if (Projectile.timeLeft % 2 == 0 && Projectile.timeLeft < TimeLeft - 10)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity * Main.rand.NextFloat(0.5f, 1.5f), false, Main.rand.Next(9, 12 + 1), 0.4f, Color.Plum);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            float inertia = MathHelper.Lerp(20, 90, HomingBuff) * Projectile.ai[1];
            float velocity = VoidEdge.ShootSpeed * Projectile.ai[1];
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                float homingDistance = 900f;
                NPC target = Projectile.FindTargetWithinRange(homingDistance);
                if (Projectile.timeLeft < TimeLeft - VoidEdge.ProjectileSpreadOutTime && target != null)
                {
                    CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, homingDistance, velocity, inertia);
                }
                else if (Projectile.Distance(Main.player[Projectile.owner].Center) > homingDistance)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * velocity) / inertia;
                }
            }
            else
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > TimeLeft - 5)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.88f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.damage = (int)(Projectile.damage / VoidEdge.TotalProjectilesPerSwing);
            Projectile.penetrate = -1;
            Projectile.ExpandHitboxBy(230);
            Projectile.Damage();
            SoundEngine.PlaySound(VoidEdge.ProjectileDeathSound with { Pitch = -0.3f }, Projectile.Center);

            int points = 25;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
            float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
            for (int k = 0; k < points; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);
                LineParticle subTrail = new LineParticle(Projectile.Center + velocity * 20.5f, velocity * 15, false, 30, 0.75f, Color.Plum);
                GeneralParticleHandler.SpawnParticle(subTrail);
            }
            for (int k = 0; k < 17; k++)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 66, new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.8f));
                dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                dust2.noGravity = true;
                dust2.color = Color.Plum;
            }
        }
    }
}
