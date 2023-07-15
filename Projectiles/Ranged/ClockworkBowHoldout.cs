using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.Ranged;
using Terraria.Audio;


namespace CalamityMod.Projectiles.Ranged
{
    public class ClockworkBowHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetStaticDefaults()
        {
            //Main.projFrames[projectile.type] = 9;    might animate the bow's string getting drawn but not rn
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float LoadedBolts => ref Projectile.ai[1];
        private ref float FramesToLoadBolt => ref Projectile.localAI[0];
        private ref float LastDirection => ref Projectile.localAI[1];
        
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        private float storedVelocity = 1f;

        //private ref float Overfilled => ref projectile.localAI[1]; Until i implement the bow animation there is no need for that
        private float angularSpread = MathHelper.ToRadians(16);

        //public override string Texture => "CalamityMod/Projectiles/Ranged/ClockworkBowHoldout";
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/ClockworkBow";

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 96;
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

            // If the player releases left click, shoot out the arrows
            if (!OwnerCanShoot)
            {

                if (LoadedBolts <= 0f) //If theres no arrows to shoot
                {
                    Projectile.Kill();
                    return;
                }
                // Fire the spread of arrows
                else
                    UnloadBolts(tipPosition);
            }

            else
            {
                // Frame 1 effects: Initialize the shoot speed
                if (FramesToLoadBolt == 0f)
                {
                    FramesToLoadBolt = Owner.ActiveItem().useAnimation;
                }

                if (Owner.HasAmmo(Owner.ActiveItem()))
                {
                    ++CurrentChargingFrames;

                    if (CurrentChargingFrames - FramesToLoadBolt / 2 <= 0.01)
                        SoundEngine.PlaySound(SoundID.Item17); //Click

                    if (CurrentChargingFrames >= FramesToLoadBolt && LoadedBolts < ClockworkBow.MaxBolts)
                    {
                        // Save the stats here for later
                        Item heldItem = Owner.ActiveItem();
                        Owner.PickAmmo(heldItem, out _, out float shootSpeed, out int damage, out float knockback, out _);
                        Projectile.damage = damage;
                        Projectile.knockBack = knockback;
                        storedVelocity = shootSpeed;

                        CurrentChargingFrames = 0f;
                        ++LoadedBolts;

                        if (LoadedBolts % 2 == 0)
                            CombatText.NewText(Owner.Hitbox, new Color(155, 255, 255), CalamityUtils.GetTextValue("Misc.ClockworkTock"), true);
                        else
                            CombatText.NewText(Owner.Hitbox, new Color(255, 200, 100), CalamityUtils.GetTextValue("Misc.ClockworkTick"), true);

                        FramesToLoadBolt *= 0.950f;

                        if (LoadedBolts >= ClockworkBow.MaxBolts)
                            SoundEngine.PlaySound(SoundID.Item23);
                        else
                        {
                            SoundEngine.PlaySound(SoundID.Item108 with { Volume = SoundID.Item108.Volume * 0.3f });
                        }
                    }
                }
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        public void UnloadBolts(Vector2 tipPosition)
        {
            //If the max amount of bolts is already loaded we can ignore the extra spread from the bolt currently loading
            if (LoadedBolts == ClockworkBow.MaxBolts)
            {
                for (int i = 0; i < LoadedBolts; i++)
                {
                    //Version that doesnt inclue the chargingframes bit , since its fully charged
                    float increment = angularSpread * (LoadedBolts - 1) / 2;
                    float spreadForThisProjectile = MathHelper.Lerp(-increment, increment, i / (float)(LoadedBolts - 1));
                    ShootProjectiles(tipPosition, spreadForThisProjectile);

                }
                SoundEngine.PlaySound(SoundID.Item38);//play the sound
            }
            else if (LoadedBolts != 0)
            {
                for (int i = 0; i < LoadedBolts + 1; i++)
                {
                    if (i == LoadedBolts) //We don't actually shoot the arrow that's currently loading
                        continue;

                    //Version that takes into account the progress of the arrow currently loading. It takes half the time it takes for the arrow to load for the range to adjust to its next position
                    float increment = angularSpread * (LoadedBolts - 1 + MathHelper.Clamp((CurrentChargingFrames / FramesToLoadBolt) * 2, 0f, 1f)) / 2;
                    float spreadForThisProjectile = MathHelper.Lerp(-increment, increment, i / (float)(MathHelper.Lerp(LoadedBolts - 1, LoadedBolts, MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f))));
                    ShootProjectiles(tipPosition, spreadForThisProjectile);
                }
                SoundEngine.PlaySound(SoundID.Item38);//play the sound
            }

            FramesToLoadBolt = Owner.ActiveItem().useAnimation; //reset the reload time
            LoadedBolts = 0; //Unload the bow
        }

