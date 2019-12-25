using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AstrealDefeat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astreal Defeat");
            Tooltip.SetDefault("Fires Astreal Arrows that emit flames as they travel\n" +
                       "Ethereal bow of the tyrant king's mother\n" +
                       "The mother strongly discouraged acts of violence throughout her life\n" +
                       "Though she kept this bow close to protect her family in times of great disaster");
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.ranged = true;
            item.width = 40;
            item.height = 78;
            item.useTime = 3;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstrealArrow>();
            item.shootSpeed = 4f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 8f)
			{
				velocity.Normalize();
				velocity *= 8f;
			}

			float ai0 = (float)Main.rand.Next(4);
            Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<AstrealArrow>(), damage, knockBack, player.whoAmI, ai0, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiritFlame);
            recipe.AddIngredient(ItemID.ShadowFlameBow);
            recipe.AddIngredient(ModContent.ItemType<GreatbowofTurmoil>());
            recipe.AddIngredient(ModContent.ItemType<BladedgeGreatbow>());
            recipe.AddIngredient(ModContent.ItemType<DarkechoGreatbow>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
