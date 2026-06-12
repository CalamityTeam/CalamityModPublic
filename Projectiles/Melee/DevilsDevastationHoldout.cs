using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Packets.Entities;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistException]
    public class DevilsDevastationHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<DevilsDevastation>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<DevilsDevastation>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/DevilsDevastation";
        public Color clr = Color.Lerp(Color.DeepPink, Color.Orange, 0.5f);
        public SlotId SoundSlot;
        public int size = 118 + 285;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size - 285);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override float AdditionalScale => 0.15f;
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public int useAnim;
        public int swingCount;
        public bool finalFlip = false;
        public bool playSwingSound = true;
        public bool holding = true;
        public int postSwingCooldown = 0;
        public bool willDie = false;
        public bool hasLaunchedBlades = false;
        public bool swooshFade = false;
        private int lastSwingId;

        public NPC lastHitTarget = null;
        private int lastHitTargetIndex = -1;
        public int postSwingCooldownMax => (int)(useAnim * 0.65f);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            // This is needed because EntitySource_ItemUse always sets Projectile.originalDamage to the item's damage,
            // regardless of the value passed into the damage argument of NewProjectile.
            // This results in continuous updating damage resetting the damage.
            Projectile.originalDamage *= 15;
        }
        public override void WhenSpawned()
        {
            IgnoreActiveAnimation = true;
            DrawUnconditionally = true;
            CanHit = false;
            Projectile.knockBack = 0;
            Projectile.ai[1] = -1;
            bool isOwner = Main.myPlayer == Projectile.owner;
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            if (isOwner)
            {
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                Projectile.netUpdate = true;
            }
            else
            {
                Vector2 syncedDelta = Owner.Calamity().mouseWorldDeltaFromPlayer;
                if (syncedDelta.LengthSquared() > 0.001f)
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65f;
                else
                    aimVel = Vector2.UnitX * Owner.direction * 65f;
                mousePos = Owner.Center - aimVel;
            }
            useAnim = (int)(Owner.HeldItem.useAnimation / Owner.GetTotalAttackSpeed<MeleeDamageClass>());
            postSwingCooldown = postSwingCooldownMax / 2;
            lastSwingId = (int)Projectile.ai[0];

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1 ? true : false;
        }
        public override void OnKill(int timeLeft)
        {
            Owner.Calamity().demonSwordKillMode = false;
            if (lastHitTarget != null && lastHitTarget.life > 0 && lastHitTarget.active && Owner.HeldItem.type == AssignedItemID && Main.myPlayer == Projectile.owner)
            {
                Owner.SetScreenshake(12.5f);
                Particle blastRing = new CustomPulse(lastHitTarget.Center, Vector2.Zero, clr, "CalamityMod/Particles/HighResHollowCircleHardEdge", Vector2.One, Main.rand.NextFloat(-10, 10), 0.05f, 0.6f, 20, true);
                GeneralParticleHandler.SpawnParticle(blastRing);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 2;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), lastHitTarget.Center, vel.RotatedBy(MathHelper.ToRadians(45)), ModContent.ProjectileType<DevilsStrike>(), 0, 0f, Owner.whoAmI, 1, 0);
                }
                Projectile strike = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), lastHitTarget.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)(Projectile.damage * 1.666f), 0f, Owner.whoAmI, lastHitTarget.whoAmI, 0);
                strike.DamageType = DamageClass.Melee;
                SoundStyle dieSound = new("CalamityMod/Sounds/Item/LanceofDestinyStrong");
                SoundEngine.PlaySound(dieSound with { Volume = 0.9f, Pitch = 0.3f }, lastHitTarget.Center);

                int bonusDamage = 4000;
                if (lastHitTarget.Calamity().demonicFlamesBonusDamage <= bonusDamage)
                {
                    lastHitTarget.Calamity().demonicFlamesBonusDamage = bonusDamage;
                    lastHitTarget.AddBuff(ModContent.BuffType<DemonicFlames>(), 210);
                    // I'm like 95% confident this shouldn't need a sync since OnKill is called on all clients
                }
            }
        }
        public override void UseStyle()
        {
            bool isOwner = Main.myPlayer == Projectile.owner;
            bool hasKillMode = Owner.Calamity().cooldowns.TryGetValue(KillMode.ID, out CooldownInstance killModeCD);

            if (!isOwner)
            {
                Vector2 syncedDelta = Owner.Calamity().mouseWorldDeltaFromPlayer;
                if (syncedDelta.LengthSquared() > 0.001f)
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65f;
            }

            if (lastHitTargetIndex >= 0)
            {
                NPC syncedTarget = Main.npc[lastHitTargetIndex];
                if (syncedTarget != null && syncedTarget.active && syncedTarget.life > 0)
                    lastHitTarget = syncedTarget;
            }

            if (isOwner)
            {
                bool shouldStartSwing = (Main.mouseLeft || (hasKillMode && killModeCD.timeLeft == KillMode.cooldownMax + 1)) && holding && postSwingCooldown == 0;
                if (shouldStartSwing)
                {
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    Projectile.ai[0] += 1f;
                    Projectile.netUpdate = true;
                }
            }

            int swingId = (int)Projectile.ai[0];
            if (swingId != lastSwingId && holding && postSwingCooldown == 0)
            {
                Animation = (int)(useAnim * 0.7f);
                holding = false;
                if (isOwner)
                {
                    killModeCD.timeLeft = KillMode.cooldownMax;
                    Owner.Calamity().killModeCooldown = KillMode.cooldownMax - 1;
                }
                swingCount++;
                lastSwingId = swingId;
            }

            if (SoundEngine.TryGetActiveSound(SoundSlot, out var Sound) && Sound.IsPlaying && lastHitTarget != null)
                Sound.Position = lastHitTarget.Center;
            if (postSwingCooldown > 0)
                postSwingCooldown--;
            else if (willDie)
            {
                if (isOwner)
                {
                    if (hasKillMode)
                        killModeCD.timeLeft = KillMode.cooldownMax;
                    Owner.Calamity().killModeCooldown = KillMode.cooldownMax;
                    Owner.Calamity().demonSwordKillMode = false;
                }
                DrawUnconditionally = false;
                Projectile.Kill();
                return;
            }
            if (isOwner && killModeCD.timeLeft < KillMode.cooldownMax)
            {
                killModeCD.timeLeft = KillMode.cooldownMax;
                Owner.Calamity().killModeCooldown = KillMode.cooldownMax;
            }

            if (holding)
            {
                Animation--;
            }

            AnimationProgress = Animation % useAnim;

            if (isOwner && !holding && Main.netMode != NetmodeID.SinglePlayer && (int)Animation % 3 == 0)
                Projectile.netUpdate = true;

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = isOwner ? Owner.Calamity().mouseWorld : (Owner.Center - aimVel);
            }

            if (CanHit && !swooshFade)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.5f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.2f);

            if (!doSwing)
            {
                Projectile.ai[1] = -Projectile.ai[1];
                holding = true;

                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                Projectile.numHits = 0;
                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                if (isOwner)
                {
                    mousePos = Owner.Calamity().mouseWorld;
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                }
                else
                {
                    mousePos = Owner.Center - aimVel;
                }
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;
                doSwing = true;
                finalFlip = false;
                playSwingSound = true;
                hasLaunchedBlades = false;
                if (isOwner && !Owner.Calamity().demonSwordKillMode && postSwingCooldown == 0)
                {
                    if (!willDie)
                        Projectile.netUpdate = true;
                    willDie = true;
                    if (lastHitTarget != null)
                    {
                        useAnim = 115;
                        SoundStyle dieSound = new("CalamityMod/Sounds/Item/DemonSwordFinalStrike");
                        SoundSlot = SoundEngine.PlaySound(dieSound with { Volume = 1f, Pitch = 0 }, lastHitTarget.Center);
                    }
                }

                postSwingCooldown = postSwingCooldownMax;
            }
            else
            {
                if (!CanHit && !postSwing)
                {
                    if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                else
                {
                    if ((Owner.Center - aimVel).X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                
                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.1f);

                for (int i = 0; i < 4; i++)
                {
                    float deathFade = willDie ? Utils.GetLerpValue(0, postSwingCooldownMax, postSwingCooldown, true) : 1;
                    float randRot = Main.rand.NextFloat(-30, 30);
                    Vector2 dustVel = -Vector2.One.RotatedBy(Projectile.rotation + RotationOffset + MathHelper.ToRadians(90)).RotatedByRandom(0.3f) * Main.rand.NextFloat(7, 17) - Owner.velocity * 0.8f;
                    Vector2 placement = Owner.Center + (new Vector2(Main.rand.NextFloat(70, 500) * deathFade, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(placement, dustVel, "CalamityMod/Particles/BloomCircle", false, 13, Main.rand.NextFloat(0.04f, 0.06f) * 10 * Projectile.scale, (Main.rand.NextBool() ? Color.MediumOrchid : clr) * deathFade, new Vector2(0.9f, 1), shrinkSpeed: 0.95f));
                }
                if (lastHitTarget != null && lastHitTarget.life > 0 && lastHitTarget.active)
                {
                    float growth = (1 - (willDie ? Utils.GetLerpValue(0, postSwingCooldownMax, postSwingCooldown, true) : 1)) * 15;
                    for (int i = 0; i < 2; i++)
                    {
                        GeneralParticleHandler.SpawnParticle(new CustomSpark(lastHitTarget.Center, Vector2.One.RotatedBy(i == 0 ? MathHelper.ToRadians(90) : 0) * Main.rand.NextFloat(-3 - growth, 3 + growth), "CalamityMod/Particles/BloomCircle", false, 13, Main.rand.NextFloat(0.04f, 0.06f) * (10 + growth), Main.rand.NextBool() ? Color.MediumOrchid : clr, new Vector2(0.7f, 1), shrinkSpeed: 0.8f));
                    }
                }
                if (holding)
                {
                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    if (isOwner)
                        aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.7f, useAnim, Animation, true)) * 0.35f)), 0.2f);
                }
                else if (!willDie)
                {
                    if (!finalFlip)
                    {
                        FlipAsSword = Owner.direction < 0 ? true : false;
                    }

                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * 0.4f) && playSwingSound)
                    {
                        SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordSwing", 2);
                        SoundEngine.PlaySound(swing with { Volume = 0.85f, Pitch = Main.rand.NextFloat(-0.4f, -0.5f) }, Projectile.Center);
                        SoundStyle swing2 = new("CalamityMod/Sounds/Item/HeavySwing");
                        SoundEngine.PlaySound(swing2 with { Volume = 0.65f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) }, Projectile.Center);
                        playSwingSound = false;
                        if (isOwner)
                            Owner.Calamity().demonSwordKillMode = false;
                    }
                    if (time > (int)(timeMax * 0.2f) && time < (int)(timeMax * 0.9f))
                        CanHit = true;
                    else
                        CanHit = false;
                    if (time > (int)(timeMax * 0.7f))
                        swooshFade = true;
                    else
                        swooshFade = false;

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, CalamityUtils.ExpInOutEasing(time / timeMax * 0.9f, 1))),
                        0.2f);

                    if (time >= timeMax * 0.9f)
                    {
                        doSwing = false;
                    }
                    if (time < (int)(timeMax * 0.75f))
                        postSwing = true;

                    if (CanHit)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            float randRot = Main.rand.NextFloat(-20, -100);
                            Dust dust = Dust.NewDustPerfect(Owner.Center + (new Vector2(520 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(MathHelper.ToRadians(randRot))), ModContent.DustType<SquashDust>(), Vector2.One.RotatedByRandom(MathHelper.Pi) * 0.9f, 0, default, Main.rand.NextFloat(1.45f, 2.4f) * Projectile.scale);
                            dust.noGravity = true;
                            dust.color = Main.rand.NextBool() ? clr : Color.BlueViolet;
                            dust.fadeIn = Projectile.scale - 1;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            float randRot = Main.rand.NextFloat(-20, -100);
                            Vector2 dustVel = (new Vector2(0, 11 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                            Vector2 placement = Owner.Center + (new Vector2(520, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)) * Projectile.scale;
                            GeneralParticleHandler.SpawnParticle(new CustomSpark(placement, dustVel, "CalamityMod/Particles/DemonSigilParticle", false, 33, Main.rand.NextFloat(0.43f, 0.56f) * Projectile.scale, Main.rand.NextBool() ? Color.MediumOrchid : clr, new Vector2(1, 1), shrinkSpeed: 0.1f));
                        }
                    }   
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
                Projectile.numHits -= 1;

            if (target != null && target.life > 0 && target.active)
            {
                lastHitTarget = target;
                lastHitTargetIndex = target.whoAmI;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.netUpdate = true;
            }

            bool hasKillMode = Owner.Calamity().cooldowns.TryGetValue(KillMode.ID, out CooldownInstance killModeCD);

            int bonusDamage = 4000;
            if (target.Calamity().demonicFlamesBonusDamage <= bonusDamage)
            {
                target.Calamity().demonicFlamesBonusDamage = bonusDamage;
                target.AddBuff(ModContent.BuffType<DemonicFlames>(), 180);
                // Demonic Flames damage must be synced, because OnHitNPC is only run for the client that hit the NPC
                if (Main.netMode != NetmodeID.SinglePlayer)
                    DemonicFlamesSyncPacket.Send(target);
            }

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            CalamityUtils.MoveNPC(target, launchVel, 40, true, Owner);

            if (target.Calamity().demonSwordImpales != 0 || Projectile.numHits == 0)
            {
                int dustNum = (int)MathHelper.Clamp(12 - Projectile.numHits * 3, 3, 12);
                for (int i = 0; i < dustNum; i++)
                {
                    float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 vel = (launchVel * 55).RotatedBy(variance) * Main.rand.NextFloat(0.2f, 1f) * (1 - Math.Abs(variance));
                    int dustStyle = 278;
                    Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                    dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                    dust2.velocity = vel;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;

                    Particle spark = new SparkParticle(Projectile.Center, vel, true, 55, 1.25f, Main.rand.NextBool() ? clr : Color.MediumOrchid);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                for (int i = 0; i < 2; i++)
                {
                    Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, Color.BlueViolet, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.7f * (i + 1), 1.3f, 18, true);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                    Particle blastRing2 = new CustomPulse(target.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.35f * (i + 1), 0.8f, 18, true);
                    GeneralParticleHandler.SpawnParticle(blastRing2);
                }
            }
            
            if (!hasLaunchedBlades)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<DevilsDevastationThrownBlade>() && projectile.ai[2] == target.whoAmI && projectile.localAI[0] != 5)
                    {
                        projectile.owner = Owner.whoAmI;
                        projectile.localAI[0] = 5;
                        projectile.velocity = (launchVel * 16.5f).RotatedByRandom(0.25f);
                        Owner.Calamity().demonSwordKillMode = true;
                        if (hasKillMode)
                            killModeCD.timeLeft = KillMode.cooldownMax + KillMode.buffMax;
                        Owner.Calamity().killModeCooldown = KillMode.cooldownMax + KillMode.buffMax;
                        hasLaunchedBlades = true;
                    }
                }
            }

            if (Projectile.numHits == 0)
            {
                Owner.SetScreenshake(12f);
                for (int i = 0; i < 5; i++)
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(target.Center + launchVel * 15, launchVel.RotatedBy((0.15f - 0.05f * i) * (i % 2 == 0 ? -1 : 1)) * (10 + 10 * i), "CalamityMod/Particles/DemonSigilParticle", false, 11, 0.7f - 0.15f * i, Main.rand.NextBool() ? clr : Color.MediumOrchid, new Vector2(1.5f, 1), extraRotation: MathHelper.ToRadians(i % 2 == 0 ? 90 : 0), shrinkSpeed: (i % 2 == 0 ? -0.8f : 0.8f)));

                for (int i = 0; i < 20; i++)
                {
                    float distPow = Main.rand.NextFloat(0, 15);
                    Vector2 sparkVel = launchVel.RotatedByRandom(distPow * 0.01f) * Main.rand.NextFloat(5, 35 + distPow);
                    Particle sparks = new CustomSpark(target.Center + (Vector2.One * distPow).RotatedByRandom(MathHelper.Pi), sparkVel, "CalamityMod/Particles/GlowSpark", false, Main.rand.Next(13, 20 + 1) * 2, (Main.rand.NextFloat(0.5f, 0.85f) - distPow * 0.12f) * 0.2f, (Main.rand.NextBool() ? clr : Color.BlueViolet) * 0.75f, new Vector2(1.3f, 0.4f), true, false, 0, false, shrinkSpeed: 0.5f);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }


                SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordInsaneImpact");
                SoundEngine.PlaySound(swing with { Volume = 0.8f, Pitch = MathHelper.Clamp(swingCount * 0.05f, -0.25f, 0.5f) }, Projectile.Center);
                SoundStyle swing2 = new("CalamityMod/Sounds/Item/HellkiteBigHit1");
                SoundEngine.PlaySound(swing2 with { Volume = 0.8f, Pitch = MathHelper.Clamp(swingCount * 0.025f, 0.4f, 0.65f) }, Projectile.Center);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.35f;
            int hitsToMinMult = 8;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearLarge");
            Asset<Texture2D> swoosh2 = ModContent.Request<Texture2D>("CalamityMod/Particles/CircularTrail");

            float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;
            float deathFade = willDie ? Utils.GetLerpValue(0, postSwingCooldownMax, postSwingCooldown, true) : 1;
            if (true)
            {
                Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineAngled");
                int draws = 60;
                for (int i = 0; i < draws; i++)
                {
                    float sine = MathF.Sin(Main.GlobalTimeWrappedHourly * 25 + i * 6);
                    float newSine = 0.3f + (0.7f - sine * 0.25f) * Utils.Remap(i, 0, draws, 0.8f, 0.6f);
                    Vector2 offsetDir = Vector2.One.RotatedBy(Projectile.rotation + RotationOffset + MathHelper.ToRadians(90));

                    float jitter = Utils.Remap(i, 0, draws / 2, 10, 0f) * deathFade;
                    Color auraColor = Color.Lerp(Color.MediumOrchid, clr, Utils.GetLerpValue(0, draws - 1, i)) with { A = 0 } * 0.38f * (Utils.Remap(jitter, 0, 10, 1, 3.5f)) * (willDie ? deathFade : 1);
                    Vector2 drawOffset = -offsetDir * 5 * deathFade * i * Projectile.scale;
                    Main.EntitySpriteDraw(tex2.Value, Projectile.Center - offsetDir * 30 * Projectile.scale - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY) + Main.rand.NextVector2Circular(jitter, jitter), tex2.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation.ToRotationVector2().RotatedByRandom(jitter * 0.085f).ToRotation() + RotationOffset + r, new Vector2(FlipAsSword ? tex2.Width() : 0, tex2.Height()), Vector2.One * (1f - i * 0.01f) * 0.06f * Projectile.scale * newSine * Utils.Remap(jitter, 0, 10, 1, 0.3f), spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }
            }
            for (int i = 0; i < 20; i++)
            {
                Color auraColor = Color.MediumOrchid with { A = 0 } * 0.18f * (willDie ? deathFade : 1);
                Vector2 drawOffset = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 2 * (willDie ? (1 - deathFade) * 3 : 2);
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY) + Main.rand.NextVector2Circular(4, 4), tex.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            if ((useAnim > 0 || DrawUnconditionally))
            {
                Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.BlueViolet with { A = 0 } * fadeIn * 0.75f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 == 0 ? -65 : 65) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 1.3f, SpriteEffects.None);
                Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, clr with { A = 0 } * fadeIn * 0.85f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 == 0 ? -65 : 65) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 1.75f, SpriteEffects.None);

                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), Color.Lerp(Color.MediumOrchid with { A = 0 }, lightColor, deathFade) * deathFade, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            return false;
        }
        public override void ResetStyle()
        {
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(willDie);
            writer.Write7BitEncodedInt(useAnim);
            writer.Write7BitEncodedInt(lastHitTargetIndex);
            writer.Write7BitEncodedInt(postSwingCooldown);
            writer.WriteVector2(aimVel);
            writer.Write(Animation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            willDie = reader.ReadBoolean();
            useAnim = reader.Read7BitEncodedInt();
            lastHitTargetIndex = reader.Read7BitEncodedInt();
            postSwingCooldown = reader.Read7BitEncodedInt();
            aimVel = reader.ReadVector2();
            Animation = reader.ReadSingle();
            mousePos = Owner.Center - aimVel;
        }
    }
}
