using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DazzlingStabber : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        public ref float AttackDelay => ref Projectile.ai[0];
        public ref float RestOffsetAngle => ref Projectile.ai[1];
        public ref float KnifeType => ref Projectile.ai[2]; // 1 - Crystal, 2 - Stone, 3 - Fire
        public Color bladeColor = Color.White;
        public int attackCooldown = 0;
        public int attacksDone = 0;
        private bool justHit = false;
        public NPC potentialTarget;
        public float moveSmoothing = 50;
        public float damageMult = 1;
        public Vector2 storedPos;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = false; // To prevent blade type desync
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 50;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0.333f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 28;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCooldown);
            writer.Write(attacksDone);
            writer.Write(justHit);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCooldown = reader.ReadInt32();
            attacksDone = reader.ReadInt32();
            justHit = reader.ReadBoolean();
        }

        public override void AI()
        {
            switch (KnifeType)
            {
                case 1: // Crystal
                    bladeColor = Color.Orchid;
                    Projectile.extraUpdates = 1;
                    damageMult = 0.5f;
                    Projectile.ArmorPenetration = 40;
                    break;
                case 2: // Rock
                    bladeColor = Color.OrangeRed;
                    Projectile.extraUpdates = 2;
                    damageMult = 3;
                    break;
                default: // Flame
                    bladeColor = Color.Orange;
                    damageMult = 1;
                    Projectile.ArmorPenetration = 20;
                    break;
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());

            ApplyPlayerBuffs();

            potentialTarget = Projectile.Center.MinionHoming(1100f, Owner);

            if (potentialTarget != null && attackCooldown == 0 && AttackDelay <= 0)
            {
                storedPos = potentialTarget.Center;
            }

            // On-hit visuals
            if (justHit)
            {
                if (KnifeType == 1)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 0.7f, Pitch = attacksDone * 0.05f }, Projectile.Center);
                    if (attacksDone >= 6)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.5f) * Main.rand.NextFloat(10f, 20f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(1.25f, 1.75f);
                            dust.color = Color.Orchid;
                            dust.noLightEmittence = true;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            Particle spark = new CustomSpark(Projectile.Center, (Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotatedByRandom(0.6f) * Main.rand.NextFloat(15f, 20f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 25, Main.rand.NextFloat(1.35f, 1.6f), Color.Orchid, new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.25f, 0.35f));
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                    }
                }
                if (KnifeType == 2)
                {
                    SoundStyle slam = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 2);
                    SoundEngine.PlaySound(slam with { Volume = 0.4f, Pitch = Main.rand.NextFloat(-0.25f, -0.15f), MaxInstances = -1 }, Projectile.Center);
                    for (int i = 0; i < 7; i++)
                    {
                        Particle spark = new CustomSpark(Projectile.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(1f, 5f), "CalamityMod/Projectiles/Typeless/ArtifactOfResilienceShard6", false, Main.rand.Next(15, 25 + 1), Main.rand.NextFloat(0.75f, 1.2f), Color.White, new Vector2(1.3f, 0.5f), false, false, Main.rand.NextFloat(-5, 5), false, false);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                justHit = false;
            }

            if (potentialTarget != null)
            {
                if (KnifeType == 1 && attacksDone > 0 && Utils.Distance(Projectile.Center, storedPos) < 150 && attackCooldown == 0)
                {
                    Particle spark = new GlowSparkParticle(Projectile.Center, -Projectile.velocity * 0.1f, false, 20, 0.03f, bladeColor * 0.55f, new Vector2(0.8f, 2.3f), true, false, 0.45f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (KnifeType == 2 && attackCooldown == 0)
                {
                    if (Main.rand.NextBool(4))
                    {
                        Particle spark = new CustomSpark(Projectile.Center + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.8f), "CalamityMod/Projectiles/Typeless/ArtifactOfResilienceShard6", false, Main.rand.Next(25, 35 + 1), Main.rand.NextFloat(0.55f, 1.2f), Color.White, Vector2.One, false, false, Main.rand.NextFloat(-5, 5), false, false);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else
                    {
                        bool memesHatesLists = Main.rand.NextBool(5);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, memesHatesLists ? 278 : ModContent.DustType<LightDust>(), -Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.4f) * Main.rand.NextFloat(1f, 4f));
                        dust.noGravity = !memesHatesLists;
                        dust.scale = Main.rand.NextFloat(0.55f, 1.1f);
                        dust.color = bladeColor;
                    }
                }
                if (KnifeType == 3)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.4f) * Main.rand.NextFloat(2f, 8f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.65f, 1f);
                    dust.color = Color.Goldenrod;
                    dust.noLightEmittence = true;

                    Particle spark2 = new CustomSpark(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 5f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 14, Main.rand.NextFloat(0.9f, 1.2f), Color.Gold, new Vector2(0.5f, 1.5f), true, false, 0, false, false, -0.25f);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    for (int i = 0; i < 2; i++)
                    {
                        Particle trail = new GlowSparkParticle(Projectile.Center + i * Projectile.velocity * 0.4f, -Projectile.velocity * 0.05f, false, 9, 0.05f, Color.Orange * 0.6f, new Vector2(1, 0.3f), true, false, 0.85f);
                        GeneralParticleHandler.SpawnParticle(trail);
                    }

                }
                if (attackCooldown == 0)
                    SliceTarget(potentialTarget);
                else
                {
                    manageAttackCooldown();
                }
            }
            else
            {
                manageAttackCooldown();
                ReturnToRestingPosition();
            }
        }
        public void manageAttackCooldown()
        {
            if (attackCooldown > 0)
            {
                attackCooldown--;
                if (KnifeType == 1)
                    ReturnToRestingPosition();
                else if (KnifeType == 2)
                    Projectile.velocity *= 0.96f;
                if (attackCooldown == 0)
                {
                    attacksDone = 0;

                    if (KnifeType == 1)
                    {
                        Particle orb2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Orchid, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.7f, 10);
                        GeneralParticleHandler.SpawnParticle(orb2);
                        Particle orb3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Orchid, "CalamityMod/Particles/SmallBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.35f, 15, true);
                        GeneralParticleHandler.SpawnParticle(orb3);

                        for (int i = 0; i < 6; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(4f, 8f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.65f, 1.15f);
                            dust.color = Color.Orchid;
                            dust.noLightEmittence = true;
                        }
                    }
                }
            }
        }
        public void ApplyPlayerBuffs()
        {
            Owner.AddBuff(ModContent.BuffType<DazzlingStabberBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<DazzlingStabber>())
            {
                if (Owner.dead)
                    Owner.Calamity().providenceStabber = false;
                if (Owner.Calamity().providenceStabber)
                    Projectile.timeLeft = 2;
            }
        }
        public void SliceTarget(NPC target)
        {
            moveSmoothing = 50;
            // Don't do anything if the attack delay is still passing.
            if (AttackDelay > -60f)
            {
                AttackDelay--;
                if (AttackDelay > 0f)
                {
                    if (KnifeType == 1)
                    {
                        if (!Projectile.WithinRange(storedPos, 150f))
                            Projectile.velocity = Projectile.velocity.RotatedBy(0.1f);
                        else
                            Projectile.velocity *= 1.004f;
                    }
                    return;
                }
            }

            // Reset the velocity to fly upward if there is very little motion to ensure
            // that the summon does not get stuck.
            if (Projectile.velocity.Length() < 3f)
                Projectile.velocity = Vector2.UnitY * -12f;

            // If close to the target, slow down dramatically.
            if (Projectile.velocity.Length() > 5f && Projectile.WithinRange(storedPos, 50f))
                Projectile.velocity *= 0.93f;

            // Otherwise, if not close, speed up dramatically.
            else if (Projectile.velocity.Length() < (KnifeType == 2 ? 15 : 40f))
                Projectile.velocity *= 1.03f;

            float angularTurnSpeed = 0.15f;
            float angleToTargetCoords = Projectile.AngleTo(storedPos);

            if (KnifeType == 3)
            {
                if (!Projectile.WithinRange(storedPos, 50f))
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(angleToTargetCoords, angularTurnSpeed).ToRotationVector2() * Projectile.velocity.Length();
            }
            if (KnifeType == 2)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(storedPos) * 15f;
            }
            if (KnifeType == 1)
            {
                if (!Projectile.WithinRange(storedPos, 400f))
                    Projectile.velocity = Projectile.SafeDirectionTo(storedPos) * 20f;
            }
            // If not super close to the target but the target is very much in the line of sight of the summon, charge.
            if (!Projectile.WithinRange(storedPos, 75f) && Vector2.Dot(Projectile.SafeDirectionTo(storedPos), Projectile.velocity.SafeNormalize(Vector2.Zero)) > 0.99f && KnifeType == 3)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(storedPos) * 36f;
                AttackDelay = 15f;

                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void ReturnToRestingPosition()
        {
            if (moveSmoothing > 2)
                moveSmoothing--;
            Projectile.rotation = Projectile.rotation.AngleTowards(RestOffsetAngle + Main.GlobalTimeWrappedHourly, 0.25f);

            float sine = Math.Abs((float)Math.Sin((Main.GlobalTimeWrappedHourly + RestOffsetAngle * 1.8f) * 8f / MathHelper.Pi));

            // Rapidly approach the resting position.
            Vector2 destination = Owner.Center + Vector2.UnitY.RotatedBy(RestOffsetAngle + Main.GlobalTimeWrappedHourly) * -220f * (MathHelper.Lerp(sine, 0.8f, 0.8f));
            Projectile.velocity = (destination - Projectile.Center) / (potentialTarget == null ? moveSmoothing : 18);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            justHit = true;
            if (KnifeType == 1)
            {
                AttackDelay = 15;
                attacksDone++;
                if (attacksDone >= 6)
                    attackCooldown = Main.rand.Next(80, 95 + 1);
            }
            if (KnifeType == 2)
            {
                Projectile.velocity = (-Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15).RotatedByRandom(0.7f);
                attackCooldown = Main.rand.Next(45, 60 + 1);
            }
            if (KnifeType == 3)
            {
                if (attackCooldown == 0)
                    attackCooldown = 5;
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            }
            Projectile.netUpdate = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= damageMult;

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

        public override bool? CanDamage() => (attackCooldown == 0 ? base.CanDamage() : false);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            if (KnifeType == 1)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/DazzlingStabber").Value;
            if (KnifeType == 2)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/DazzlingStabber2").Value;
            if (KnifeType == 3)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/DazzlingStabber3").Value;

            Projectile.DrawProjectileWithBackglow(bladeColor with { A = 0 }, lightColor, 3.5f, tex);

            return false;
        }
    }
}
