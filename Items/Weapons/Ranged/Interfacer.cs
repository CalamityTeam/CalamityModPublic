using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Interfacer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Disseminator");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Fires a spread of bullets from the gun, above, and below");
        }

        public override void SetDefaults()
        {
            item.damage = 52;
            item.ranged = true;
            item.width = 66;
            item.height = 24;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item38;
            item.autoReuse = true;
            item.shootSpeed = 13f;
            item.shoot = ProjectileID.PurificationPowder;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			//shotgun spread
            int num6 = Main.rand.Next(4, 6);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-30, 31) * 0.05f;
                int bullet = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                Main.projectile[bullet].extraUpdates += 1;
            }
            float num72 = item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = player.direction;
            }
            else
            {
                num80 = num72 / num80;
            }

			//bullets from above
            int num107 = Main.rand.Next(4, 6);
			int bulletDamage = (int)(damage * 0.7f);
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(51) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-50, 51);
                vector2.Y -= 100 * num108;
                num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
                num80 = num72 / num80;
                num78 *= num80;
                num79 *= num80;
                float speedX4 = num78 + Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = num79 + Main.rand.Next(-30, 31) * 0.02f;
                int bullet = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, bulletDamage, knockBack, player.whoAmI, 0.0f, 0.0f);
                Main.projectile[bullet].extraUpdates += 1;
                Main.projectile[bullet].tileCollide = false;

				//bullets from below
                vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(51) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y + 600f);
                vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-50, 51);
                vector2.Y += 100 * num108;
                num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
                num80 = num72 / num80;
                num78 *= num80;
                num79 *= num80;
                float speedX6 = num78 + Main.rand.Next(-30, 31) * 0.02f;
                float speedY7 = num79 + Main.rand.Next(-30, 31) * 0.02f;
                int bullet2 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX6, -speedY7, type, bulletDamage, knockBack, player.whoAmI, 0.0f, 0.0f);
                Main.projectile[bullet2].extraUpdates += 1;
                Main.projectile[bullet2].tileCollide = false;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ConferenceCall>());
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
