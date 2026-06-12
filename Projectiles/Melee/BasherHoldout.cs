using System;
using System.Collections.Generic;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;

namespace CalamityMod.Projectiles.Melee
{
    public class BasherHoldout : ModProjectile, ILocalizedModType
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Basher>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Basher";
        public ref float attackTimer => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public int fireRate => (int)((Owner.HeldItem.useAnimation * Owner.inverseMeleeSpeed * Projectile.MaxUpdates) / 2.75f);
        public int time = 0;
        public Vector2 toMouse = Vector2.Zero;
        public int cooldown => (int)(-Owner.HeldItem.useAnimation * Owner.inverseMeleeSpeed * Projectile.MaxUpdates);
        public float bladeRot = 0; // Rotation of the blade
        public int swingCount = 0; // Increases at the start of a swing
        public float bladefx = 0; // Visual effects of the blade
        // Checks to make sure sounds only happen once per swing
        public bool makeSound = true;
        public bool performSwing = false;
        public float enabledHitbox = 0; // Deals damage when above 0, ticks down quickly
        public float angledHold = 0; // Adds some angle based on swing direction
        public float rumble = 0;
        public override bool? CanDamage() => enabledHitbox > 0 ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float outset = 15 * Projectile.scale;
            Vector2 hitboxPosition = Owner.Center + toMouse * outset;
            float hitboxSize = 70 * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(hitboxPosition, hitboxSize, targetHitbox);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.scale = 0;
        }
        public void OnSpawn()
        {
            if (time == 0)
            {
                Projectile.knockBack = 0;
                attackTimer = cooldown;
                swingCount = 1;
            }
        }
        public void ResetVariables() // After a swing, reset/adjust many stats
        {
            for (int i = 0; i < Main.maxNPCs; i++)
                Projectile.localNPCImmunity[i] = 0;
            Projectile.numHits = 0;
            swingCount++;
            makeSound = true;
            enabledHitbox = 0;
            Projectile.ForceNetUpdate();
        }
        public void Positioning(Vector2 toMouse) // Hand and holdout positioning
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = (Projectile.velocity * Owner.direction).ToRotation();

            float angleVariation = (float)Math.Sin(time * 0.06f / Projectile.MaxUpdates) * 0.1f;

            float outwardDistance = 5;
            Owner.ChangeDir(Math.Sign(toMouse.X));
            Owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, (toMouse.ToRotation() + (bladeRot + angleVariation) * Owner.direction + MathHelper.PiOver2 * -Owner.direction) + (Owner.direction == -1 ? MathHelper.Pi : 0));
            Vector2 handPos = Owner.GetFrontHandPosition(CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation) + (Owner.compositeFrontArm.rotation + MathHelper.PiOver2).ToRotationVector2() * outwardDistance;
            float armRotation = (Utils.DirectionTo(Owner.Center, handPos).ToRotation() - MathHelper.PiOver2) * Owner.gravDir + (Owner.gravDir == -1 ? MathHelper.Pi : 0f);
            Owner.SetCompositeArmBack(true, CompositeArmStretchAmount.Full, armRotation);

            Projectile.velocity = toMouse.RotatedBy((bladeRot + angleVariation + angledHold) * Owner.direction);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = handPos;
        }
        public override void AI()
        {
            if (!Owner.CantUseHoldout(false))
                Projectile.timeLeft = 5;

            Projectile.scale = MathHelper.Lerp(Projectile.scale, Owner.GetMeleeScale() + 0.1f, 0.3f / Projectile.MaxUpdates);
            toMouse = Utils.DirectionTo(Owner.Center, Owner.ClampedMouseWorld());

            Positioning(toMouse);

            OnSpawn();

            if (enabledHitbox > 0)
            {
                bladefx = MathHelper.Lerp(bladefx, 1, 0.5f / Projectile.MaxUpdates * Owner.meleeSpeed);
                if (Projectile.FinalExtraUpdate())
                    enabledHitbox--;
            }
            else
                bladefx = MathHelper.Lerp(bladefx, 0, 0.55f / Projectile.MaxUpdates * Owner.meleeSpeed);

            if (attackTimer == -1 || time < 25)
            {
                if (Owner.HeldItem.type != ItemType<Basher>() || Main.mapFullscreen || Owner.mouseInterface || Owner.dead)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
                Owner.itemTime = Owner.itemAnimation = 5;

            #region Not Swinging
            if (attackTimer < 0) // When the sword isn't swinging
            {
                Timers();

                float lerp = Utils.GetLerpValue(cooldown, -1, attackTimer, true);

                float lerpSpeed = 0.2f / Projectile.MaxUpdates * Owner.meleeSpeed;
                float maxAngle = MathHelper.PiOver4;
                if (swingCount % 2 == 0)
                    angledHold = MathHelper.Lerp(angledHold, maxAngle, lerpSpeed);
                else
                    angledHold = MathHelper.Lerp(angledHold, -maxAngle, lerpSpeed);

                if (attackTimer == 0) // Reset a few things in prep for the next swing
                    ResetVariables();
                else // Wind up animation in prep for the next swing
                {
                    int swingDir = (swingCount % 2 != 0 ? 1 : -1);
                    float lerp2 = Utils.GetLerpValue(cooldown, -1, attackTimer, true);
                    float start = -MathHelper.Pi * swingDir;
                    float end = -MathHelper.PiOver4 * 3 * swingDir;
                    bladeRot = MathHelper.Lerp(start, end, CalamityUtils.EaseInOutExp(lerp2, 2f, 2f));

                    if (Owner.controlUseItem && lerp > 0.9f)
                        performSwing = true;
                }
                return;
            }
            #endregion

            if (attackTimer >= fireRate) // Once the swing is done, put it on cooldown
            {
                attackTimer = cooldown;
                performSwing = false;
            }
            else // Do the swing
            {
                #region Swinging
                int swingDir = (swingCount % 2 != 0 ? -1 : 1);
                float lerp = Utils.GetLerpValue(0, fireRate - 1, attackTimer, true);
                float start = -MathHelper.PiOver4 * 3 * swingDir;
                float end = MathHelper.Pi * swingDir;
                bladeRot = MathHelper.Lerp(start, end, CalamityUtils.EaseInOutExp(lerp, 5f, 3f));

                if (lerp > 0.3f && lerp < 0.8f)
                {
                    Vector2 dustPosition = Projectile.Center + Projectile.velocity * 78 * Projectile.scale;
                    Vector2 swingVel = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * (swingCount % 2 == 0 ? -1 : 1) * Owner.direction);
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Pearlwood);
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.85f, 1.3f);
                    dust.velocity = swingVel.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f, 2.5f);

                    bool fall = Main.rand.NextBool(8);
                    Dust dust2 = Dust.NewDustPerfect(dustPosition, ModContent.DustType<SquashDustPixelated>(), swingVel * Main.rand.NextFloat(3.5f, 5.5f));
                    dust2.scale = Main.rand.NextFloat(0.65f, 0.85f);
                    dust2.noGravity = fall ? false : true;
                    dust2.color = Color.Chartreuse;
                    if (!fall)
                        dust2.fadeIn = 0.9f;
                    else
                    {
                        dust.velocity.Y = 1.5f;
                        dust2.fadeIn = -0.5f;
                    }
                    
                }

                if (lerp > 0.4f) // Partway through make the hitbox and sound
                {
                    if (makeSound)
                    {
                        enabledHitbox = 8;
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.75f, Pitch = Main.rand.NextFloat(-0.35f, -0.55f) }, Projectile.Center);
                        makeSound = false;
                    }
                }
                #endregion
            }
            Timers();
        }
        public void Timers()
        {
            time++;
            // Only increment the attack animation when on cooldown or when allowed to swing
            if ((attackTimer < fireRate && performSwing) || attackTimer < -1)
                attackTimer++;
            if (rumble > 0)
                rumble -= 0.2f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits == 0)
            {
                Owner.SetScreenshake(2f);

                SoundStyle bash1 = new("CalamityMod/Sounds/Item/DampExplosion");
                SoundEngine.PlaySound(bash1 with { Volume = 0.45f, Pitch = 0.7f }, Projectile.Center);
                SoundStyle bash2 = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 3);
                SoundEngine.PlaySound(bash2 with { Volume = 0.75f, Pitch = -0.2f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DrumTomHigh with { Volume = 0.55f, Pitch = -0.9f }, Projectile.Center);

                Vector2 playerLaunchVel = -toMouse * 10;
                Owner.velocity = playerLaunchVel;
                rumble = 20;
            }

            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, 14f, true, Owner);

            for (int i = 0; i < MathHelper.Clamp(15 - Projectile.numHits * 3, 2, 15); i++)
            {
                bool dType = i % 4 == 0;
                Dust dust = Dust.NewDustPerfect(target.Center, !dType ? DustID.Pearlwood : ModContent.DustType<SquashDustPixelated>(), launchVel.RotatedByRandom(dType ? 0.2f : 0.5f) * Main.rand.NextFloat(2.5f, 7f) * (dType ? 3 : 1));
                dust.scale = Main.rand.NextFloat(0.65f, 1.05f) * ((dType || Main.rand.NextBool(3)) ? 2.2f : 1f);
                dust.noGravity = true;
                dust.color = dType ? Color.Chartreuse : default;
                if (dType)
                    dust.fadeIn = 1.7f;
            }

            float minMult = 0.3f;
            int hitsToMinMult = 5;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 3)
                return false;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearThin").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
            Vector2 vel = Projectile.rotation.ToRotationVector2();
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + ((Owner.direction == -1) ? MathHelper.Pi - MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 rotationPoint = (Owner.direction == -1) ? new Vector2(texture.Width, texture.Height) : new Vector2(0, texture.Height);
            SpriteEffects flipSprite = (Owner.direction == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 placeAdjust = (drawRotation + MathHelper.PiOver2).ToRotationVector2() * 2.5f + ((swingCount % 2 == 0 ? new Vector2(-6, 0) : new Vector2(-2, 0)).RotatedBy(drawRotation) * Owner.direction);

            Color clr = Color.Tan;
            int swingCountFlip = (swingCount % 2 == 0 ? 1 : 3) + Owner.direction == 0 ? 2 : 0;
            Vector2 vel2 = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            float extraRot = MathHelper.PiOver4 * 1.2f * (swingCount % 2 == 0 ? (Owner.direction == -1 ? 1 : -1) : (Owner.direction == -1 ? -1 : 1));
            float swooshDrawRotation = vel2.ToRotation() + extraRot;
            SpriteEffects swooshFlipSprite = (swingCount % 2 == 0 ? (Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None) : (Owner.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));

            Main.EntitySpriteDraw(swoosh, drawPosition + Projectile.velocity.RotatedBy(extraRot) * 12.5f, null, clr with { A = 0 } * bladefx * 0.4f, swooshDrawRotation, swoosh.Size() * 0.5f, Projectile.scale * 0.5f, swooshFlipSprite); //flipSprite | (swingCount % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally)

            Main.EntitySpriteDraw(texture, drawPosition + placeAdjust + Main.rand.NextVector2Circular(rumble, rumble), null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite); // The holdout
            
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
