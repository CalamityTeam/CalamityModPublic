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

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        private ref float CurrentChargingFrames => ref projectile.ai[0];
        private ref float PumpkinsCharge => ref projectile.ai[1];
        private ref float FramesToLoadNextPumpkin => ref projectile.localAI[0];
        private ref float Overfilled => ref projectile.localAI[1]; //This functionally is a "IsPlayingShootAnim" variable
        private float angularSpread = MathHelper.ToRadians(15);

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

        public void SmokeBurst(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2 * PumpkinsCharge; i++)
            {
                Dust dust = Dust.NewDustPerfect(tipPosition + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<PumplerDust>());
                dust.velocity = (dust.position - Owner.Center) * 0.3f + Owner.velocity;
                dust.scale = Main.rand.NextFloat(0.3f, 0.8f);
                dust.alpha = Main.rand.Next(50) + 100;
                dust.rotation = Main.rand.NextFloat(6.28f);
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
            SmokeBurst(tipPosition);
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
            int projectileType = ModContent.ProjectileType<PumplerGrenade>();
            int PumpkinDamage = (int)(heldItem.damage * Owner.RangedDamage());
            float shootSpeed = heldItem.shootSpeed * 1.5f;
            float knockback = Owner.GetWeaponKnockback(heldItem, heldItem.knockBack);
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
            //Dom oomfie!
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
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(1f - 0.20f * CurrentChargingFrames - 0.1f*(5f-PumpkinsCharge) , 0f, 1f)); 
                //tint effect is visible if its charging. The more pumpkins are loaded, the more opacity
            }
            else
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0f);
            }
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1f - 0.04f * (PumpkinsCharge - 1), 0.8f, 0.5f));
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

    public class PumplerDust : ModDust
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "CalamityMod/Dusts/SmallSmoke";
            return true;
        }
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 24, 24);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color gray = new Color(40, 40, 40);
            Color ret;
            ret = Color.Lerp(Color.Orange, gray, MathHelper.Clamp((float)(dust.alpha - 100) / 80 , 0f, 1f));
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.85f;

            if (dust.alpha < 165)
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);
                dust.scale += 0.01f;
                dust.alpha += 3;
            }
            else
            {
                dust.scale *= 0.975f;
                dust.alpha += 2;
            }

            dust.position += dust.velocity;
            if (dust.alpha >= 255)
                dust.active = false;

            return false;
        }
    }

    public class PumplerGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squash Shell");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.timeLeft = 180;
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.ignoreWater = true;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PumplerGrenade";

        private void Explode()
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            if (Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(15f, 15f), ModContent.DustType<PumplerDust>());
                    dust.velocity = (dust.position - projectile.Center) * 0.2f + projectile.velocity;
                    dust.scale = Main.rand.NextFloat(0.8f, 1.6f);
                    dust.alpha = Main.rand.Next(30) + 100;
                    dust.rotation = Main.rand.NextFloat(6.28f);
                }
                //CalamityUtils.ExplosionGores(projectile.Center, 1);
            }
            projectile.Kill();
        }
        public override void AI()
        {
            if (projectile.timeLeft == 1)
                Explode();

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Point tileCoords = projectile.Bottom.ToTileCoordinates();
            if (Main.tile[tileCoords.X, tileCoords.Y + 1].nactive() &&
                WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) && projectile.timeLeft < 165)
            {
                Explode();
                projectile.Kill();
            }
            else
            {
                projectile.velocity.Y += 0.4f;
                if (projectile.velocity.Y > 16f)
                    projectile.velocity.Y = 16f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= -1f;
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            Explode();
        }
    }


}



