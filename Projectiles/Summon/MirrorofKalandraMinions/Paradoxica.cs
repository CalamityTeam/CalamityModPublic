using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.MirrorofKalandraMinions
{
    public class Paradoxica : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(MirrorofKalandra.TargetDistanceDetection, Owner);
        public ref float AITimer => ref Projectile.ai[0];
        public ref float Oscillation => ref Projectile.ai[1];
        public bool hasTeleported = false;
        public Vector2 ChargeStartingPosition;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 12000;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = MirrorofKalandra.Scimitar_IFrames;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 104;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasTeleported);
            writer.WriteVector2(ChargeStartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasTeleported = reader.ReadBoolean();
            ChargeStartingPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            CheckMinionExistence();

            if (Main.rand.NextBool(30))
            {
                GenericSparkle sparkle = new GenericSparkle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 4),
                    Vector2.Zero,
                    Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(1f)),
                    Color.White,
                    Main.rand.NextFloat(.6f, 1f),
                    20,
                    Main.rand.NextFloat(.08f, .12f),
                    Main.rand.NextFloat(.1f, .3f));
                GeneralParticleHandler.SpawnParticle(sparkle);
            }

            if (Target is not null)
            {
                if (!hasTeleported)
                {
                    int dustAmount = 60;
                    for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
                    {
                        float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                        Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(5f, 15f);
                        Dust teleportDust = Dust.NewDustPerfect(Projectile.Center, 212, velocity, 0, default, 2f);
                        teleportDust.noGravity = true;
                    }

                    Projectile.Center = Target.Center;
                    hasTeleported = true;
                }

                //
                // Code from Virid Vanguard @ DoBehaviour_RegularPierceSlashes().
                //

                int attackCycleTime = 66; // 1.5x as long as Virid Vanguard.
                float upwardRiseTimeRatio = 0.4f;
                float pierceTimeRatio = 0.14f;

                // Initialize the starting position on the first frame.
                int localAITimer = (int)AITimer;
                if (localAITimer % attackCycleTime == 1f)
                {
                    ChargeStartingPosition = Projectile.Center + Main.rand.NextVector2Circular(80f, 80f);
                    Projectile.netUpdate = true;
                }

                float attackCompletion = localAITimer / (float)attackCycleTime % 1f;

                // Reset the trail point array if aiming at the target.
                if (attackCompletion < upwardRiseTimeRatio)
                    Projectile.oldPos = new Vector2[Projectile.oldPos.Length];

                // Use extra updates to make the swords move faster than normal.
                Projectile.MaxUpdates = 2;

                // This represents the entire attack cycle in a single interpolant via a bunch of GetLerpValues and vector math. The patterns is as follows:
                // 1. Hover very quickly to the starting position.
                // 2. Rise upward somewhat.
                // 3. Lerp towards the target rapidly.
                // 4. Lerp through the target rapidly.
                // 5. Repeat.
                float offsetDistanceFactor = MathHelper.Lerp(1.61f, 3f, 1f / 7f % 1f);
                Vector2 startingPosition = ChargeStartingPosition + Vector2.UnitY * Utils.GetLerpValue(0f, upwardRiseTimeRatio, attackCompletion, true) * -200f;
                Vector2 targetOffset = Target.Center - startingPosition;
                Vector2 endingPosition = Target.Center + targetOffset.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(targetOffset.Length(), 60f, 240f) * offsetDistanceFactor;
                float pierceCompletion = Utils.GetLerpValue(upwardRiseTimeRatio, upwardRiseTimeRatio + pierceTimeRatio, attackCompletion, true);
                float throughTargetCompletion = Utils.GetLerpValue(upwardRiseTimeRatio + pierceTimeRatio, 1f, attackCompletion, true);

                // Update rotation and interpolate through the necessary positions.
                Projectile.rotation = Projectile.rotation.AngleTowards(targetOffset.ToRotation() + MathHelper.PiOver2, MathHelper.Pi / 5f);
                Projectile.Center = Vector2.Lerp(Projectile.Center, Vector2.Lerp(startingPosition, Target.Center, pierceCompletion), pierceCompletion * 0.5f);
                if (throughTargetCompletion > 0f)
                    Projectile.Center = Vector2.Lerp(Target.Center, endingPosition, throughTargetCompletion);
                Projectile.velocity = Vector2.Zero;

                // Play a slice sound once ready to charge.
                if (localAITimer % attackCycleTime == (int)(attackCycleTime * upwardRiseTimeRatio))
                    SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound with { Pitch = 1.6f, Volume = 0.1f }, Projectile.Center);

                AITimer++;
            }
            else if (Target is null)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center + Projectile.rotation.ToRotationVector2() * (MirrorofKalandra.IdleDistanceFromPlayer + MirrorofKalandra.IdleDistanceFromPlayer * (MathF.Sin(Oscillation) / MirrorofKalandra.OscillationRange)), .4f);
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver2 + MathHelper.PiOver4 / 1.5f, .15f);
                Oscillation += MirrorofKalandra.OscillationSpeed;

                hasTeleported = false;
                Projectile.extraUpdates = 0;
                AITimer = 0f;
            }
        }

        public void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<KalandraMirrorBuff>(), 3600);
            if (Projectile.type != ModContent.ProjectileType<Paradoxica>())
                return;

            if (Owner.dead)
                ModdedOwner.KalandraMirror = false;
            if (ModdedOwner.KalandraMirror)
                Projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SparkParticle sparkOnHit = new SparkParticle(Vector2.Lerp(Projectile.Center, target.Center, .8f),
                (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * .01f, // It'll rotate, but due to the multiplier it won't move.
                false,
                20,
                Main.rand.NextFloat(1.2f, 1.8f),
                Color.White);
            GeneralParticleHandler.SpawnParticle(sparkOnHit);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            float rotation = (Target is not null) ? Projectile.rotation : Projectile.rotation + MathHelper.PiOver4;

            if (CalamityConfig.Instance.Afterimages && Target is not null)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = Color.Gray with { A = 125 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length);
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
