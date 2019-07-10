using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Projectiles;

namespace CalamityMod.Items.Weapons
{
	public class StarCannonEX : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Cannon EX");
			Tooltip.SetDefault("Fires a mix of normal, starfury, and astral stars");
		}

	    public override void SetDefaults()
	    {
			item.damage = 88;
			item.ranged = true;
			item.width = 66;
			item.height = 22;
			item.useTime = 10;
			item.useAnimation = 10;
			item.useStyle = 5;
            item.rare = 7;
			item.noMelee = true;
			item.knockBack = 8f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.UseSound = SoundID.Item9;
			item.autoReuse = true;
			item.shoot = 12;
			item.shootSpeed = 15f;
			item.useAmmo = AmmoID.FallenStar;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{    
		    int num6 = Main.rand.Next(1, 3);
		    for (int index = 0; index < num6; ++index)
		    {
		        float num7 = speedX;
		        float num8 = speedY;
		        float SpeedX = speedX + (float) Main.rand.Next(-15, 16) * 0.05f;
		        float SpeedY = speedY + (float) Main.rand.Next(-15, 16) * 0.05f;
                switch (Main.rand.Next(3))
                {
                    case 0: break;
                    case 1: type = 9; break;
                    case 2: type = mod.ProjectileType("AstralStar"); break;
                }
		        int star = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[star].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceRanged = true;
			}
		    return false;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StarCannon);
            recipe.AddIngredient(null, "AstralJelly", 10);
			recipe.AddIngredient(null, "Stardust", 25);
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}
