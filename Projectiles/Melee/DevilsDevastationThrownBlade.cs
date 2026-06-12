using System;
using System.IO;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Packets.Entities;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsDevastationThrownBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/DevilsDevastation";

        public int ChargeupTime => (int)Projectile.localAI[2];
        public int Lifetime = 300;
        public float OverallProgress => 1 - Projectile.timeLeft / (float)Lifetime;
        public float ThrowProgress => 1 - Projectile.timeLeft / (float)(Lifetime);
        public float ChargeProgress => 1 - (Projectile.timeLeft - Lifetime) / (float)(ChargeupTime);
        public Player Owner => Main.player[Projectile.owner];

        // Real variables here
        public ref float time => ref Projectile.ai[0];
        public ref float stabOrder => ref Projectile.ai[1];
        public ref NPC stabbedTarget => ref Main.npc[(int)Projectile.ai[2]];

        public Color clr = Color.Lerp(Color.DeepPink, Color.Orange, 0.5f);
        public Color usedColor = Color.White;
        public bool thrown = false;
        public int fadeOutTime = 240;
        public bool stuckInTarget = false;
        public int stuckTimer = 0;
        public bool exitedTarget = false;
        public bool stuckInGround = false;
        public Vector2 impalePos;
        public int bounces = 0;
        public Vector2 tipPosition = Vector2.Zero;
        public bool fadingOut => Projectile.timeLeft <= (Lifetime - fadeOutTime);
        private bool visualChargeInitialized = false;
        private int visualChargeTimer = 0;
        private int visualChargeTime = 0;

        private Vector2 GetAimDirection()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 toMouse = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                float aimAngle = toMouse.ToRotation();
                if (Math.Abs(MathHelper.WrapAngle(aimAngle - Projectile.localAI[1])) > 0.02f)
                {
                    Projectile.localAI[1] = aimAngle;
                    if (Projectile.timeLeft % 6 == 0)
                        Projectile.netUpdate = true;
                }
                return toMouse;
            }
            return Projectile.localAI[1].ToRotationVector2();
        }
        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime + ChargeupTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 toMouse = (Owner.Calamity().mouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                Projectile.localAI[1] = toMouse.ToRotation();
                Projectile.netUpdate = true;
                Projectile.scale = Owner.GetMeleeScale();
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return (ChargeProgress >= 1 && !stuckInGround && !stuckInTarget);
        }

        //Swing animation keys
        public CurveSegment pullback = new CurveSegment(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new CurveSegment(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement(float progress) => PiecewiseAnimation(progress, new CurveSegment[] { pullback, throwout });

        public override void AI()
        {
            bool isOwner = Main.myPlayer == Projectile.owner;
            if (!visualChargeInitialized && !isOwner)
            {
                int fallbackCharge = (int)MathHelper.Clamp((Owner.HeldItem.useTime / 2.8f), 1, 100);
                visualChargeTime = (int)(Projectile.localAI[2] > 0f ? Projectile.localAI[2] : fallbackCharge);
                visualChargeTimer = visualChargeTime;
                visualChargeInitialized = true;
            }

            if (!thrown && ChargeProgress >= 1f)
                thrown = true;

            if (thrown && !stuckInGround && !stuckInTarget && !exitedTarget)
                Projectile.extraUpdates = 3;

            float rate = (Main.GlobalTimeWrappedHourly + time * 3) * 2;
            List<Color> eColors = new List<Color>()
            {
                clr,
                Color.BlueViolet
            };
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            usedColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.spriteDirection = Projectile.direction;
            Vector3 Light = Color.MediumOrchid.ToVector3();
            Lighting.AddLight(Projectile.Center, Light * 0.85f);

            if (Projectile.timeLeft == Lifetime && Main.myPlayer == Projectile.owner)
            {
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 toMouse = GetAimDirection();
                Projectile.velocity = toMouse * 14;
                Projectile.Center = Owner.MountedCenter + toMouse * 30f * Projectile.scale;
                Projectile.spriteDirection = Projectile.direction;
                thrown = true;
                time = 0;
                Projectile.extraUpdates = 3;
                Projectile.netUpdate = true;
            }

            if (Projectile.velocity.X > 0)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;

            if (thrown)
            {
                Projectile.spriteDirection = Projectile.direction;

                if (!stuckInGround && exitedTarget)
                    Projectile.rotation += (0.15f * (MathF.Abs(Projectile.velocity.Y) * 0.03f + 0.85f)) * Main.rand.NextFloat(0.7f, 1f) * Projectile.direction * Projectile.Opacity;
                else
                    Projectile.rotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver4 * (Projectile.direction == 1 ? 1 : 3));

                if (time > fadeOutTime * 0.7f && !exitedTarget)
                    Projectile.velocity *= 0.93f;

                if (stuckInTarget)
                {
                    Projectile.tileCollide = false;
                    time--;
                    Projectile.timeLeft++;
                    if (stuckTimer > 0)
                        stuckTimer--;
                    else
                    {
                        Projectile.velocity *= 0.01f;
                        stabbedTarget.Calamity().demonSwordImpales--;
                        stuckInTarget = false;
                        FadeOutEffect();
                    }

                    Projectile.Center = stabbedTarget.Center + impalePos;
                    if ((stabbedTarget.life <= 0) || stabbedTarget == null || Projectile.localAI[0] == 5 || !stabbedTarget.active)
                    {
                        Projectile.timeLeft = Lifetime;
                        stabbedTarget.Calamity().demonSwordImpales--;
                        stuckInTarget = false;
                        exitedTarget = true;

                        Projectile.rotation += Main.rand.NextFloat(-1.2f, 1.2f);
                        for (int i = 0; i < Main.maxNPCs; i++)
                            Projectile.localNPCImmunity[i] = 0;
                        Projectile.numHits = 0;
                        SoundStyle unstuck = new("CalamityMod/Sounds/NPCHit/PerfLargeHit", 3);
                        SoundEngine.PlaySound(unstuck with { Volume = 0.85f, Pitch = Main.rand.NextFloat(0.3f, 0.4f), MaxInstances = 3 }, Projectile.Center);
                    }
                }
                if (exitedTarget && !stuckInGround)
                {
                    Projectile.extraUpdates = 5;
                    Projectile.velocity *= 0.987f;
                }

                if (fadingOut)
                    FadeOutEffect();
            }
            if (!fadingOut && !stuckInTarget && !stuckInGround && ChargeProgress >= 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 safeVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 dustVel = exitedTarget ? safeVel.RotatedBy(MathHelper.ToRadians(-90 * Projectile.direction) + Projectile.rotation) : safeVel.RotatedBy(MathHelper.ToRadians(70 * (i == 0 ? 1 : -1))) * 6;
                    if (!exitedTarget)
                    {
                        Particle spark = new VelChangingSpark(Projectile.Center + safeVel * 95 * Projectile.scale, -dustVel, -Projectile.velocity, "CalamityMod/Particles/BloomCircle", 5, 0.2f * Projectile.scale, (clr) * 0.95f, new Vector2(1.2f, 1f), true, false, 0, false, 1.0f, 0.22f);
                        GeneralParticleHandler.SpawnParticle(spark);

                        Particle spark2 = new VelChangingSpark(Projectile.Center + safeVel * 95 * Projectile.scale, -dustVel, -Projectile.velocity, "CalamityMod/Particles/BloomCircle", 10, 0.2f * Projectile.scale, (Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet) * 0.65f, new Vector2(1.2f, 1f), true, false, 0, false, 1.0f, 0.22f);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    float rot = Projectile.rotation + (MathHelper.TwoPi * i);
                    Vector2 vel = (Utils.MoveTowards(-Projectile.velocity, new Vector2(0, -130).RotatedBy(rot).RotatedBy(-1.3f * Projectile.direction), (Utils.GetLerpValue(5, 2, Projectile.velocity.Length(), true))));
                    if (exitedTarget && i == 0)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, -70 * Projectile.scale).RotatedBy(rot), Main.rand.NextBool(4) ? 278 : ModContent.DustType<SquashDust>());
                        dust2.noGravity = (dust2.type == 278 ? false : true);
                        dust2.scale = (dust2.type == 278 ? 0.75f : 0.9f) * Projectile.scale;
                        dust2.color = Main.rand.NextBool() ? Color.BlueViolet : clr;
                        dust2.velocity = (vel * 2).RotatedByRandom(0.4f);
                        dust2.fadeIn = (dust2.type == 278 ? 0 : Projectile.scale - 1);
                    }
                }
                
                if (Main.rand.NextBool(4))
                {
                    Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.6f), "CalamityMod/Particles/DemonSigilParticle", false, 17, Main.rand.NextFloat(0.2f, 0.3f) * Projectile.scale, Color.Lerp(Color.MediumOrchid, Color.BlueViolet, Main.rand.NextFloat(0, 0.7f)) * 0.6f, new Vector2(1, 1), true, false, Main.rand.NextFloat(-1f, 1f), false, false);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }

            //Anticipation animation. Make the player look like they're holding the item
            float chargeProgress = ChargeProgress;
            bool canVisualCharge = !isOwner && Projectile.velocity.LengthSquared() < 0.01f && visualChargeTimer > 0;
            if (canVisualCharge)
            {
                chargeProgress = 1f - (visualChargeTimer / (float)visualChargeTime);
                visualChargeTimer--;
            }
            if (chargeProgress < 1f)
            {
                ThrowAnimation(chargeProgress);
                if (ChargeProgress >= 0.6f && time == 0)
                {
                    SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordSwing", 2);
                    SoundEngine.PlaySound(swing with { Volume = 0.65f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                    time++;
                }
                return;
            }
            else
                time++;
        }

        public void FadeOutEffect()
        {
            Projectile.tileCollide = false;
            if (Projectile.timeLeft > Lifetime - fadeOutTime)
                Projectile.timeLeft = Lifetime - fadeOutTime;
            Projectile.Opacity = Utils.GetLerpValue(0, Lifetime - fadeOutTime, Projectile.timeLeft, true);

            Vector2 dustVel = new Vector2(10, 10).RotatedByRandom(Math.PI) * Main.rand.NextFloat(0.2f, 1f) * Projectile.Opacity;
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), dustVel, 0, default, Main.rand.NextFloat(1.1f, 1.4f) * Projectile.scale);
                dust.noGravity = true;
                dust.color = Main.rand.NextBool() ? Color.MediumOrchid : clr;
                dust.noLight = true;
                dust.noLightEmittence = true;
                dust.alpha = 100;
                dust.velocity += Projectile.velocity;
                dust.fadeIn = Projectile.scale - 1;
            }
        }
        public void ThrowAnimation(float chargeProgress)
        {
            Vector2 aimDirection = GetAimDirection();
            Owner.ChangeDir(MathF.Sign(aimDirection.X));

            float armRotation = ArmAnticipationMovement(chargeProgress) * Owner.direction;

            Owner.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = Owner.direction;
            Projectile.direction = Owner.direction;

            Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * -80f * Owner.gravDir * Projectile.scale;
            Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRotation) * Owner.gravDir;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int bonusDamage = 1000;
            if (target.Calamity().demonicFlamesBonusDamage <= bonusDamage)
            {
                target.Calamity().demonicFlamesBonusDamage = bonusDamage;
                target.AddBuff(ModContent.BuffType<DemonicFlames>(), 120);
                // Demonic Flames damage must be synced, because OnHitNPC is only run for the client that hit the NPC
                if (Main.netMode != NetmodeID.SinglePlayer)
                    DemonicFlamesSyncPacket.Send(target);
            }

            if (!exitedTarget)
            {
                SoundStyle stuck = new("CalamityMod/Sounds/Item/HellkiteSmallHit", 3);
                SoundEngine.PlaySound(stuck with { Volume = 0.65f, Pitch = Main.rand.NextFloat(0.4f, 0.5f), MaxInstances = 3 }, Projectile.Center);
                SoundStyle stuck2 = new("CalamityMod/Sounds/Item/DemonSwordImpact", 2);
                SoundEngine.PlaySound(stuck2 with { Volume = 0.8f, Pitch = Main.rand.NextFloat(-0.3f, -0.2f), MaxInstances = 3 }, Projectile.Center);

                Projectile.ai[2] = target.whoAmI;

                if (target.Calamity().demonSwordImpales < 0)
                    target.Calamity().demonSwordImpales = 0;
                if (target.Calamity().demonSwordImpales >= 5)
                {
                    float bladeValue = -1;
                    Projectile ejectedBlade = null;
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.owner == Projectile.owner && projectile.type == Projectile.type && Projectile.localAI[0] != 5 && projectile.ai[2] == Projectile.ai[2] && projectile.timeLeft > (Lifetime - fadeOutTime) && (bladeValue == -1 || bladeValue > projectile.ai[1]))
                        {
                            bladeValue = projectile.ai[1];
                            ejectedBlade = projectile;
                        }
                    }
                    ejectedBlade.localAI[0] = 5;
                    ejectedBlade.ai[1] += 1000;
                    ejectedBlade.velocity = Projectile.velocity;

                    for (int i = 0; i < 12; i++)
                    {
                        Particle spark2 = new SparkParticle(target.Center, Projectile.velocity.RotatedByRandom(0.45) * Main.rand.NextFloat(2.5f, 4.5f), false, 50, Main.rand.NextFloat(0.2f, 1.3f), Main.rand.NextBool() ? clr : Color.BlueViolet);
                        GeneralParticleHandler.SpawnParticle(spark2);

                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.RotatedByRandom(0.35) * Main.rand.NextFloat(2f, 4f), 0, default, Main.rand.NextFloat(1.3f, 1.6f));
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.color = Main.rand.NextBool() ? Color.MediumOrchid : clr;
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Particle spark2 = new SparkParticle(target.Center, Projectile.velocity.RotatedByRandom(0.3) * Main.rand.NextFloat(0.4f, 2.5f), false, 50, Main.rand.NextFloat(0.2f, 1.3f), Main.rand.NextBool() ? Color.MediumOrchid : clr);
                        GeneralParticleHandler.SpawnParticle(spark2);

                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.RotatedByRandom(0.2) * Main.rand.NextFloat(0.5f, 2f), 0, default, Main.rand.NextFloat(1.1f, 1.4f));
                        dust.noGravity = false;
                        dust.noLight = true;
                        dust.color = Main.rand.NextBool() ? clr : Color.BlueViolet;
                    }
                }
                Projectile crack = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 2).RotatedByRandom(0.3f), ModContent.ProjectileType<DevilsStrike>(), 0, 0f, Projectile.owner, 0.8f, 1);
                crack.timeLeft = 200;
                target.Calamity().demonSwordImpales++;

                stuckInTarget = true;
                impalePos = Projectile.Center - stabbedTarget.Center;
                stuckTimer = 3600;
            }
            Projectile.netUpdate = true;
            if (Main.netMode != NetmodeID.SinglePlayer)
                DemonSwordImpalesSyncPacket.Send(target);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.45f;
            int hitsToMinMult = 9;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult * (exitedTarget ? 1.15f : 1f);
        }
        public override bool? CanDamage()
        {
            if (ChargeProgress < 1 || fadingOut || stuckInGround || stuckInTarget || (Projectile.numHits > 0 && !exitedTarget))
                return false;

            return base.CanDamage();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => (Projectile.numHits > 0 && !exitedTarget) ? false : CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width / (exitedTarget ? 0.3f : 1) * Projectile.scale, targetHitbox); // After exiting a target, the hitbox is larger
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/DevilsDevastation").Value;
            Vector2 generalDrawPos = Projectile.Center - Main.screenPosition;
            Color clr = Color.Lerp(Color.DeepPink, Color.Orange, 0.35f);
            float fadeScale = (1 - Projectile.Opacity) + (stuckInTarget ? 0.55f : 0);
            for (int i = 0; i < 16; i++)
            {
                Color auraColor = usedColor with { A = 0 } * 0.4f * fadeScale * Projectile.Opacity;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 16f).ToRotationVector2() * 9 * fadeScale) + Main.rand.NextVector2Circular(3, 3);
                Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset, null, auraColor, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            if (exitedTarget && !stuckInGround)
            {
                Asset<Texture2D> p = ModContent.Request<Texture2D>("CalamityMod/Particles/CircularSmearFire3");
                Asset<Texture2D> p2 = ModContent.Request<Texture2D>("CalamityMod/Particles/SemiCircularSmearSwipe");

                Main.EntitySpriteDraw(p2.Value, generalDrawPos, null, clr with { A = 0 } * 0.65f * Projectile.Opacity, Projectile.rotation * Main.rand.NextFloat(1.6f, 1.7f), p2.Size() * 0.5f, 1.4f * Main.rand.NextFloat(0.8f, 1.15f) * Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                Main.EntitySpriteDraw(p.Value, generalDrawPos, null, Color.MediumOrchid with { A = 0 } * 0.75f * Projectile.Opacity, Projectile.rotation * Main.rand.NextFloat(1.2f, 1.3f), p.Size() * 0.5f, 1.25f * Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }
            Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.BlueViolet with { A = 0 }, lightColor, Projectile.Opacity) * Projectile.Opacity, Projectile.rotation, centerTexture.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            byte state = 0;
            if (thrown)
                state |= 1;
            if (stuckInTarget)
                state |= 2;
            if (exitedTarget)
                state |= 4;
            if (stuckInGround)
                state |= 8;

            writer.Write(state);
            writer.Write(Projectile.rotation);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
            writer.Write(impalePos.X);
            writer.Write(impalePos.Y);
            writer.Write(stuckTimer);
            writer.Write((short)bounces);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            byte state = reader.ReadByte();
            thrown = (state & 1) != 0;
            stuckInTarget = (state & 2) != 0;
            exitedTarget = (state & 4) != 0;
            stuckInGround = (state & 8) != 0;

            Projectile.rotation = reader.ReadSingle();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
            impalePos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            stuckTimer = reader.ReadInt32();
            bounces = reader.ReadInt16();
        }
    }
}
