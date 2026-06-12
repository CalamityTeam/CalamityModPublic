using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
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
    public class StellarStrikerHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<StellarStriker>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<StellarStriker>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/StellarStriker";
        public int size = 118;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public int useAnim;
        public int swingCount;
        public bool spawnBoom = true;
        public bool finalFlip = false;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
        }
        public override void WhenSpawned()
        {
            Projectile.knockBack = 0;
            Projectile.ai[1] = -1;

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, it does not influence Stellar Striker's projectile spawning
            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0 ? true : false;
        }

        public override void UseStyle()
        {
            AnimationProgress = Animation % useAnim;
            DrawUnconditionally = false;

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.Calamity().mouseWorld;
            }

            if (CanHit)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.1f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.15f);


            if (!doSwing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                Projectile.numHits = 0;
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0 ? true : false;
                doSwing = true;
                swingCount++;
                spawnBoom = true;
                finalFlip = false;
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
                    
                
                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians((Owner.direction == -1 ? 0 : 120)), 0.1f);

                if (AnimationProgress < (useAnim / 3))
                {
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        doSwing = false;
                        //Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = RotationOffset.AngleLerp(MathHelper.ToRadians(-45f * Projectile.ai[1] * Owner.direction), 0.2f);
                }
                else
                {
                    if (!finalFlip)
                    {
                        FlipAsSword = Owner.direction < 0 ? true : false;
                    }

                    float time = (AnimationProgress) - (useAnim / 8);
                    float timeMax = useAnim - (useAnim / 8);

                    if (time == (int)(timeMax * 0.3f))
                    {
                        SoundStyle fire = new("CalamityMod/Sounds/Item/TerratomereSwing");
                        SoundEngine.PlaySound(fire with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.65f, 0.75f) }, Projectile.Center);
                        SoundStyle fire2 = new("CalamityMod/Sounds/Item/SwingMid");
                        SoundEngine.PlaySound(fire2 with { Volume = 0.6f, Pitch = Main.rand.NextFloat(0.05f, 0.12f) }, Projectile.Center);
                    }
                    if ( time > (int)(timeMax * 0.25f) && time < (int)(timeMax * 0.85f))
                    {
                        CanHit = true;
                    }
                    else
                        CanHit = false;

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(-45f * Projectile.ai[1] * Owner.direction, 405f * -Projectile.ai[1] * Owner.direction, CalamityUtils.ExpInOutEasing(time / timeMax, 1))),
                        0.2f);

                    if (time >= timeMax)
                        doSwing = false;
                    if (time < (int)(timeMax * 0.85f))
                        postSwing = true;
                }
                if (CanHit)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 particleVel = new Vector2(0, 3 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                        Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(5, 130), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;
                        Particle orb = new CustomPulse(particlePos, -particleVel * Main.rand.NextFloat(1.1f, 3.2f), Main.rand.NextBool(4) ? Color.PaleTurquoise : Color.Turquoise, "CalamityMod/Particles/Sparkle", new Vector2(2f, 1f), particleVel.ToRotation(), Main.rand.NextFloat(0.4f, 1.1f) * Projectile.scale, 0.2f, 23);
                        GeneralParticleHandler.SpawnParticle(orb);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        float randRot = Main.rand.NextFloat(-30, -60);
                        Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                        Particle spark2 = new GlowSparkParticle(Owner.Center + (new Vector2(140 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.4f)), -dustVel, false, 20, Main.rand.NextFloat(0.007f, 0.012f) * Projectile.scale, Color.Turquoise, new Vector2(1.3f, 0.5f), true, false, Main.rand.NextFloat(0.5f, 0.7f));
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                }
                else if (Main.rand.NextBool(3))
                {
                    Vector2 particleVel = new Vector2(0, 3 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                    Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(5, 130), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;

                    Particle orb = new CustomPulse(particlePos, particleVel * Main.rand.NextFloat(0.8f, 1.2f), Main.rand.NextBool(4) ? Color.PaleTurquoise : Color.Turquoise, "CalamityMod/Particles/HealingPlus", new Vector2(1f, 1f), Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(0.8f, 1.2f) * Projectile.scale, 0.2f, 23);
                    GeneralParticleHandler.SpawnParticle(orb);

                    //GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(particlePos, -particleVel.RotatedByRandom(0.2f) * 2, Main.rand.NextBool(4) ? Color.PaleTurquoise : Color.Turquoise, 23, Main.rand.NextFloat(0.2f, 0.6f), 0.65f, 0, true));
                }
                Lighting.AddLight(Projectile.Center, Color.Turquoise.ToVector3() * fadeIn);
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
            {
                Projectile.numHits -= 1;
            }

            SoundStyle fire = new("CalamityMod/Sounds/Item/CursedDaggerThrow");
            SoundEngine.PlaySound(fire with { Volume = 0.65f, Pitch = 0.7f }, Projectile.Center);
            SoundStyle fire2 = new("CalamityMod/Sounds/Item/MagicRockSound");
            SoundEngine.PlaySound(fire2 with { Volume = 0.55f, Pitch = 0.6f }, Projectile.Center);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, 17, true, Owner);

            for (int i = 0; i < MathHelper.Clamp(10 - Projectile.numHits * 2, 2, 10); i++)
            {
                Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -20).RotatedByRandom(0.4) * Main.rand.NextFloat(0.2f, 1f));
                dust2.scale = Main.rand.NextFloat(0.55f, 0.95f);
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise;
                if (Main.rand.NextBool(4))
                {
                    Particle spark3 = new CustomSpark(target.Center, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -24).RotatedByRandom(0.5) * Main.rand.NextFloat(0.2f, 1f), "CalamityMod/Particles/Sparkle", false, 35, Main.rand.NextFloat(0.3f, 1f), Main.rand.NextBool(3) ? Color.PaleTurquoise : Color.Turquoise, new Vector2(0.6f, 1.5f));
                    GeneralParticleHandler.SpawnParticle(spark3);
                }
            }

            if (spawnBoom)
            {
                Vector2 spawnSpot = target.Center + new Vector2(Main.rand.NextFloat(-550, 550), Main.rand.NextFloat(-750, -950));
                Projectile meteor = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnSpot, Vector2.Zero, ModContent.ProjectileType<StellarStrikerMeteor>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, 0, 0, 7);
                meteor.scale = Projectile.scale;
                spawnBoom = false;
            }

            target.AddBuff(ModContent.BuffType<Nightwither>(), 500);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.3f;
            int hitsToMinMult = 5;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/CircularSmearSmokey");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;

                Main.EntitySpriteDraw(swoosh.Value, Owner.Center - Main.screenPosition, null, Color.Turquoise with { A = 0 } * (float)Math.Pow(fadeIn, 3), Projectile.rotation + MathHelper.PiOver4 * Owner.direction + RotationOffset * 1.75f, swoosh.Size() * 0.5f, Projectile.scale * 2f, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));

                for (int i = 0; i < 25; i++)
                {
                    Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StellarStrikerGhost").Value;
                    Color auraColor = Color.Turquoise with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * 6 * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}
