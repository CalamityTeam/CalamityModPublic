using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Collections.Generic;
using CalamityMod.Graphics.Metaballs;

namespace CalamityMod.Projectiles.Magic
{
    public class SpiritCongregation : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Vector2 HoverOffset = Vector2.Zero;
        public ref float Time => ref Projectile.ai[0];
        public ref float BaseDamage => ref Projectile.ai[1];
        public ref float DeathCounter => ref Projectile.localAI[0];
        public bool WasStrongBefore
        {
            get => Projectile.localAI[1] == 1f;
            set => Projectile.localAI[1] = value.ToInt();
        }
        public Player Owner => Main.player[Projectile.owner];
        public float CurrentPower => (float)Math.Pow(Utils.GetLerpValue(15f, 840f, Time, true), 4D);
        public float CongregationDiameter => MathHelper.SmoothStep(54f, 185f, CurrentPower);
        public float MovementSpeed
        {
            get
            {
                float movementSpeed = 9f;

                // Make speed gradually build up over time, with growths at certain points.
                movementSpeed += MathHelper.SmoothStep(0f, 2.2f, Utils.GetLerpValue(0.18f, 0.3f, CurrentPower, true));
                movementSpeed += MathHelper.SmoothStep(0f, 4f, Utils.GetLerpValue(0.4f, 0.52f, CurrentPower, true));
                movementSpeed += MathHelper.SmoothStep(0f, 5f, Utils.GetLerpValue(0.6f, 0.72f, CurrentPower, true));
                movementSpeed += MathHelper.SmoothStep(0f, 6f, Utils.GetLerpValue(0.8f, 0.92f, CurrentPower, true));

                return movementSpeed;
            }
        }
        public const float LargeMouthPowerLowerBound = 0.62f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 108;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.timeLeft = 90000;
            Projectile.hide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DeathCounter);
            writer.WriteVector2(HoverOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DeathCounter = reader.ReadSingle();
            HoverOffset = reader.ReadVector2();
        }

        public override void AI()
        {
            // If close to dying slow down and do nothing else.
            if (DeathCounter > 0f)
            {
                DeathCounter++;
                if (DeathCounter >= 35f)
                    Projectile.Kill();
                Projectile.scale = 1f - DeathCounter / 35f;
                Projectile.velocity *= 0.98f;
                EmitGhostGas();
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                // Prepare for death prior to not being channeled.
                if (!Owner.channel)
                {
                    DeathCounter = 1f;
                    Projectile.netUpdate = true;
                    return;
                }

                Vector2 previousVelocity = Projectile.velocity;

                // If not sufficiently close to the mouse, move towards it.
                if (!Projectile.WithinRange(Main.MouseWorld, 80f))
                    MoveTowardsMouse();

                // Otherwise slow down to a point.
                else if (Projectile.velocity.Length() > 4f)
                    Projectile.velocity *= 0.95f;

                // Sync velocity if it changed from what it was before.
                if (previousVelocity != Projectile.velocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            // Handle dynamic damage.
            if (BaseDamage == 0f)
                BaseDamage = Projectile.damage;
            else
            {
                float damageBoostFactor = MathHelper.SmoothStep(1f, 1.85f, CurrentPower);
                Projectile.damage = (int)(BaseDamage * damageBoostFactor);
            }

            // Increment the timer on the final extra update.
            if (Projectile.FinalExtraUpdate())
                Time++;

            // Set the hover offset to a zero vector after enough time has passed and the projectile is powerful/tame enough.
            bool tame = CurrentPower > 0.97f;
            if (tame)
            {
                if (HoverOffset != Vector2.Zero)
                    HoverOffset = Vector2.Zero;
            }

            // Previously, Gruesome Eminence stopped being able to hurt you when fully charged.
            // This has been changed so that its threat is everpresent.
            Projectile.hostile = Time > 75f;
            // Projectile.hostile = !tame && Time > 75f;

            // Explode into a burst of spirit dust and gas clouds when a bigger face appears.
            if (!WasStrongBefore && CurrentPower > LargeMouthPowerLowerBound)
            {
                float burstDirectionVariance = 3;
                float burstSpeed = 14f;
                for (int j = 0; j < 16; j++)
                {
                    burstDirectionVariance += j * 2;
                    for (int k = 0; k < 40; k++)
                    {
                        Dust burstDust = Dust.NewDustPerfect(Projectile.Center, 267);
                        burstDust.scale = Main.rand.NextFloat(1.74f, 2.5f);
                        burstDust.position += Main.rand.NextVector2Circular(10f, 10f);
                        burstDust.velocity = Main.rand.NextVector2Square(-burstDirectionVariance, burstDirectionVariance).SafeNormalize(Vector2.UnitY) * burstSpeed;
                        burstDust.color = Color.Lerp(Color.DarkViolet, Color.Black, Main.rand.NextFloat(0.6f));
                        burstDust.noGravity = true;
                    }
                    burstSpeed += 1.8f;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4f, 4f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dustVelocity, ModContent.ProjectileType<SpiritDust>(), 0, 0f, Projectile.owner);
                    }
                }

                SoundEngine.PlaySound(SoundID.DD2_BetsyFlyingCircleAttack, Projectile.Center);
                WasStrongBefore = true;
            }

            // Otherwise, if not tame, periodically define a new offset.
            // This determines where to fly to and adds unpredictability at first.
            else if (Main.myPlayer == Projectile.owner && Time % 55f == 54f)
            {
                float maxHoverOffset = MathHelper.SmoothStep(460f, 0f, CurrentPower);
                HoverOffset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.6f, 1f) * maxHoverOffset;
                Projectile.netUpdate = true;
            }

            // Release spirits that fly towards the target
            if (!tame)
                ReleaseSmallSpirits();

            EmitGhostGas();
            UpdateFrames();
        }

        public void MoveTowardsMouse()
        {
            // Make inertia become more significant the more power the congregation has, due to growing size.
            float inertia = MathHelper.Lerp(18f, 40f, CurrentPower);

            Vector2 directionToMouseOffset = Projectile.SafeDirectionTo(Main.MouseWorld + HoverOffset);
            Vector2 directionToOwner = Projectile.SafeDirectionTo(Owner.Center);
            Vector2 idealVelocity = Vector2.Lerp(directionToMouseOffset, directionToOwner, 0.25f) * MovementSpeed;

            // Approach the ideal velocity.
            Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, MovementSpeed * 0.04f);
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + idealVelocity) / inertia;
        }

        public void ReleaseSmallSpirits()
        {
            if (Projectile.WithinRange(Owner.Center, 230f) || Time % 45f != 44f)
                return;

            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 spiritVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7f, 10f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, spiritVelocity, ModContent.ProjectileType<SmallSpirit>(), 70, 0f, Projectile.owner, Projectile.identity);
            }
        }

        public void EmitGhostGas()
        {
            float particleSize = CongregationDiameter;
            if (Projectile.oldPosition != Projectile.position && Time > 2f)
                particleSize += (Projectile.oldPosition - Projectile.position).Length() * 4.2f;

            // Place a hard limit on particle sizes.
            if (particleSize > 210f)
                particleSize = 210f;

            // Make particles shrink when dying.
            particleSize *= MathHelper.Lerp(1f, 0.5f, Utils.GetLerpValue(0f, 35f, DeathCounter, true));

            int particleSpawnCount = Main.rand.NextBool(8) ? 3 : 1;
            for (int i = 0; i < particleSpawnCount; i++)
            {
                // Summon a base particle.
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(1f, 1f) * particleSize / 26f;
                GruesomeMetaball.SpawnParticle(spawnPosition, Main.rand.NextVector2Circular(4.4f, 4.4f), particleSize);

                // And an "ahead" particle that spawns based on current movement.
                // This causes the "head" of the overall thing to have bumps when moving.
                spawnPosition += Projectile.velocity.RotatedByRandom(1.38f) * particleSize / 105f;
                GruesomeMetaball.SpawnParticle(spawnPosition, Main.rand.NextVector2Circular(i * 1.5f + 7f, i * 1.5f + 7f), particleSize * 0.3f);

                // Make particle sizes exponentially decrease across loop iterations.
                particleSize *= 0.9f;
            }

            // Release gas projectiles randomly. This does not happen when dying.
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(16) && DeathCounter <= 0f)
            {
                Vector2 dustVelocity = -Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(1.04f) * 1.5f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dustVelocity, ModContent.ProjectileType<SpiritDust>(), 0, 0f, Projectile.owner);
            }
        }

        public void UpdateFrames()
        {
            int maxFrame = CurrentPower <= LargeMouthPowerLowerBound ? 6 : 9;
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % maxFrame;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            // Ensure that the teeth draw over the metaballs.
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (CurrentPower > LargeMouthPowerLowerBound)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SpiritCongregationBig").Value;

            DrawHead(texture);
            return false;
        }

        public void DrawHead(Texture2D texture, float scaleFactor = 1f)
        {
            int maxFrame = CurrentPower <= LargeMouthPowerLowerBound ? 6 : 9;
            float offsetFactor = Projectile.scale * ((CongregationDiameter - 54f) / 90f + 1.5f);
            offsetFactor *= texture.Width / 90f;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * offsetFactor * 15f;
            Rectangle frame = texture.Frame(1, maxFrame, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects direction = MathF.Cos(Projectile.rotation) > 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale * scaleFactor, direction, 0);
        }

        public void DrawHeadForMetaball()
        {
            Texture2D backTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SpiritCongregationBack").Value;
            if (CurrentPower > LargeMouthPowerLowerBound)
                backTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/SpiritCongregationBackBig").Value;

            DrawHead(backTexture, 1.04f);
        }

        // Damage scales up over time as it grows.
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float damageFactor = 0.25f + 0.75f * CurrentPower;
            int fullPowerDamage;
            if (Main.masterMode)
                fullPowerDamage = 540;
            else if (Main.expertMode)
                fullPowerDamage = 450;
            else
                fullPowerDamage = 360;

            modifiers.SourceDamage *= 0f;
            modifiers.SourceDamage.Flat += (int)(damageFactor * fullPowerDamage);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float hitboxRadius = CongregationDiameter * MathHelper.Lerp(0.45f, 0.17f, i / (float)Projectile.oldPos.Length) * 1.4f;
                Vector2 hitboxCircle = Vector2.One * hitboxRadius;
                if (CalamityUtils.CircularHitboxCollision(Projectile.oldPos[i] + hitboxCircle * 0.5f, hitboxRadius, targetHitbox))
                    return true;
            }
            return false;
        }
    }
}
