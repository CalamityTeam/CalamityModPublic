using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Roxcalibur : ModItem
    {
        private int Roxcooldown = 901;
        private int RoxCanUse = 0;
        private int RoxCanAlt = 0;
        private bool RoxAlt = false;
        private bool Didrefresh = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Roxcalibur");
            Tooltip.SetDefault("You couldn’t get it out of the rock, so you just brought the rock instead\n" +
                "A hellish entity of flesh holds the key to this weapon’s power\n" +
                "Left click to shoot several rock shards\n" +
                "Right click to dive downwards and bounce off enemies\n" +
                "Diving into blocks creates a shockwave");
        }

        public override void SetDefaults()
        {
            item.damage = 200;
            item.knockBack = 13;
            item.melee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.NPCHit42;
            item.width = 100;
            item.height = 100;
            item.autoReuse = true;
            item.useAnimation = 45;
            item.useTime = 45;

            item.shoot = ModContent.ProjectileType<Rox1>();
            item.shootSpeed = 10f;

            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.Calamity().donorItem = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 8;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 25);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 5);
            recipe.AddIngredient(ItemID.Obsidian, 10);
            recipe.AddIngredient(ItemID.StoneBlock, 100);
            recipe.AddIngredient(ItemID.Amethyst, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

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
                Main.PlaySound(SoundID.Item70, player.position);
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
            return RoxCanAlt >= item.useAnimation + 5 && player.position.Y != player.oldPosition.Y && !player.mount.Active && player.gravDir != -1;
        }

        public override void UseStyle(Player player)
        {
            //Modifies the altfire use style and the location of the sprite of the weapon
            if (player.altFunctionUse == 2)
            {
                player.itemLocation.X -= 32f * player.direction;
                player.itemLocation.Y -= 60f;
                item.useStyle = ItemUseStyleID.HoldingUp;
                RoxAlt = true;
            }
            //Modifies to use style and hold out of the main fire mode
            else
            {
                player.itemLocation += new Vector2(-7.5f * player.direction, 8.5f * player.gravDir).RotatedBy(player.itemRotation);
                item.useStyle = ItemUseStyleID.SwingThrow;
                RoxAlt = false;
            }
        }

        //Makes the alt fire only spawn the projectile once
        public override bool OnlyShootOnSwing => RoxAlt;

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

        public override float MeleeSpeedMultiplier(Player player)
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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.useStyle == ItemUseStyleID.HoldingUp)
            {
                //Spawns a projectile on the tip of the sword in the alt fire
                float positionx;
                float positiony = position.Y + (item.height / 2) + 23;

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
                Projectile.NewProjectile(positionx, positiony - (player.height / 10f), player.velocity.X, player.velocity.Y, ModContent.ProjectileType<RoxSlam>(), 0, 0, player.whoAmI, cooldown);
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
                for (int i = 0; i < 3; i++)
                {
                    //Else shoot the spread of rock shards
                    int rotation = Main.rand.Next(-10, 11);
                    Vector2 pertubedspeed = new Vector2(speedX / 2, -10f * player.gravDir).RotatedBy(MathHelper.ToRadians(rotation));
                    Projectile.NewProjectile(position.X, position.Y, pertubedspeed.X, pertubedspeed.Y, ModContent.ProjectileType<Rox1>(), (int)(damage * 0.5), 1f, player.whoAmI, Main.rand.Next(3));
                }
                RoxCanAlt = 0;

                return false;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (item.useStyle == ItemUseStyleID.HoldingUp)
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

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (item.useStyle == ItemUseStyleID.HoldingUp)
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
    }
}
