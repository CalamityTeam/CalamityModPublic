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
    public class SkytideDragoonHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<SkytideDragoon>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<SkytideDragoon>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/SkytideDragoon";
        public int size = 140;
        public override float HitboxOutset => size * 1.45f;
        public override Vector2 HitboxSize => new Vector2(30, 30); // Has custom collision
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);

        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public float colorFadeIn = 0;
        public int useAnim;
        public int swingCount = 0;
        public float spearOutset = 0;
        public bool fireProj = true;
        public bool fancySwing => swingCount % 7 == 0;
        public bool redirected = false;
        public int attackPower = 0; // How many swings you have left before you have to call another bolt
        public float attackMult = 0; // A multiplier based on attack power that changes smoothly
        public Vector2 tipOutset;

        public Color color1 = Color.Orchid;
        public Color color2 = Color.Cyan;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void WhenSpawned()
        {
            Projectile.knockBack = 0;
            Projectile.ai[1] = 1;

            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            Projectile.rotation = Vector2.UnitY.ToRotation() + MathHelper.ToRadians(-135);
            useAnim = fancySwing ? (int)(Owner.itemAnimationMax * 2f) : Owner.itemAnimationMax;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1 ? true : false;
        }

        public override void UseStyle()
        {
            tipOutset = Vector2.One.RotatedBy(Projectile.rotation + MathHelper.ToRadians(90)) * spearOutset * 12;
            AbsolutePosition = Owner.MountedCenter + tipOutset;
            AnimationProgress = Animation % useAnim;
            DrawUnconditionally = false;

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.Calamity().mouseWorld;
            }

            if (CanHit)
            {
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.15f);
            }
            else
            {
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.12f);
            }
            colorFadeIn = MathHelper.Lerp(colorFadeIn, 0, 0.07f);

            attackMult = MathHelper.Lerp(attackMult, attackPower, 0.15f);


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
                FlipAsSword = Owner.direction == -1 ? true : false;
                fireProj = true;

                doSwing = true;

                swingCount++;

                if (swingCount == 1 && Owner.Calamity().mouseRight)
                    redirected = true;
                else
                    redirected = false;

                useAnim = (fancySwing || redirected) ? (int)(Owner.itemAnimationMax * 2f) : Owner.itemAnimationMax;
                AnimationProgress = Animation % useAnim;
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

                if (fancySwing)
                {
                    Projectile.rotation = Projectile.rotation.AngleLerp(Vector2.UnitY.ToRotation() + MathHelper.ToRadians(-135), (redirected ? 0.1f : 0.22f));
                }
                else if (redirected)
                {
                    float lerp = Utils.GetLerpValue(0, useAnim / 2f, AnimationProgress);
                    CanHit = true;
                    if (AnimationProgress < (useAnim / 2f))
                    {
                        Projectile.rotation += 0.81f * Owner.direction * (1 - lerp);
                        Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);
                        for (int i = 0; i < 2; i++)
                        {
                            Color color = Color.Lerp(Color.White, Main.rand.NextBool() ? color1 : color2, 0.65f);
                            Vector2 pos = Owner.Center + (new Vector2(160 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f));
                            Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.2f) * Main.rand.NextFloat(1f, 4f);
                            Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<SquashDust>(), vel, 0, default, Main.rand.NextFloat(1.85f, 2.2f) * Projectile.scale);
                            dust.noGravity = true;
                            dust.color = color;
                            dust.fadeIn = Projectile.scale - 1;
                            
                            Particle spark2 = new BoltParticle(pos, -vel.RotatedBy(Owner.direction == 1 ? MathHelper.ToRadians(45) : MathHelper.ToRadians(-135)) * Main.rand.NextFloat(1.5f, 1.8f), false, 13, Main.rand.NextFloat(0.2f, 0.35f) * Projectile.scale, color * 0.8f, new Vector2(1.2f, 1f), true, true, false, 0.25f);
                            GeneralParticleHandler.SpawnParticle(spark2);

                            if (i % 2 == 0)
                            {
                                Particle orb = new CustomSpark(pos, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 35, 0.75f * Projectile.scale, color * 0.75f, new Vector2(1f, 1));
                                GeneralParticleHandler.SpawnParticle(orb);
                            }
                        }
                    }
                    else
                    {
                        Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.15f);
                    }
                }
                else
                    Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.25f);

                if (AnimationProgress < (useAnim / 2))
                {
                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;

                    if (redirected)
                        CanHit = true;
                    else
                        CanHit = false;

                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        doSwing = false;
                    }
                    spearOutset = MathHelper.Lerp(spearOutset, MathHelper.ToRadians(120f), 0.2f);
                    FlipAsSword = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0 ? true : false;
                }
                else
                {
                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    spearOutset = MathHelper.Lerp(spearOutset, MathHelper.ToRadians(120f), 0.2f);

                    if (time == (int)(timeMax * 0.4f))
                    {
                        SoundStyle fire = new("CalamityMod/Sounds/Item/SkytideSwing");
                        SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) - (fancySwing ? 0.3f : 0) }, Projectile.Center);
                    }
                    if ( time > (int)(timeMax * 0.4f) && time < (int)(timeMax * 0.7f))
                    {
                        aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                        CanHit = true;
                    }
                    else
                    {
                        if (fireProj && time >= (int)(timeMax * 0.7f))
                        {
                            if (fancySwing) // Call down the bolt
                            {
                                Owner.SetScreenshake(3f);
                                SoundStyle sound = new("CalamityMod/Sounds/Item/SkytideBolt");
                                SoundEngine.PlaySound(sound with { Volume = 0.8f }, Projectile.Center);
                                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(0, -600), new Vector2(0, 10), ModContent.ProjectileType<DragoonBigBolt>(), (int)(Projectile.damage * 10), Projectile.knockBack, Projectile.owner, 0, 0.5f);
                                proj.timeLeft = (int)(45 / Projectile.scale);
                                proj.scale = Projectile.scale;
                                swingCount = 0;
                                attackPower = 6;
                            }
                            else if (redirected) // Redirect the bolt
                            {
                                Owner.SetScreenshake(4.5f);
                                SoundStyle fire = new("CalamityMod/Sounds/Item/SkytideBolt");
                                SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = -0.2f }, Projectile.Center);
                                SoundStyle fire2 = new("CalamityMod/Sounds/Item/AuricBulletHit");
                                SoundEngine.PlaySound(fire2 with { Volume = 0.5f, Pitch = 0.2f }, Projectile.Center);
                                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - aimVel * 2 * Projectile.scale, aimVel.SafeNormalize(Vector2.UnitX) * -10, ModContent.ProjectileType<DragoonBigBolt>(), (int)(Projectile.damage * 10), Projectile.knockBack, Projectile.owner, 0, 1f);
                                proj.scale = Projectile.scale;
                                swingCount = -1;
                                attackPower = 0;
                            }
                            else // Five projectile scattershot
                            {
                                for (int i = -2; i < 3; i++)
                                {
                                    float rot = (0.1f * attackPower * i / 6f);
                                    Vector2 vel = rot.ToRotationVector2().RotatedBy(aimVel.ToRotation()) * -7;
                                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center - aimVel * 2 * Projectile.scale, vel, ModContent.ProjectileType<DragoonSmallBolt>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0, rot);
                                    proj.scale = Projectile.scale;
                                }
                                attackPower--;
                            }
                            colorFadeIn = 1;
                            fireProj = false;
                        }
                    }
                    if (time >= (int)(timeMax * 0.85f))
                        CanHit = false;

                    spearOutset = MathHelper.Lerp(spearOutset, MathHelper.ToRadians(MathHelper.Lerp(450f, 0f, CalamityUtils.ExpInOutEasing(time / timeMax, 1))), 0.2f);

                    if (time >= timeMax)
                        doSwing = false;
                    if (time < (int)(timeMax * 0.7f))
                        postSwing = true;

                    if (CanHit)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Color color = Color.Lerp(Color.White, Main.rand.NextBool() ? color1 : color2, 0.65f);
                            float bonus = (1 - (7 / (attackPower + 1))) * 0.3f;
                            Vector2 pos = Owner.Center + (new Vector2(160 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.05f));
                            Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-45)).RotatedByRandom(0.5f) * Main.rand.NextFloat(6f, 10f);
                            Dust dust = Dust.NewDustPerfect(pos, Main.rand.NextBool(3) ? DustID.FireworksRGB : ModContent.DustType<SquashDust>(), vel * (1 + bonus), 0, default, Main.rand.NextFloat(1.05f, 1.4f));
                            dust.noGravity = dust.type != DustID.FireworksRGB;
                            dust.color = color;
                            dust.scale += bonus;
                            if (dust.type != DustID.FireworksRGB)
                            {
                                dust.scale *= Projectile.scale;
                                dust.fadeIn = Projectile.scale - 1;
                            }
                        }
                    }
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) // Custom collision since it's a spear
        {
            // Perform an AABB line collision check to check the whole spear.
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + ((Projectile.rotation - MathHelper.ToRadians(45)).ToRotationVector2() * HitboxOutset + tipOutset) * Projectile.scale, HitboxSize.X * Projectile.scale, ref _);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
                Projectile.numHits -= 1;

            SoundStyle fire = new("CalamityMod/Sounds/Item/AuricBulletHit");
            SoundEngine.PlaySound(fire with { Volume = 0.55f, Pitch = 0.8f }, Projectile.Center);
            SoundStyle fire2 = new("CalamityMod/Sounds/Custom/DefenseDamage");
            SoundEngine.PlaySound(fire2 with { Volume = 0.65f, Pitch = 0.4f }, Projectile.Center);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, 23, true, Owner);

            target.AddBuff(BuffID.Electrified, 230);
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
                Asset<Texture2D> tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/SkytideDragoonHoldout");
                Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/SkytideDragoonGlow");
                float fxRot = Projectile.rotation + MathHelper.ToRadians(45);
                float fxPosRot = Projectile.rotation - MathHelper.ToRadians(45);
                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;
                Vector2 drawPos = (Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY)) + Projectile.velocity.SafeNormalize(Vector2.UnitX);

                Color glowColor = Color.Lerp(color1, color2, colorFadeIn) with { A = 0 };
                Color auraColor = Color.Lerp(color2, color1, colorFadeIn) with { A = 0 } * 0.15f * fadeIn;
                for (int i = 0; i < 25; i++)
                {
                    // Outline effect
                    Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/SkytideDragoonGhost").Value;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * 7.2f * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, drawPos + drawOffset, centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(tex.Value, drawPos, tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                for (int i = 0; i < 2; i++)
                    Main.EntitySpriteDraw(glowTex.Value, drawPos, glowTex.Frame(1, FrameCount, 0, Frame), glowColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(glowTex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));

                Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/ArchSmear");
                Asset<Texture2D> orb = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 55.5f / MathHelper.Pi);
                for (int i = 0; i < 6; i++)
                {
                    // While not super visible here, this hitbox extention effect looks great and I'll use it on stuff like Terra Lance later
                    Color tipColor = Color.Lerp(color2, color1, (i + 4) / 6) with { A = 0 } * 0.3f * fadeIn;
                    Vector2 scale = new Vector2(0.25f - i * 0.04f, (1.5f + i * 0.15f) * colorFadeIn) * (0.75f * colorFadeIn * Main.rand.NextFloat(0.9f, 1.1f) + 0.25f) * Projectile.scale;
                    Main.EntitySpriteDraw(tex2.Value, Owner.Center - Main.screenPosition + fxPosRot.ToRotationVector2() * 130 * Projectile.scale, null, tipColor, fxRot, tex2.Size() * 0.5f, scale, SpriteEffects.None);
                }
                for (int i = 0; i < 6; i++)
                {
                    // the electric orb at the tip
                    Color orbColor = Color.Lerp(Color.Lerp(color2, color1, (i + 2) / 6), Color.White,  i / 6) with { A = 0 } * 0.5f;
                    Vector2 scale = new Vector2(Math.Abs(sine * 0.5f) + 0.1f, 1) * (0.05f + i * 0.01f) * attackMult * Main.rand.NextFloat(0.9f, 1.1f) * 2 * Projectile.scale;
                    Main.EntitySpriteDraw(orb.Value, Owner.Center - Main.screenPosition + fxPosRot.ToRotationVector2() * 188 * Projectile.scale + tipOutset * Projectile.scale, null, orbColor, Main.rand.NextFloat(-5, 5), orb.Size() * 0.5f, scale, SpriteEffects.None);
                }
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}
