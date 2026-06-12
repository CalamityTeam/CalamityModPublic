using System;
using CalamityMod.Dusts;
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
    public class OldLordClaymoreHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<OldLordClaymore>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<OldLordClaymore>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/OldLordClaymore";
        public int size = 76;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public override float AdditionalScale => 0.2f;
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public int useAnim;
        public int swingCount;
        public bool finalFlip = false;
        public bool playSwingSound = true;
        public bool swooshFade = false;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
        }
        public override void WhenSpawned()
        {
            CanHit = false;
            Projectile.knockBack = 0;
            Projectile.ai[1] = -1;
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1 ? true : false;
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

            if (CanHit && !swooshFade)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.5f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.35f);


            if (!doSwing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                Projectile.numHits = 0;
                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;
                doSwing = true;
                swingCount++;
                finalFlip = false;
                playSwingSound = true;
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

                if (AnimationProgress < (useAnim / 1.5f))
                {
                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        doSwing = false;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.7f, useAnim, Animation, true)) * 0.35f)), 0.2f);
                }
                else
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
                    }
                    if (time > (int)(timeMax * 0.2f) && time < (int)(timeMax * 0.8f))
                        CanHit = true;
                    else
                        CanHit = false;
                    if (time > (int)(timeMax * 0.7f))
                        swooshFade = true;
                    else
                        swooshFade = false;

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, CalamityUtils.ExpInOutEasing(time / timeMax, 1))),
                        0.2f);

                    if (time >= timeMax)
                        doSwing = false;
                    if (time < (int)(timeMax * 0.7f))
                        postSwing = true;

                    if (CanHit)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(Owner.Center + (new Vector2(95 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), ModContent.DustType<SquashDust>(), Vector2.One.RotatedByRandom(MathHelper.Pi) * 0.6f, 0, default, Main.rand.NextFloat(1.15f, 1.5f) * Projectile.scale);
                            dust.noGravity = true;
                            dust.color = Main.rand.NextBool() ? Color.Orange :Color.OrangeRed;
                            dust.fadeIn = Projectile.scale - 1;
                        }
                        float randRot = Main.rand.NextFloat(-30, -60);
                        Vector2 dustVel = (new Vector2(0, 8 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));

                        GeneralParticleHandler.SpawnParticle(new SparkParticle(Owner.Center + (new Vector2(95 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), dustVel, false, 23, Main.rand.NextFloat(0.4f, 0.8f), Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed));
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

            target.AddBuff(BuffID.OnFire3, 180);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            CalamityUtils.MoveNPC(target, launchVel, 12, true, Owner);

            int dustNum = (int)MathHelper.Clamp(12 - Projectile.numHits * 3, 3, 12);
            for (int i = 0; i < dustNum; i++)
            {
                float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                int dustStyle = 278;
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle, Projectile.velocity);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance);
                dust2.velocity = (launchVel * 25).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.Orange : Color.OrangeRed;
            }

            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.type == ModContent.ProjectileType<BladecrestOathswordThrownBlade>() && projectile.ai[2] == target.whoAmI && projectile.localAI[0] != 5)
                {
                    projectile.owner = Owner.whoAmI;
                    projectile.localAI[0] = 5;
                    projectile.velocity = (Vector2.Lerp(projectile.velocity, launchVel * 14, 0.7f) * 1.2f).RotatedByRandom(0.1f);
                }
            }

            if (Projectile.numHits == 0)
            {
                Owner.SetScreenshake(5f);
                SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordStrongImpact");
                SoundEngine.PlaySound(swing with { Volume = 0.95f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
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
            if ((useAnim > 0 || DrawUnconditionally) && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OldLordClaymoreGlow");
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearLarge");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;

                for (int i = 0; i < 20; i++)
                {
                    Color auraColor = Color.OrangeRed with { A = 0 } * 0.18f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 4 * fadeIn;
                    Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.OrangeRed with { A = 0 } * fadeIn * 0.5f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(Projectile.ai[1] == 1 ? -70 : 70) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 0.325f, SpriteEffects.None);


                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), glowTex.Frame(1, FrameCount, 0, Frame), Color.White, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(glowTex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}
