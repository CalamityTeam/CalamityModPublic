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
	public class MolecularManipulator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Molecular Manipulator");
			Tooltip.SetDefault("Is it nullable or not?  Let's find out!\nFires a fast null bullet that distorts NPC stats\nUses your life as ammo");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 700;
	        item.ranged = true;
	        item.width = 60;
	        item.height = 30;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 8f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shootSpeed = 25f;
	        item.shoot = mod.ProjectileType("NullShot2");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	player.statLife -= 5;
			if (player.statLife <= 0)
			{
				player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
			}
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("NullShot2"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "NullificationRifle");
            recipe.AddIngredient(null, "DarkPlasma", 2);
            recipe.AddIngredient(null, "CoreofCalamity", 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}