using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Patreon
{
    public class LightGodsBrilliance : ModItem
    {
		
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light God's Brilliance");
            Tooltip.SetDefault("Casts small, homing light beads along with explosive light balls");
        }

        public override void SetDefaults()
        {
            item.damage = 100;
            item.magic = true;
            item.mana = 4;
            item.width = 34;
            item.height = 36;
            item.useTime = 3;
            item.useAnimation = 3;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item9;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("LightBead");
            item.shootSpeed = 25f;
        }
		
		public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(139, 0, 0);
                }
            }
        }
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			int num6 = Main.rand.Next(2, 5);
			for (int index = 0; index < num6; ++index)
			{
				float num7 = speedX;
				float num8 = speedY;
				float SpeedX = speedX + (float) Main.rand.Next(-50, 51) * 0.05f;
				float SpeedY = speedY + (float) Main.rand.Next(-50, 51) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
			}
			if (Main.rand.Next(3) == 0)
			{
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("LightBall"), (int)((double)damage * 2.0), knockBack, player.whoAmI, 0.0f, 0.0f);
			}
			
			return false;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ShadecrystalTome");
			recipe.AddIngredient(null, "AbyssalTome");
			recipe.AddIngredient(ItemID.HolyWater, 10);
			recipe.AddIngredient(null, "EndothermicEnergy", 5);
			recipe.AddIngredient(null, "NightmareFuel", 5);
			recipe.AddIngredient(ItemID.SoulofLight, 30);
			recipe.AddIngredient(null, "EffulgentFeather", 5);
			recipe.AddTile(TileID.Bookcases);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}