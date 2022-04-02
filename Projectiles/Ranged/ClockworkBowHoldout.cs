using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.Ranged;


namespace CalamityMod.Projectiles.Ranged
{
    public class ClockworkBowHoldout : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ClockworkBow");
            //Main.projFrames[projectile.type] = 9;    might animate the bow's string getting drawn but not rn
        }

        private Player Owner => Main.player[projectile.owner];

        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        private ref float CurrentChargingFrames => ref projectile.ai[0];
        private ref float LoadedBolts => ref projectile.ai[1];
        private ref float FramesToLoadBolt => ref projectile.localAI[0];
        private ref float LastDirection => ref projectile.localAI[1];

        //private ref float Overfilled => ref projectile.localAI[1]; Until i implement the bow animation there is no need for that
        private float angularSpread = MathHelper.ToRadians(16);

        //public override string Texture => "CalamityMod/Projectiles/Ranged/ClockworkBowHoldout";
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/ClockworkBow";

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 96;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + projectile.velocity * projectile.width * 0.5f;

            // If the player releases left click, shoot out the arrows
            if (!OwnerCanShoot)
            {

                if (LoadedBolts <= 0f) //If theres no arrows to shoot
                {
                    projectile.Kill();
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

                ++CurrentChargingFrames;

                if (CurrentChargingFrames - FramesToLoadBolt / 2 <= 0.01)
                    Main.PlaySound(SoundID.Item17); //Click

                if (CurrentChargingFrames >= FramesToLoadBolt && LoadedBolts < ClockworkBow.MaxBolts)
                {
                    CurrentChargingFrames = 0f;
                    ++LoadedBolts;

                    if (LoadedBolts % 2 == 0)
                        CombatText.NewText(Owner.Hitbox, new Color(155, 255, 255), "Tock", true);
                    else
                        CombatText.NewText(Owner.Hitbox, new Color(255, 200, 100), "Tick", true);

                    FramesToLoadBolt *= 0.950f;

                    if (LoadedBolts >= ClockworkBow.MaxBolts)
                        Main.PlaySound(SoundID.Item23);
                    else
                    {
                        var loadSound = Main.PlaySound(SoundID.Item108);
                        if (loadSound != null)
                            loadSound.Volume *= 0.3f;
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
                Main.PlaySound(SoundID.Item38);//play the sound
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
                Main.PlaySound(SoundID.Item38);//play the sound
            }

            FramesToLoadBolt = Owner.ActiveItem().useAnimation; //reset the reload time
            LoadedBolts = 0; //Unload the bow
        }

        public void ShootProjectiles(Vector2 tipPosition, float projectileRotation)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            int BoltDamage = (int)(heldItem.damage * Owner.RangedDamage() * (LoadedBolts + 1) / (float)(ClockworkBow.MaxBolts + 1));
            float shootSpeed = heldItem.shootSpeed * 1f;
            float knockback = heldItem.knockBack;
            bool uselessFuckYou = OwnerCanShoot; //Not a very nice thing to say :/
            int projectileType = 0;

            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref BoltDamage, ref knockback, false);
            projectileType = ModContent.ProjectileType<PrecisionBolt>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(projectileRotation) * shootSpeed;

            Projectile.NewProjectile(tipPosition, shootVelocity, projectileType, BoltDamage, knockback, projectile.owner, 0f, 0f);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == projectile.owner)
            {
                float interpolant = Utils.InverseLerp(5f, 25f, Owner.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Owner.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (projectile.velocity != oldVelocity)
                {
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }

            projectile.position = armPosition - projectile.Size * 0.5f + projectile.velocity.SafeNormalize(Vector2.Zero) * 35f;
            projectile.rotation = projectile.velocity.ToRotation();
            int oldDirection = projectile.spriteDirection;
            if (oldDirection == -1)
                projectile.rotation += MathHelper.Pi;

            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
            // If the direction differs from what it originaly was, undo the previous 180 degree turn.
            // If this is not done, the bow will have 1 frame of rotational "jitter" when the direction changes based on the
            // original angle. This effect looks very strange in-game.
            if (projectile.spriteDirection != oldDirection)
                projectile.rotation -= MathHelper.Pi;

            projectile.timeLeft = 3;
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
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

                if (i == LoadedBolts - 1 || LoadedBolts == ClockworkBow.MaxBolts) //If the arrow we are looking at is the one that just got loaded, OR all arrows got loaded, we apply some flashiness
                {
                    spriteBatch.EnterShaderRegion();
                    GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f - MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f));
                    GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1f - (LoadedBolts / ClockworkBow.MaxBolts), 0.8f, 0.85f));
                    GameShaders.Misc["CalamityMod:BasicTint"].Apply();
                }

                Color Transparency = Color.White * (1 - Shift);
                var BoltTexture = ModContent.GetTexture("CalamityMod/Projectiles/Ranged/PrecisionBolt");
                Vector2 PointingTo = new Vector2((float)Math.Cos(projectile.rotation + BoltAngle), (float)Math.Sin(projectile.rotation + BoltAngle)); //It's called trigonometry we do a little trigonometry
                Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
                float FlipFactor = Owner.direction < 0 ? MathHelper.Pi : 0f;
                Vector2 drawPosition = Owner.Center + PointingTo.RotatedBy(FlipFactor) * (20f + (Shift * 40)) - ShiftDown.RotatedBy(FlipFactor) * (BoltTexture.Width / 2) - Main.screenPosition;

                spriteBatch.Draw(BoltTexture, drawPosition, null, Transparency, projectile.rotation + BoltAngle + MathHelper.PiOver2 + FlipFactor, BoltTexture.Size(), 1f, 0, 0);

                if (i == LoadedBolts - 1 || LoadedBolts == ClockworkBow.MaxBolts) //Don't forget to exit the shader region
                    spriteBatch.ExitShaderRegion();
            }
            return true;
        }

        public override bool CanDamage() => false;
    }
}
