using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClockworkBow : ModItem
    {
        public const int MaxBolts = 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clockwork Bow");
            Tooltip.SetDefault("Hold left click to load up to six precision bolts\n" +
                "The more precision bolts are loaded, the harder they hit");
        }

        public override void SetDefaults()
        {
            item.damage = 66;
            item.ranged = true;
            item.width = 48;
            item.height = 92;
            item.useTime = 60;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.25f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 30f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Cog, 50);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool CanUseItem(Player player)
        {
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item20;
            item.channel = true;
            return player.ownedProjectileCounts[ModContent.ProjectileType<ClockworkBowHoldout>()] <= 0;
        }
        public override float UseTimeMultiplier(Player player)
        {
            return 1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<ClockworkBowHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }
    }
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

        //private ref float Overfilled => ref projectile.localAI[1]; Until i implement the bow animation there is no need for that
        private float angularSpread = MathHelper.ToRadians(16);

        //public override string Texture => "CalamityMod/Projectiles/Ranged/ClockworkBowHoldout";
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/ClockworkBow";

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 92;
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

                    if (LoadedBolts%2 == 0)
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
                    float increment = angularSpread * (LoadedBolts-1)/2;
                    float spreadForThisProjectile = MathHelper.Lerp(-increment, increment, i / (float)(LoadedBolts-1));
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
                    float increment = angularSpread * (LoadedBolts - 1 + MathHelper.Clamp((CurrentChargingFrames / FramesToLoadBolt) * 2, 0f, 1f))/2;
                    float spreadForThisProjectile = MathHelper.Lerp(-increment, increment, i / (float)(LoadedBolts));
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
            int BoltDamage = (int)(heldItem.damage * Owner.RangedDamage() * (2-(ClockworkBow.MaxBolts-LoadedBolts)));
            float shootSpeed = heldItem.shootSpeed * 1.5f;
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
                float interpolant = Utils.InverseLerp(5f, 25f, projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (projectile.velocity != oldVelocity)
                {
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }
            projectile.position = armPosition - projectile.Size * 0.5f + projectile.velocity.SafeNormalize(Vector2.Zero) * 35f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = projectile.direction;
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

            float loops = LoadedBolts +1;
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
                    BoltAngle = MathHelper.Lerp(-increment, increment, i / (float)(LoadedBolts-1));
                    }
                else
                    {
                    float increment = angularSpread * (LoadedBolts - 1 + MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt) , 0f, 1f)) / 2;
                    BoltAngle = MathHelper.Lerp(-increment, increment, i / (float)(MathHelper.Lerp(LoadedBolts-1, LoadedBolts, MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f))));
                    }

                if (i == LoadedBolts) //If the arrow we are looking at is the one being loaded, we give it some shift (used for position & alpha)
                    Shift = 1-(CurrentChargingFrames / FramesToLoadBolt);
                
                if (i == LoadedBolts-1 || LoadedBolts == ClockworkBow.MaxBolts) //If the arrow we are looking at is the one that just got loaded, OR all arrows got loaded, we apply some flashiness
                {
                    spriteBatch.EnterShaderRegion();
                    GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f - MathHelper.Clamp((CurrentChargingFrames * 2 / FramesToLoadBolt), 0f, 1f));
                    GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1f - (LoadedBolts/ClockworkBow.MaxBolts), 0.8f, 0.85f));
                    GameShaders.Misc["CalamityMod:BasicTint"].Apply();
                }

                Color Transparency = Color.White * (1 - Shift);
                var BoltTexture = ModContent.GetTexture("CalamityMod/Projectiles/Ranged/PrecisionBolt");
                Vector2 PointingTo = new Vector2((float)Math.Cos(projectile.rotation + BoltAngle), (float)Math.Sin(projectile.rotation + BoltAngle)); //It's called trigonometry we do a little trigonometry
                Vector2 ShiftDown = PointingTo.RotatedBy(-MathHelper.PiOver2);
                //Vector2 ShiftDown = new Vector2((float)Math.Cos(projectile.rotation + BoltAngle - MathHelper.PiOver2), (float)Math.Sin(projectile.rotation + BoltAngle - MathHelper.PiOver2)); //Shift the arrow halfway down so it appears aligned. I'm also P sure theres a function to rotate a vector by an angle but i forgor
                Vector2 drawPosition = Owner.Center+ PointingTo*(20f+(Shift*40)) - ShiftDown*(BoltTexture.Width/2) - Main.screenPosition;

                spriteBatch.Draw(BoltTexture, drawPosition, null, Transparency, projectile.rotation + BoltAngle + MathHelper.PiOver2 , BoltTexture.Size(), 1f, 0, 0);

                if (i == LoadedBolts - 1 || LoadedBolts == ClockworkBow.MaxBolts) //Don't forget to exit the shader region
                    spriteBatch.ExitShaderRegion();
            }
            return true;
        }

        public override bool CanDamage() => false;
    }

    public class PrecisionBolt : ModProjectile
    {


        NPC potentialTarget = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("PrecisionBolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.timeLeft = 119;
            projectile.penetrate = 1;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PrecisionBolt";

        private Vector2 Recalibrate()
        {            
            float Increment = MathHelper.ToRadians(80 * (projectile.timeLeft / 120)); //Get the rotation increment that should be applied to the bolt

            //return the velocity of the projectile rotated in the direction that makes it closer to its target. Doesnt work somehow
            if (projectile.velocity.RotatedBy(Increment).AngleBetween(projectile.SafeDirectionTo(potentialTarget.Center)) < projectile.velocity.RotatedBy(-Increment).AngleBetween(projectile.SafeDirectionTo(potentialTarget.Center)))
               return projectile.velocity.RotatedBy(Increment);
            else
                return projectile.velocity.RotatedBy(-Increment);

        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.LightSteelBlue.ToVector3());
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.timeLeft % 10 ==0)
                Main.PlaySound(SoundID.Item93, projectile.Center);
            if (potentialTarget == null)
                potentialTarget = projectile.Center.ClosestNPCAt(1500f, true);
            if (potentialTarget != null && projectile.timeLeft % 10 == 0)
                projectile.velocity = Recalibrate();


            Dust trail = Dust.NewDustPerfect(projectile.Center, 267); //Dust trail kinda poopy but idk how to make a cool trail :(
            trail.velocity = Vector2.Zero;
            trail.color = Color.Yellow;
            trail.scale = Main.rand.NextFloat(1f, 1.1f);
            trail.noGravity = true;
        }

        public override void Kill(int timeLeft)
        {
            // Release a burst of magic dust on death.
            if (Main.dedServ)
                return;

            Main.PlaySound(SoundID.Item94, projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust zap = Dust.NewDustPerfect(projectile.Center, 267);
                zap.velocity = projectile.velocity;
                zap.color =  Color.Yellow;
                zap.scale = Main.rand.NextFloat(1f, 1.1f);
                zap.noGravity = true;
            }
        }
    }

}
