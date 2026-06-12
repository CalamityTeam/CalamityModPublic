using System;
using System.Collections.Generic;
using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.Player;


namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AugerHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public ref float attackTimer => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public float scaleFx = 1;
        public int fireRate => Owner.itemAnimationMax;
        public int time = 0;
        public bool longSwing => swingCount >= 2;
        public int cooldown => -Owner.itemAnimationMax * (Owner.Calamity().buffedAuger ? 2 : longSwing ? 3 : 1);
        public float bladeRot = 0; // Rotation of the blade
        public int swingCount = 0; // Increases at the start of a swing
        public float bladefx = 0; // Scaling on the effects of the blade
        // Both checks to make sure sounds/hitboxes only happen once per swing
        public bool makeSound = true;
        public bool makeHitbox = true;
        public int cooldownGiven => (int)(800 / Owner.GetAttackSpeed<MeleeDamageClass>()); // Cooldown is effected by melee speed, lines up perfectly so you do 22 swings and end in time for a big swing
        public float pullFx = 1; // Scaling on the effects of the blade when it's buffed
        public bool pressedRight = false;
        // Spawns a hitbox to do damage, rather than dealing damage itself
        // Theres many reasons this is better, but at the very least it prevents bloating this single projectile's code
        public override bool? CanDamage() => false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public void Positioning(Vector2 toMouse) // Hand and holdout positioning
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Owner.direction).ToRotation();

            Owner.ChangeDir(Math.Sign(toMouse.X));
            Owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, (toMouse.ToRotation() + bladeRot * Owner.direction + MathHelper.PiOver2 * -Owner.direction) + (Owner.direction == -1 ? MathHelper.Pi : 0));
            Vector2 handPos = Owner.GetFrontHandPosition(CompositeArmStretchAmount.None, Owner.compositeFrontArm.rotation) + (Owner.compositeFrontArm.rotation + MathHelper.PiOver2).ToRotationVector2() * 9;
            float armRotation = (Utils.DirectionTo(Owner.Center, handPos).ToRotation() - MathHelper.PiOver2) * Owner.gravDir + (Owner.gravDir == -1 ? MathHelper.Pi : 0f);
            Owner.SetCompositeArmBack(true, CompositeArmStretchAmount.Full, armRotation);
            Projectile.velocity = toMouse.RotatedBy(bladeRot * Owner.direction);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = handPos;
        }
        public void OnSpawn()
        {
            if (time == 0)
            {
                scaleFx = Owner.Calamity().buffedAuger ? 1.5f : 1;
                attackTimer = cooldown;
                if (Owner.Calamity().buffedAuger)
                {
                    swingCount = 1;
                }
            }
        }
        public override void AI()
        {
            if (!Owner.CantUseHoldout(false))
                Projectile.timeLeft = 5;

            Projectile.scale = MathHelper.Lerp(Projectile.scale, Owner.GetMeleeScale(), 0.3f / Projectile.MaxUpdates);

            Vector2 toMouse = Utils.DirectionTo(Owner.Center, Owner.ClampedMouseWorld());

            Positioning(toMouse);

            if (Owner.Calamity().mouseRight)
                pressedRight = true;

            #region Right Click
            if (Projectile.ai[2] > 0) // Right click "gravity well" and buff
            {
                if (Projectile.ai[2] == 5)
                {
                    time = 0;
                    attackTimer = 0;
                    swingCount = -1;
                    makeHitbox = true;
                    Projectile.ai[2]--;
                    pullFx = 5;
                }
                if (bladefx > 0)
                    bladefx = 0;
                bladeRot = 0;
                pullFx = MathHelper.Lerp(pullFx, 1, 0.12f);

                if (attackTimer > fireRate && makeHitbox)
                {
                    Projectile magnetZone = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.ClampedMouseWorld(), Vector2.Zero, ModContent.ProjectileType<AugerPull>(), 0, 0, Projectile.owner, Projectile.scale, 0, 0);

                    SoundStyle pull = new("CalamityMod/Sounds/Item/AugerPull");
                    SoundEngine.PlaySound(pull with { Volume = 0.9f, Pitch = 0 }, Owner.ClampedMouseWorld());
                    Owner.Calamity().buffedAuger = true;
                    Owner.Calamity().arsenalCooldown = cooldownGiven;
                    Owner.AddCooldown(ArsenalPower.ID, cooldownGiven);
                    makeHitbox = false;
                }
                if (attackTimer >= fireRate * 2)
                {
                    Projectile.Kill();
                    return;
                }
                time++;
                attackTimer++;
                return;
            }
            #endregion

            OnSpawn();

            if (scaleFx > 1 && !Owner.Calamity().buffedAuger) // Fade out the extra size from the big blade
                scaleFx = MathHelper.Lerp(scaleFx, 1, 0.15f * Owner.GetAttackSpeed<MeleeDamageClass>());

            #region Not Swinging
            if (attackTimer < 0) // When the sword isn't swinging
            {
                if ((pressedRight || Owner.Calamity().mouseRight) && !Owner.Calamity().buffedAuger && Owner.Calamity().arsenalCooldown <= 0)
                    Projectile.ai[2] = 5;
                else
                    pressedRight = false;
                if (longSwing && attackTimer == -1)
                {
                    swingCount = 0;
                }
                attackTimer++;
                time++;

                if (longSwing && attackTimer <= cooldown * 0.35f)
                {
                    float lerp = Utils.GetLerpValue(cooldown, -1, attackTimer, true);
                    bladefx = MathHelper.Lerp(bladefx, 0, lerp);
                }
                else
                {
                    if (!Main.mouseLeft && longSwing && !Owner.Calamity().buffedAuger)
                    {
                        Projectile.Kill();
                        return;
                    }

                    float lerp = Utils.GetLerpValue((longSwing ? cooldown * 0.35f : cooldown), -1, attackTimer, true);
                    bladefx = MathHelper.Lerp(bladefx, 1.5f, lerp);
                }
                if (attackTimer >= cooldown * 0.3f)
                {
                    if (makeSound && Owner.Calamity().buffedAuger) // Windup Sound
                    {
                        SoundStyle windup = new("CalamityMod/Sounds/Item/AugerWindup");
                        SoundEngine.PlaySound(windup with { Volume = 0.9f, Pitch = 0 }, Projectile.Center);
                        makeSound = false;
                    }
                }
                if (attackTimer == 0) // Reset a few things in prep for the next swing
                {
                    pressedRight = false;
                    swingCount++;
                    makeSound = true;
                    makeHitbox = true;
                }
                else // Wind up animation in prep for the next swing
                {
                    int swingDir = (swingCount % 2 != 0 ? 1 : -1);
                    float lerp = Utils.GetLerpValue(cooldown, -1, attackTimer, true);
                    bladeRot = MathHelper.Lerp(-MathHelper.Pi * swingDir, -MathHelper.PiOver4 * 3 * swingDir, CalamityUtils.EaseInOutExp(lerp, 2f, 2f));
                }
                return;
            }
            #endregion

            if (attackTimer >= fireRate) // Once the swing is done, put it on cooldown
            {
                attackTimer = cooldown * (Owner.Calamity().buffedAuger ? 3 : 1);
                if (Owner.Calamity().buffedAuger)
                {
                    Owner.Calamity().buffedAuger = false;
                }
            }
            else // Do the swing
            {
                #region Swinging
                int swingDir = (swingCount % 2 != 0 ? -1 : 1);
                float lerp = Utils.GetLerpValue(0, fireRate - 1, attackTimer, true);
                bladeRot = MathHelper.Lerp(-MathHelper.PiOver4 * 3 * swingDir, MathHelper.Pi * swingDir, CalamityUtils.EaseInOutExp(lerp, 5f, 3f));

                if (lerp > 0.5f) // Halfway through make the hitbox and sound
                {
                    if (makeHitbox)
                    {
                        SoundStyle swing = Owner.Calamity().buffedAuger ? new("CalamityMod/Sounds/Item/AugerBigSlash") : swingCount % 2 != 0 ? new("CalamityMod/Sounds/Item/AugerSlash1") : new("CalamityMod/Sounds/Item/AugerSlash2");
                        SoundEngine.PlaySound(swing with { Volume = Owner.Calamity().buffedAuger ? 1f : 0.7f, Pitch = -0.1f, MaxInstances = 2 }, Projectile.Center);

                        int damage = Projectile.damage;
                        if (Owner.Calamity().buffedAuger)
                        {
                            Owner.SetScreenshake(5f);
                            damage = (int)(damage * 2.5f);
                        }
                        Projectile slash = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center + toMouse * 20 * (float)Math.Pow(scaleFx, 4), toMouse * 25, ModContent.ProjectileType<AugerSlash>(), damage, 0, Projectile.owner, 0, swingCount, Owner.Calamity().buffedAuger ? 5 : 0);
                        slash.localAI[0] = Projectile.scale;
                        makeHitbox = false;
                    }
                }
                #endregion
            }
            time++;
            attackTimer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 3)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/AugerHoldout").Value;
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/AugerHoldoutGlow").Value;
            Texture2D blade = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowBlade").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
            Vector2 vel = Projectile.rotation.ToRotationVector2();
            float randSize = Main.rand.NextFloat(0.8f, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Owner.direction == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float lerp = Utils.GetLerpValue(0, fireRate - 1, attackTimer, true);
            float length = MathHelper.Lerp(0f, 0.1f, CalamityUtils.EaseInOutExp(lerp, 1f, 1f));
            float bladeRotation = drawRotation + MathHelper.PiOver2 * Owner.direction;
            Vector2 bladePlaceAdjust = (bladeRotation - MathHelper.PiOver2).ToRotationVector2();
            for (int i = 0; i < 2; i++) // The actual energy blade
                Main.EntitySpriteDraw(blade, drawPosition + bladePlaceAdjust * 20, null, (i == 0 ? Effects.ArsenalEffects.ArsenalGaussColor : Color.White) with { A = 0 }, bladeRotation, new Vector2(blade.Width / 2, blade.Height), new Vector2((i == 0 ? 0.2f : 0.1f) * randSize, 0.2f * bladefx) * Projectile.scale * 0.07f * bladefx * scaleFx, SpriteEffects.None);
            for (int i = 0; i < 2; i++) // The glow at the "hilt"
                Main.EntitySpriteDraw(bloom, drawPosition + bladePlaceAdjust * 20, null, (i == 0 ? Effects.ArsenalEffects.ArsenalGaussColor : Color.White) with { A = 0 }, bladeRotation, bloom.Size() / 2, (Projectile.ai[2] > 0 ? new Vector2(randSize, randSize * (i == 0 ? 1f : 0.9f)) * 0.1f : new Vector2(0.15f * bladefx * randSize, (i == 0 ? 0.1f : 0.05f))) * Projectile.scale * 0.9f * (Projectile.ai[2] > 0 ? pullFx : bladefx) * scaleFx, SpriteEffects.None);

            if (scaleFx > 1) // Extra blade tips for the improved slash
            {
                for (int b = -2; b <= 2; b++)
                {
                    if (b == 0) b++;
                    for (int i = 0; i < 2; i++)
                    {
                        Main.EntitySpriteDraw(blade, drawPosition + bladePlaceAdjust * 20, null, (i == 0 ? Effects.ArsenalEffects.ArsenalGaussColor : Color.White) with { A = 0 }, bladeRotation + (MathHelper.PiOver4 * 0.55f * b), new Vector2(blade.Width / 2, blade.Height), new Vector2(0.65f * (i == 0 ? 0.1f : 0.05f), 0.09f * (i == 0 ? 1f : 0.9f) * (2 - Math.Abs(b * 0.5f))) * randSize * Projectile.scale * 0.2f * bladefx * (scaleFx - 1), SpriteEffects.None);
                    }
                }
            }
            Vector2 placeAdjust = (drawRotation + MathHelper.PiOver2).ToRotationVector2() * 2.5f;
            // Spesifically doesn't scale with melee size, but the actual energy blade does
            Main.EntitySpriteDraw(texture, drawPosition + placeAdjust, null, drawColor, drawRotation, rotationPoint, 1, flipSprite); // The holdout

            Main.EntitySpriteDraw(glow, drawPosition + placeAdjust, null, Color.White, drawRotation, rotationPoint, 1, flipSprite); // The glow

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
