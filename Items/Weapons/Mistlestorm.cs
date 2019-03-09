using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class Mistlestorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mistlestorm");
			Tooltip.SetDefault("Casts a storm of pine needles and leaves");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 60;
	        item.magic = true;
	        item.mana = 5;
	        item.width = 48;
	        item.height = 48;
	        item.useTime = 6;
	        item.useAnimation = 6;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item39;
	        item.autoReuse = true;
	        item.shoot = 336;
	        item.shootSpeed = 24f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num106 = 2 + Main.rand.Next(3);
            for (int num107 = 0; num107 < num106; num107++)
            {
                float num110 = 0.025f * (float)num107;
                speedX += (float)Main.rand.Next(-35, 36) * num110;
                speedY += (float)Main.rand.Next(-35, 36) * num110;
                float num84 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                num84 = item.shootSpeed / num84;
                speedX *= num84;
                speedY *= num84;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, (float)Main.rand.Next(0, 10 * (num107 + 1)), 0f);
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 206, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

        public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Razorpine);
            recipe.AddIngredient(ItemID.LeafBlower);
            recipe.AddIngredient(null, "UeliaceBar", 7);
            recipe.AddIngredient(null, "DarkPlasma");
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}