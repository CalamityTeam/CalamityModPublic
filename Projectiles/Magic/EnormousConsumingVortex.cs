using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EnormousConsumingVortex : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public float IdealScale
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public const int TentacleSpawnRate = 20;
        public const int PulseInterval = 18;
        public const float PulseHitboxExpandRatio = 2.5f;
        public const float RadialOffsetVarianceFactor = 0.1f;
        public const float StartingScale = 0.0004f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subsuming Vortex");
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.scale = StartingScale;
            projectile.timeLeft = 540;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        // Vanilla Terraria does not sync projectile scale by default.
        public override void SendExtraAI(BinaryWriter writer) => writer.Write(projectile.scale);
        public override void ReceiveExtraAI(BinaryReader reader) => projectile.scale = reader.ReadSingle();

        public override void AI()
        {
            projectile.rotation += MathHelper.ToRadians(8f); // Spin 2 win.
            projectile.alpha = (int)MathHelper.Lerp(255, 0, Utils.InverseLerp(0f, 20f, Time, true)); // Fade in completely after 20 frames.
            projectile.scale = MathHelper.Lerp(StartingScale, IdealScale, Utils.InverseLerp(0f, 30f, Time, true)); // Expand completely after 30 frames.

            // Determine the ideal scale in the first frame.
            if (IdealScale == 0f)
            {
                IdealScale = Main.rand.NextFloat(2.2f, 3f);
                projectile.netUpdate = true;
            }

            // Target enemy if possible and idly spawn tentacles.
            if (Time < 150)
            {
                TargetingMovement();
                if (Time % TentacleSpawnRate == TentacleSpawnRate - 1 && Main.myPlayer == projectile.owner)
                {
                    ProduceSubsumingHentai();
                }
            }
            // Slow down and pulse frequently.
            else if (Time < 220)
            {
                projectile.velocity *= 0.96f;
                if (Time % PulseInterval == 0f)
                {
                    PulseEffect();
                }
            }
            else if (Time >= 240)
            {
                ExplodeEffect();
                projectile.Kill();
            }
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(projectile.scale * 62));
            Time++;
        }

        public void ProduceSubsumingHentai()
        {
            int tentacleDamage = (int)(projectile.damage * 0.25f);
            float xStartingAcceleration = Main.rand.NextFloat(0.001f, 0.04f) * Main.rand.NextBool(2).ToDirectionInt();
            float yStartingAcceleration = Main.rand.NextFloat(0.001f, 0.04f) * Main.rand.NextBool(2).ToDirectionInt();
            Projectile subsumingHentai = Projectile.NewProjectileDirect(
                projectile.Center,
                Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 13f),
                ModContent.ProjectileType<SubsumingTentacle>(),
                tentacleDamage,
                projectile.knockBack * 0.5f,
                projectile.owner,
                xStartingAcceleration,
                yStartingAcceleration
            );

            subsumingHentai.tileCollide = false;
        }

        public void TargetingMovement()
        {
            NPC potentialTarget = projectile.Center.ClosestNPCAt(600f, true, true);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 5f + projectile.SafeDirectionTo(potentialTarget.Center) * 7f) / 6f;
        }

        public void PulseEffect()
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, PulseHitboxExpandRatio);
            projectile.Damage();
            if (!Main.dedServ)
            {
                Main.PlaySound(SoundID.Item43, projectile.Center);
                for (int i = 0; (float)i < 85; i++)
                {
                    float angle = i / 85f * MathHelper.TwoPi;
                    Vector2 spawnPosition = projectile.Center + angle.ToRotationVector2() * (500f + Main.rand.NextFloat(-8f, 8f));
                    Vector2 velocity = (angle - (float)Math.PI).ToRotationVector2() * Main.rand.NextFloat(27f, 38.5f);
                    Dust dust = Dust.NewDustPerfect(spawnPosition, 264, velocity);
                    dust.scale = 0.9f;
                    dust.fadeIn = 1.25f;
                    dust.color = Main.hslToRgb(i / 85f, 1f, 0.8f);
                    dust.noGravity = true;
                }
            }
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 1f / PulseHitboxExpandRatio);
        }

        public void ExplodeEffect()
        {
            Main.PlaySound(SoundID.Zombie, projectile.Center, 104);
            if (!Main.dedServ)
            {
                for (int i = 0; i < 200; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(50f, 50f);
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 264, velocity);
                    dust.color = Main.hslToRgb(i / 100f % 1f, 1f, 0.8f);
                    dust.fadeIn = 1.25f;
                    dust.noGravity = true;
                }
            }
            if (Main.myPlayer == projectile.owner)
            {
                int vortexDamage = (int)(projectile.damage * 0.75f);
                NPC closestTarget = projectile.Center.ClosestNPCAt(1600f, true, true);

                for (int i = 0; i < 12; i++)
                {
                    float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 velocity = Vector2.UnitY.RotatedBy(rotation);
                    if (closestTarget != null)
                        velocity = projectile.SafeDirectionTo(closestTarget.Center, -Vector2.UnitY).RotatedByRandom(0.4f);

                    velocity *= Main.rand.NextFloat(3f, 5f);

                    Projectile.NewProjectileDirect(projectile.Center,
                        velocity,
                        ModContent.ProjectileType<Vortex>(),
                        vortexDamage,
                        projectile.knockBack,
                        projectile.owner,
                        0f,
                        Main.rand.NextFloat(0.5f, 1.8f));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int vortexesToDraw = 27;
            for (int i = 0; i < vortexesToDraw; i++)
            {
                float rotation = MathHelper.TwoPi / i * vortexesToDraw + projectile.rotation;
                float drawScale = projectile.scale * 0.66f;

                drawScale *= (float)(Math.Cos(Time / 18f + rotation * 2f) + 1f) * 0.3f + 0.7f; // Range of 0.7 to 1. Used to give variance with the scale.
                float offsetFactor = (float)(Math.Sin(Time / 18f + rotation * 2f) + 1f) * 0.2f + 0.8f; // Range of 0.8 to 1.

                // Due to the way RotatedBy works, the offset can be negative, giving the projectile a
                // dual buzzsaw look.
                Vector2 drawOffset = Vector2.UnitX.RotatedBy(rotation) * 30f * offsetFactor * drawScale;
                Vector2 drawPosition = projectile.Center + drawOffset - Main.screenPosition;
                Color colorToDraw = Main.hslToRgb((i / (float)vortexesToDraw + Time / 40f) % 1f, 1f, 0.75f);
                spriteBatch.Draw(ModContent.GetTexture(Texture),
                                 drawPosition,
                                 null,
                                 colorToDraw * 0.7f,
                                 rotation,
                                 ModContent.GetTexture(Texture).Size() * 0.5f,
                                 drawScale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }
    }
}
