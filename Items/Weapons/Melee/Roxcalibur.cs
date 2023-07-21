using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Roxcalibur : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        private int Roxcooldown = 901;
        private int RoxCanUse = 0;
        private int RoxCanAlt = 0;
        private bool RoxAlt = false;
        private bool Didrefresh = false;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.knockBack = 13;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.NPCHit42;
            Item.width = 100;
            Item.height = 100;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 64;

            Item.shoot = ModContent.ProjectileType<Rox1>();
            Item.shootSpeed = 10f;

            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().donorItem = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 8;

        public override void UpdateInventory(Player player)
        {
            //Timers for the cooldown and usage of the weapon
            Roxcooldown++;
            RoxCanUse++;
            RoxCanAlt++;
            if (Roxcooldown == 600)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(player.Center, player.width, player.height, 191, Main.rand.Next(-8, 9), Main.rand.Next(-8, 9), 100, default, 1);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = player.Center;
                }
                SoundEngine.PlaySound(SoundID.Item70, player.Center);
            }
            // Resets the weapon usage if the alt fire collides with the ground
            if (RoxAlt && player.ownedProjectileCounts[ModContent.ProjectileType<RoxSlam>()] <= 0)
            {
                player.itemAnimation = 0;
                player.itemTime = 0;
                RoxCanUse = 0;
                RoxAlt = false;
                Didrefresh = false;
            }
            // Controls what happens when the alt fire is active
            if (RoxAlt && player.ownedProjectileCounts[ModContent.ProjectileType<RoxSlam>()] > 0)
            {
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
                player.maxFallSpeed = 22f;
                player.controlJump = false;
                player.controlUp = false;
                player.dashDelay = 60;
            }
        }

        public override bool CanUseItem(Player player)
        {
            //Only useable in hardmode and if a second has passed after the alt fire ended
            return Main.hardMode && RoxCanUse >= 60;
        }

        public override bool AltFunctionUse(Player player)
        {
            return RoxCanAlt >= Item.useAnimation + 5 && player.position.Y != player.oldPosition.Y && !player.mount.Active && player.gravDir != -1;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            //Modifies the altfire use style and the location of the sprite of the weapon
            if (player.altFunctionUse == 2)
            {
                player.itemLocation.X -= 32f * player.direction;
                player.itemLocation.Y -= 60f;
                Item.useStyle = ItemUseStyleID.HoldUp;
                RoxAlt = true;
            }
            //Modifies to use style and hold out of the main fire mode
            else
            {
                player.itemLocation += new Vector2(-7.5f * player.direction, 8.5f * player.gravDir).RotatedBy(player.itemRotation);
                Item.useStyle = ItemUseStyleID.Swing;
                RoxAlt = false;
            }
        }

        //Makes the alt fire only spawn the projectile once
        // TODO -- Fix this, because tmod removed this function
        //public override bool OnlyShootOnSwing => RoxAlt;

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            //Changes the hitbox of the alt fire to fit the sprite
            if (player.altFunctionUse == 2)
            {
                if (player.direction == 1)
                {
                    hitbox.X = (int)player.position.X - 9;
                }
                else
                {
                    hitbox.X = (int)player.position.X - 22;
                }
                hitbox.Y = (int)player.position.Y + 20;
                hitbox.Width = 52;
            }
            //Why did i need to fix this hitbox i have no clue
            else
            {
                if (player.direction == -1)
                {
                    hitbox.X += 16;
                }
                hitbox.Width -= 15;
            }
        }

        public override float UseSpeedMultiplier(Player player)
        {
            //If you use alt fire it can hold the sword for way longer
            if (player.altFunctionUse == 2)
            {
                //Use animation 40/0.1 = 400
                return 0.1f;
            }
            else
            {
                return 1f;
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (player.altFunctionUse == 2)
            {
                //Prevents the player from getting stuck on tiles like platforms
                if (player.position.Y == player.oldPosition.Y)
                {
                    player.itemAnimation = 0;
                    player.itemTime = 0;
                    RoxAlt = false;
                    RoxCanUse = 0;
                    if (Didrefresh)
                    {
                        Roxcooldown = 600;
                        Didrefresh = false;
                    }
                }
                // Makes the player forcefully dive down
                if (CalamityUtils.CountHookProj() <= 0)
                    player.velocity.Y = player.maxFallSpeed;
                //Rotates the sprite of the item on alt fire to have it point downwards
                player.itemRotation = player.direction * MathHelper.ToRadians(135f);
                //Dust trail
                int dusty = hitbox.Y + (hitbox.Height / 2);
                Dust.NewDust(new Vector2(hitbox.X, dusty), hitbox.Width, hitbox.Height / 2, 191, player.velocity.X * 0.25f, player.velocity.Y * 0.25f, 160, default, 0.7f);
            }
            else
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 191, 0f, 0f, 160, default, 0.7f);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Item.useStyle == ItemUseStyleID.HoldUp)
            {
                //Spawns a projectile on the tip of the sword in the alt fire
                float positionx;
                float positiony = position.Y + (Item.height / 2) + 23;

                int cooldown = 0;
                //Check if the entire cooldown has passed
                if (Roxcooldown >= 600)
                {
                    cooldown = 1;
                }
                // Makes the projectile spawn in the proper position on the X axis
                if (player.direction == 1)
                {
                    positionx = position.X + 6;
                }
                else
                {
                    positionx = position.X - 8;
                }
                Projectile.NewProjectile(source, positionx, positiony - (player.height / 10f), player.velocity.X, player.velocity.Y, ModContent.ProjectileType<RoxSlam>(), 0, 0, player.whoAmI, cooldown);
                //Resets the cooldown after shooting
                if (Roxcooldown >= 600)
                {
                    Roxcooldown = 0;
                    //"Why yes i did refresh the cooldown whatcha gonna do about it?" -continues on OnHitNPC
                    Didrefresh = true;
                }
                return false;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pertubedspeed = new Vector2(velocity.X / 2, -10f * player.gravDir).RotatedByRandom(MathHelper.ToRadians(10f));
                    Projectile.NewProjectile(source, position.X, position.Y, pertubedspeed.X, pertubedspeed.Y, ModContent.ProjectileType<Rox1>(), (int)(damage * 0.5), 1f, player.whoAmI, Main.rand.Next(3));
                }
                RoxCanAlt = 0;

                return false;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Item.useStyle == ItemUseStyleID.HoldUp)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    //"Well you did refresh the cooldown but like i dont want it to be wasted on an enemy hit so imma ask you for a refund"
                    if (Didrefresh)
                    {
                        Roxcooldown = 600;
                        Didrefresh = false;
                    }
                    //End the alt attack
                    player.itemAnimation = 0;
                    player.itemTime = 0;
                    RoxAlt = false;
                    RoxCanUse = 0;
                    player.velocity.Y = -16f;
                    player.fallStart = (int)(player.position.Y / 16f);
                }
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (Item.useStyle == ItemUseStyleID.HoldUp)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    //"Well you did refresh the cooldown but like i dont want it to be wasted on a player hit so imma ask you for a refund"
                    if (Didrefresh)
                    {
                        Roxcooldown = 600;
                        Didrefresh = false;
                    }
                    //End the alt attack
                    player.itemAnimation = 0;
                    player.itemTime = 0;
                    RoxAlt = false;
                    RoxCanUse = 0;
                    player.velocity.Y = -16f;
                    player.fallStart = (int)(player.position.Y / 16f);
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[WOF]", Main.hardMode ? string.Empty : this.GetLocalizedValue("LockedInfo") + "\n");

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 25).
                AddIngredient(ItemID.SoulofNight, 10).
                AddIngredient<EssenceofHavoc>(5).
                AddIngredient(ItemID.Obsidian, 10).
                AddIngredient(ItemID.StoneBlock, 100).
                AddIngredient(ItemID.Amethyst, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
