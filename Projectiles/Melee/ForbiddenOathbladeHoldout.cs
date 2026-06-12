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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistException]
    public class ForbiddenOathbladeHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<ForbiddenOathblade>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ForbiddenOathblade>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ForbiddenOathblade";
        public int size = 74 + 10;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size - 10);
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
        public int postSwingCooldownMax => (int)(useAnim * 0.65f);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
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
                Animation--;

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
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.23f);

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
                        SoundEngine.PlaySound(swing2 with { Volume = 0.65f, Pitch = Main.rand.NextFloat(0.4f, 0.5f) }, Projectile.Center);
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
                        for (int i = 0; i < 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Owner.Center + (new Vector2(113 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), ModContent.DustType<SquashDust>(), Vector2.One.RotatedByRandom(MathHelper.Pi) * 0.6f, 0, default, Main.rand.NextFloat(1.15f, 1.5f) * Projectile.scale);
                            dust.noGravity = true;
                            dust.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                            dust.fadeIn = Projectile.scale - 1;
                        }
                        float randRot = Main.rand.NextFloat(-30, -60);
                        Vector2 dustVel = (new Vector2(0, 8 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                        Vector2 placement = Owner.Center + (new Vector2(113 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f));
                        GeneralParticleHandler.SpawnParticle(new CustomSpark(placement, dustVel, "CalamityMod/Particles/DemonSigilParticle", false, 23, Main.rand.NextFloat(0.23f, 0.36f), Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet, new Vector2(1, 1), shrinkSpeed: 0.2f));
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

            bool hasKillMode = Owner.Calamity().cooldowns.TryGetValue(KillMode.ID, out CooldownInstance killModeCD);

            int bonusDamage = 120;
            if (target.Calamity().demonicFlamesBonusDamage <= bonusDamage)
            {
                target.Calamity().demonicFlamesBonusDamage = bonusDamage;
                target.AddBuff(ModContent.BuffType<DemonicFlames>(), 180);
                // Demonic Flames damage must be synced, because OnHitNPC is only run for the client that hit the NPC
                if (Main.netMode != NetmodeID.SinglePlayer)
                    DemonicFlamesSyncPacket.Send(target);
            }

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            CalamityUtils.MoveNPC(target, launchVel, 17, true, Owner);

            int dustNum = (int)MathHelper.Clamp(12 - Projectile.numHits * 3, 3, 12);
            for (int i = 0; i < dustNum; i++)
            {
                float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                int dustStyle = 278;
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                dust2.velocity = (launchVel * 25).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
            }

            for (int i = 0; i < 2; i++)
            {
                Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, Color.BlueViolet, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.7f * (i + 1), 1f, 18, true);
                GeneralParticleHandler.SpawnParticle(blastRing);
                Particle blastRing2 = new CustomPulse(target.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.35f * (i + 1), 0.5f, 18, true);
                GeneralParticleHandler.SpawnParticle(blastRing2);
            }

            if (!hasLaunchedBlades)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<ForbiddenOathbladeThrownBlade>() && projectile.ai[2] == target.whoAmI && projectile.localAI[0] != 5)
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
                Owner.SetScreenshake(5.5f);
                for (int i = 0; i < 4; i++)
                    GeneralParticleHandler.SpawnParticle(new CustomSpark(target.Center + launchVel * 15, launchVel.RotatedBy((0.15f - 0.05f * i) * (i % 2 == 0 ? -1 : 1)) * (10 + 10 * i), "CalamityMod/Particles/DemonSigilParticle", false, 11, 0.7f - 0.15f * i, Color.MediumOrchid, new Vector2(1.5f, 1), extraRotation: MathHelper.ToRadians(i % 2 == 0 ? 90 : 0), shrinkSpeed: (i % 2 == 0 ? -0.8f : 0.8f)));

                SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordStrongImpact");
                SoundEngine.PlaySound(swing with { Volume = 1f, Pitch = MathHelper.Clamp(swingCount * 0.05f, -0.1f, 0.65f) }, Projectile.Center);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.3f;
            int hitsToMinMult = 7;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally))
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearLarge");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;
                float deathFade = willDie ? Utils.GetLerpValue(0, postSwingCooldownMax, postSwingCooldown, true) : 1;

                for (int i = 0; i < 20; i++)
                {
                    Color auraColor = Color.MediumOrchid with { A = 0 } * 0.18f * (willDie ? deathFade : fadeIn);
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 6 * (willDie ? (1 - deathFade) * 2 : fadeIn);
                    Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.BlueViolet with { A = 0 } * fadeIn * 0.65f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 == 0 ? -70 : 70) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 0.377f, SpriteEffects.None);


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
            writer.Write7BitEncodedInt(postSwingCooldown);
            writer.WriteVector2(aimVel);
            writer.Write(Animation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            willDie = reader.ReadBoolean();
            useAnim = reader.Read7BitEncodedInt();
            postSwingCooldown = reader.Read7BitEncodedInt();
            aimVel = reader.ReadVector2();
            Animation = reader.ReadSingle();
            mousePos = Owner.Center - aimVel;
        }
    }
}