        public void ShootProjectiles(Vector2 tipPosition, float projectileRotation)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(projectileRotation) * storedVelocity;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<PrecisionBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 35f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            int oldDirection = Projectile.spriteDirection;
            if (oldDirection == -1)
                Projectile.rotation += MathHelper.Pi;

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the bow will have 1 frame of rotational "jitter" when the direction changes based on the
            // original angle. This effect looks very strange in-game.
            if (Projectile.spriteDirection != oldDirection)
                Projectile.rotation -= MathHelper.Pi;

            Projectile.timeLeft = 3;
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
            float loops = LoadedBolts + 1;
            if (LoadedBolts == ClockworkBow.MaxBolts) //If the bow is fully loaded, shave off the part where you draw the arrow that's charging currently
                loops = LoadedBolts;

            for (int i = 0; i < loops; i++)
            {
                float BoltAngle;
                float Shift = 0;

                if (LoadedBolts == 0) //We calculating angles YEAH!
                {
                    BoltAngle = 0;
                }
                else if (LoadedBolts == ClockworkBow.MaxBolts)
                {
                    float increment = angularSpread * (LoadedBolts - 1) / 2;
                    BoltAngle = MathHelper.Lerp(-increment, increment, i / (float)(LoadedBolts - 1));
                }
                else
                {
                    float increment = angularSpread * (LoadedBolts - 1 + MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f)) / 2;
                    BoltAngle = MathHelper.Lerp(-increment, increment, i / (float)(MathHelper.Lerp(LoadedBolts - 1, LoadedBolts, MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f))));
                }

                if (i == LoadedBolts) //If the arrow we are looking at is the one being loaded, we give it some shift (used for position & alpha)
                    Shift = 1 - (CurrentChargingFrames / FramesToLoadBolt);

                // You need to have arrows left for the tint to not remain in stasis
                if ((i == LoadedBolts - 1 || LoadedBolts == ClockworkBow.MaxBolts) && Owner.HasAmmo(Owner.ActiveItem())) //If the arrow we are looking at is the one that just got loaded, OR all arrows got loaded, we apply some flashiness
                {
                    Main.spriteBatch.EnterShaderRegion();
                    GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f - MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f));
                    GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1f - (LoadedBolts / ClockworkBow.MaxBolts), 0.8f, 0.85f));
                    GameShaders.Misc["CalamityMod:BasicTint"].Apply();
                }

                Color Transparency = Color.White * (1 - Shift);
                var BoltTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/PrecisionBolt").Value;
                Vector2 PointingTo = new Vector2((float)Math.Cos(Projectile.rotation + BoltAngle), (float)Math.Sin(Projectile.rotation + BoltAngle)); //It's called trigonometry we do a little trigonometry
                Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
                float FlipFactor = Owner.direction < 0 ? MathHelper.Pi : 0f;
                Vector2 drawPosition = Owner.Center + PointingTo.RotatedBy(FlipFactor) * (20f + (Shift * 40)) - ShiftDown.RotatedBy(FlipFactor) * (BoltTexture.Width / 2) - Main.screenPosition;

                Main.EntitySpriteDraw(BoltTexture, drawPosition, null, Transparency, Projectile.rotation + BoltAngle + MathHelper.PiOver2 + FlipFactor, BoltTexture.Size(), 1f, 0, 0);

                if ((i == LoadedBolts - 1 || LoadedBolts == ClockworkBow.MaxBolts) && Owner.HasAmmo(Owner.ActiveItem())) //Don't forget to exit the shader region
                    Main.spriteBatch.ExitShaderRegion();
            }
            return true;
        }

        public override bool? CanDamage() => false;
    }
}
