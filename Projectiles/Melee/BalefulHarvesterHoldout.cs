using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BalefulHarvesterHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<BalefulHarvester>();
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<BalefulHarvester>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/BalefulHarvester";
        public int size = 106;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);

        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = false;
        public bool postSwing = false;
        public float fadeIn = 0f;
        public int useAnim;
        public bool FirstIFrameReset = false;
        public bool SecondIFrameReset = false;

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

            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX).X < 0 ? true : false;
        }

        public override void UseStyle()
        {
            AnimationProgress = Animation % useAnim;

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
                Projectile.ResetLocalNPCHitImmunity();
                Projectile.numHits = 0;
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65f;
                CanHit = false;
                FirstIFrameReset = false;
                SecondIFrameReset = false;
                if (mousePos.X < Owner.Center.X)
                    Owner.direction = -1;
                else
                    Owner.direction = 1;
                FlipAsSword = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0;
                doSwing = true;
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

                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) - MathHelper.ToRadians(45f * Owner.direction), 0.1f);

                if (AnimationProgress < (useAnim * 0.4f))
                {
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        doSwing = false;
                    }
                    RotationOffset = RotationOffset.AngleLerp(MathHelper.ToRadians(-30f * Owner.direction), 0.12f);
                }
                else
                {
                    FlipAsSword = Owner.direction < 0;

                    float time = AnimationProgress - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time % (int)(timeMax * 0.33f) == 0)
                    {
                        SoundStyle fire = new("CalamityMod/Sounds/Item/TerratomereSwing");
                        SoundEngine.PlaySound(fire with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.75f, 0.85f) }, Projectile.Center);
                        SoundStyle fire2 = new("CalamityMod/Sounds/Item/SwingMid");
                        SoundEngine.PlaySound(fire2 with { Volume = 0.6f, Pitch = Main.rand.NextFloat(0.25f, 0.35f) }, Projectile.Center);
                    }

                    CanHit = time < (int)(timeMax * 0.85f);

                    if (time > (int)(timeMax * 0.66f))
                    {
                        RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(765f * Owner.direction, 1125f * Owner.direction, CalamityUtils.ExpInOutEasing(time * 3f / timeMax, 1))), 0.1f * Owner.GetAttackSpeed<MeleeDamageClass>());
                        if (!SecondIFrameReset)
                        {
                            SecondIFrameReset = true;
                            Projectile.ResetLocalNPCHitImmunity();
                        }
                    }
                    else if (time > (int)(timeMax * 0.33f))
                    {
                        RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(405f * Owner.direction, 765f * Owner.direction, CalamityUtils.ExpInOutEasing(time * 3f / timeMax, 1))), 0.1f * Owner.GetAttackSpeed<MeleeDamageClass>());
                        if (!FirstIFrameReset)
                        {
                            FirstIFrameReset = true;
                            Projectile.ResetLocalNPCHitImmunity();
                        }
                    }
                    else
                        RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(45f * Owner.direction, 405f * Owner.direction, CalamityUtils.ExpInOutEasing(time * 3f / timeMax, 1))), 0.1f * Owner.GetAttackSpeed<MeleeDamageClass>());

                    if (time >= (int)(timeMax * 0.98f))
                        doSwing = false;
                    if (time < (int)(timeMax * 0.85f))
                        postSwing = true;
                }

                if (CanHit)
                {
                    Vector2 sparkVel = Vector2.UnitY.RotatedBy(FinalRotation + MathHelper.ToRadians(-45)) * (3.5f * Owner.direction);
                    Vector2 sparkPos = Owner.Center + Vector2.UnitX.RotatedBy(FinalRotation + MathHelper.ToRadians(-45)) * Main.rand.Next(80, 140) * Projectile.scale;
                    CustomSprite critSpark = new(sparkPos, sparkVel, 15, "CalamityMod/Particles/CritSpark", 1f * Projectile.scale, Color.Orange, frameCount: 4, frame: Main.rand.Next(4));
                    GeneralParticleHandler.SpawnParticle(critSpark);
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
                Projectile.numHits -= 1;

            // Whoa on-hit effects !!!
            for (int i = 0; i < 4; i++)
            {
                Vector2 sparkVel = Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(4.5f, 6.5f);
                CustomSprite critSpark = new(target.Center, sparkVel, 40, "CalamityMod/Particles/CritSpark", 1f, new Color(255, 85, 0), frameCount: 4, frame: Main.rand.Next(4));
                GeneralParticleHandler.SpawnParticle(critSpark);
            }
            DesertProwlerSkullParticle skullEffect = new(target.Center, Vector2.Zero, Color.OrangeRed, Color.Black, 1f, 180f);
            GeneralParticleHandler.SpawnParticle(skullEffect);

            // Debuff and actually spawning the projectiles
            target.AddBuff(BuffID.OnFire3, 300);
            CalamityPlayer.HorsemansBladeOnHit(Owner, target.whoAmI, Projectile.damage, Projectile.knockBack * 0.5f, 0, ModContent.ProjectileType<BalefulHarvesterProjectile>());
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
            // Draw the actual sword
            if (useAnim > 0 && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;

                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, lightColor, FinalRotation + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
            }

            // Draw a swing smear and sparkle while swinging
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            float time = AnimationProgress - (useAnim / 3);
            float timeMax = useAnim - (useAnim / 3);
            if (time > (int)(timeMax * 0.1f) && doSwing)
            {
                Asset<Texture2D> vanillaSmear = TextureAssets.Projectile[997];
                Rectangle vanillaSmearFrame = vanillaSmear.Frame(1, 4, 0, 0);
                Asset<Texture2D> smear = ModContent.Request<Texture2D>("CalamityMod/Particles/SemiCircularSmearSwipe");
                float smearOpacity = CalamityUtils.Convert01To010(time / timeMax);
                Main.EntitySpriteDraw(vanillaSmear.Value, Projectile.Center - Main.screenPosition, vanillaSmearFrame, new Color(193, 83, 43) * smearOpacity * 0.75f, FinalRotation - MathHelper.PiOver2, vanillaSmearFrame.Size() / 2f, 1.25f * Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(smear.Value, Projectile.Center - Main.screenPosition, null, new Color(247, 115, 0) * smearOpacity, FinalRotation, smear.Size() / 2f, 1.8f * Projectile.scale, SpriteEffects.None);

                if (smearOpacity > 0.65f)
                {
                    Vector2 sparklePos = Projectile.Center - Main.screenPosition - (Vector2.UnitY.RotatedBy(FinalRotation + MathHelper.PiOver4 * Owner.direction) * 120f);
                    Asset<Texture2D> bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                    Main.EntitySpriteDraw(bloom.Value, sparklePos, null, Color.White * 0.75f, 0f, bloom.Size() / 2f, 0.2f * Projectile.scale, SpriteEffects.None);
                    Asset<Texture2D> sparkle = ModContent.Request<Texture2D>("CalamityMod/Particles/FullStar");
                    Main.EntitySpriteDraw(sparkle.Value, sparklePos, null, Color.Orange, 0f, sparkle.Size() / 2f, 2f * Projectile.scale, SpriteEffects.None);
                }
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }

        public override void ResetStyle()
        {
        }
    }
}
