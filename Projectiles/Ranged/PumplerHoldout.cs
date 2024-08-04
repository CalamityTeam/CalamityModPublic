using System;
using System.IO;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

//TY dom for coding condemnation, great reference, would steal code from again :)
//                  - Iban

namespace CalamityMod.Projectiles.Ranged
{
    public class PumplerHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Pumpler>();

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float PumpkinsCharge => ref Projectile.ai[1];
        
        private ref float FramesToLoadNextPumpkin => ref Projectile.localAI[0];

        //This possibly could moved to ai[2] ¯\_(ツ)_/¯
        private ref float Overfilled => ref Projectile.localAI[1]; //This functionally is a "IsPlayingShootAnim" variable

        private bool IsOverfilled => (Overfilled >= 0.5f);
        private float angularSpread = MathHelper.ToRadians(15);

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;

            // Unloads all pumpkins if you can't shoot/stop holding out the projectile or if the gun is overfilled
            if (Owner.CantUseHoldout() || IsOverfilled)
            {

                if (PumpkinsCharge <= 0f && !IsOverfilled) //If the projectile isnt playing its animation and if there arent any pumpkins loaded, kill it
                {
                    Projectile.Kill();
                    return;
                }

                //Animation stuff start
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 10) //10 fps anim
                {
                    Projectile.frame++;
                    if (Projectile.frame >= Main.projFrames[Projectile.type] - 1)
                        Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                }
                // Shoot anim is done? Remove the shoot anim tag
                if (Projectile.frame == 0 && IsOverfilled)
                    Overfilled = 0f;
                //Animation stuff end

                // Fire the spread of pumpkins & empty the barrel
                if (PumpkinsCharge >= 1)
                    UnloadChamber(tipPosition);
            }

            else
            {
                // Frame 1 effects: Initialize the shoot speed
                if (FramesToLoadNextPumpkin == 0f)
                {
                    FramesToLoadNextPumpkin = Owner.ActiveItem().useAnimation;
                }
                // Actually make progress towards loading more arrows. Also, make smoke come out eventually perhaps
                ++CurrentChargingFrames;
                // If it is time to load an pumpkin, produce a pulse of dust and add a pumpkin
                // Also accelerate charging, because it's fucking awesome. <- Agreeing with dom so hard i made it increase harder
                if (CurrentChargingFrames >= FramesToLoadNextPumpkin && PumpkinsCharge < Pumpler.MaxPumpkins)
                {
                    Projectile.frame = (Projectile.frame + 1); //next anim frame
                    CurrentChargingFrames = 0f;
                    ++PumpkinsCharge;
                    FramesToLoadNextPumpkin *= 0.850f;
                    // Debug text:
                    // CombatText.NewText(Projectile.Hitbox, new Color(255f * (PumpkinsCharge / 5f) / 255f, 151f * (PumpkinsCharge / 5f) / 255f, 78f * (PumpkinsCharge / 5f) / 255f), ((int)PumpkinsCharge).ToString(), true);
                    if (PumpkinsCharge >= Pumpler.MaxPumpkins)
                        SoundEngine.PlaySound(SoundID.Item69);
                    else
                    {
                        var loadSound = SoundEngine.PlaySound(SoundID.Item108 with { Volume = SoundID.Item108.Volume * 0.3f });
                    }
                }
                //If the pumpkins have been charging for too long, unload that mf!!!
                if (CurrentChargingFrames >= 160)
                {
                    Overfilled = 1f;
                }
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        public void SmokeBurst(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2 * PumpkinsCharge; i++)
            {
                Particle smoke = new SmallSmokeParticle(tipPosition + Main.rand.NextVector2Circular(10f, 10f), Vector2.Zero, Color.Orange, new Color(40, 40, 40), Main.rand.NextFloat(0.3f, 0.8f), 145 - Main.rand.Next(50));
                smoke.Velocity = (smoke.Position - Owner.Center) * 0.3f + Owner.velocity;
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public void UnloadChamber(Vector2 tipPosition)
        {
            //Spread calculation doesn't work with only one pumpkin loaded
            if (PumpkinsCharge == 1)
                ShootProjectiles(tipPosition, 0);
            else
            {
                for (int i = 0; i < PumpkinsCharge; i++)
                {
                    float spreadForThisProjectile = MathHelper.Lerp(-angularSpread, angularSpread, i / (float)(PumpkinsCharge - 1f));
                    ShootProjectiles(tipPosition, spreadForThisProjectile);

                }
            }
            SoundEngine.PlaySound(SoundID.Item96);//play the sound
            SmokeBurst(tipPosition);
            FramesToLoadNextPumpkin = Owner.ActiveItem().useAnimation; //reset the reload time
            PumpkinsCharge = 0; //Unload the barrel
            if (!IsOverfilled)
                Overfilled = 1f; //Make the shoot anim play
            Projectile.frame = 6; //Initialize the animation
        }

        public void ShootProjectiles(Vector2 tipPosition, float projectileRotation)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            int projectileType = ModContent.ProjectileType<PumplerGrenade>();
            int PumpkinDamage = heldItem is null ? 0 : Owner.GetWeaponDamage(heldItem);
            float shootSpeed = heldItem.shootSpeed * 1.5f;
            float knockback = Owner.GetWeaponKnockback(heldItem, heldItem.knockBack);
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(projectileRotation) * shootSpeed;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, projectileType, PumpkinDamage, knockback, Projectile.owner, 0f, 0f);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Owner.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            //Please someone kill me i dont want to deal with offset moments,
            //Dom oomfie!
            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 30f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 35f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the gun will have 1 frame of rotational "jitter" when the direction changes based on the
            // original angle. This effect looks very strange in-game.
            if (Projectile.spriteDirection != oldDirection)
                Projectile.rotation -= MathHelper.Pi;



            Projectile.timeLeft = 2;
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            if (PumpkinsCharge > 0 && !IsOverfilled)
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(1f - 0.20f * CurrentChargingFrames - 0.1f * (5f - PumpkinsCharge), 0f, 1f));
                //tint effect is visible if its charging. The more pumpkins are loaded, the more opacity
            }
            else
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0f);
            }
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1f - 0.04f * (PumpkinsCharge - 1), 0.8f, 0.5f));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frameRectangle = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Frame(1, 9, 0, Projectile.frame);
            Main.EntitySpriteDraw(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value, drawPosition, frameRectangle, lightColor, Projectile.rotation, frameRectangle.Size() * 0.5f, 1f, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public override bool? CanDamage() => false;


        //netcode hell
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(IsOverfilled);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Overfilled = reader.ReadBoolean() ? 1.0f : 0.0f;
        }
    }
}



