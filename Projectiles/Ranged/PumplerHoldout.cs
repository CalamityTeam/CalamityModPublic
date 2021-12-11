using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;


//TY dom for coding condemnation, great reference, would steal code from again :)
//                  - Iban

namespace CalamityMod.Projectiles.Ranged
{
    public class PumplerHoldout : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpler");
            Main.projFrames[projectile.type] = 9;
        }



        private Player Owner => Main.player[projectile.owner];

        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        private ref float CurrentChargingFrames => ref projectile.ai[0];
        private ref float PumpkinsCharge => ref projectile.ai[1];
        private ref float FramesToLoadNextPumpkin => ref projectile.localAI[0];
        private ref float Overfilled => ref projectile.localAI[1]; //This functionally is a "IsPlayingShootAnim" variable
        private float angularSpread = MathHelper.ToRadians(20);

        public override string Texture => "CalamityMod/Projectiles/Ranged/PumplerHoldout";

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 34;
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




            // Unloads all pumpkins if you can't shoot/stop holding out the projectile or if the gun is overfilled
            if (!OwnerCanShoot || Overfilled == 1f)
            {

                if (PumpkinsCharge <= 0f && Overfilled == 0f) //If the projectile isnt playing its animation and if there arent any pumpkins loaded, kill it
                {
                    projectile.Kill();
                    return;
                }


                //Animation stuff start
                projectile.frameCounter++;
                if (projectile.frameCounter >= 10) //10 fps anim
                {
                    projectile.frame++;
                    if (projectile.frame >= Main.projFrames[projectile.type] - 1)
                        projectile.frame = 0;
                    projectile.frameCounter = 0;
                }

                // Shoot anim is done? Remove the shoot anim tag
                if (projectile.frame == 0 && Overfilled == 1f)
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

                    projectile.frame = (projectile.frame + 1); //next anim frame
                    CurrentChargingFrames = 0f;
                    ++PumpkinsCharge;
                    FramesToLoadNextPumpkin *= 0.850f;



                    //CombatText.NewText(projectile.Hitbox, new Color(255*(PumpkinsCharge/5), 151 * (PumpkinsCharge / 5), 78 * (PumpkinsCharge / 5)), ((int)PumpkinsCharge).ToString(), true);
                    //Colors don't properly shift towards orange, they remain white fsr. I assume its float fuckery. Also the numbers appear too high up


                    if (PumpkinsCharge >= Pumpler.MaxPumpkins)
                        Main.PlaySound(SoundID.Item69);
                    else
                    {
                        var loadSound = Main.PlaySound(SoundID.Item108);
                        if (loadSound != null)
                            loadSound.Volume *= 0.3f;
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

        //eventually make the gun flash colors on every step with a shader but like ripping stuff off  from slr is complicated


        public void SmokeBurst(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;


            //here im supposed to do smoke that goes bazinga but rn its kinda just copypasted from condemnation. Method aint even fucking used

            for (int i = 0; i < 2; i++)
            {
                Dust chargeMagic = Dust.NewDustPerfect(tipPosition + Main.rand.NextVector2Circular(20f, 20f), 267);
                chargeMagic.velocity = (tipPosition - chargeMagic.position) * 0.1f + Owner.velocity;
                chargeMagic.scale = Main.rand.NextFloat(1f, 1.5f);
                chargeMagic.color = projectile.GetAlpha(Color.White);
                chargeMagic.noGravity = true;
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

            Main.PlaySound(SoundID.Item96);//play the sound
            FramesToLoadNextPumpkin = Owner.ActiveItem().useAnimation; //reset the reload time
            PumpkinsCharge = 0; //Unload the barrel
            if (!(Overfilled == 1f))
                Overfilled = 1f; //Make the shoot anim play
            projectile.frame = 6; //Initialize the animation

        }

        public void ShootProjectiles(Vector2 tipPosition, float projectileRotation)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            int PumpkinDamage = (int)(heldItem.damage * Owner.RangedDamage());
            float shootSpeed = heldItem.shootSpeed * 1.5f;
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot; //Not a very nice thing to say :/
            int projectileType = 0;

            //Might have to change its ammo into grenades or something like that since it doesnt even fire bullets anymore LOL. That or don't use ammo at all
            //really don't wanna make players go through the annoyance of farming pumpkins just to fire a funny weapon
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref PumpkinDamage, ref knockback, false);
            projectileType = ProjectileID.JackOLantern;

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(projectileRotation) * shootSpeed;

            Projectile.NewProjectile(tipPosition, shootVelocity, projectileType, PumpkinDamage, knockback, projectile.owner, 0f, 0f);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == projectile.owner)
            {
                float interpolant = Utils.InverseLerp(5f, 25f, projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (projectile.velocity != oldVelocity)
                {
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }




            //Please someone kill me i dont want to deal with offset moments,
            projectile.position = armPosition - projectile.Size * 0.5f + projectile.velocity.SafeNormalize(Vector2.Zero) * 30f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
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

            spriteBatch.EnterShaderRegion();
            if (PumpkinsCharge > 0 && Overfilled == 0f)
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(1f - 0.25f * CurrentChargingFrames, 0f, 1f)); //tint effect is visible if its charging
            }
            else
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0f);
            }

            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(0.04f * (PumpkinsCharge - 1), 0.8f, 0.5f));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();


            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Rectangle frameRectangle = Main.projectileTexture[projectile.type].Frame(1, 9, 0, projectile.frame);

            spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPosition, frameRectangle, Color.White, projectile.rotation, frameRectangle.Size() * 0.5f, 1f, projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            spriteBatch.ExitShaderRegion();


            return false;
        }




        public override bool CanDamage() => false;












        //netcode hell 
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Overfilled);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Overfilled = reader.ReadInt32();
        }


    }


}
