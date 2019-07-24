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
	public class Starfleet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starfleet");
			Tooltip.SetDefault("Fires a spread of plasma blasts");
		}

	    public override void SetDefaults()
	    {
			item.damage = 70;
			item.ranged = true;
			item.width = 80;
			item.height = 26;
			item.useTime = 55;
			item.useAnimation = 55;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 15f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item92;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("PlasmaBlast");
			item.shootSpeed = 12f;
			item.useAmmo = 75;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{    
		    for (int index = 0; index < 5; ++index)
		    {
		        float num7 = speedX;
		        float num8 = speedY;
		        float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
		        float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
		        Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		    }
		    return false;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "StarCannonEX");
            recipe.AddIngredient(ItemID.ElectrosphereLauncher);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}