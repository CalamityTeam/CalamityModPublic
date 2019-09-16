using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Genisis : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Genisis");
			Tooltip.SetDefault("Fires a Y-shaped beam of destructive energy and a spread of lasers");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 65;
	        item.magic = true;
	        item.mana = 4;
	        item.width = 74;
	        item.height = 28;
	        item.useTime = 3;
	        item.useAnimation = 3;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 1.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shootSpeed = 6f;
	        item.shoot = mod.ProjectileType("BigBeamofDeath");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
	        int num6 = 3;
	        float SpeedX = speedX + (float) Main.rand.Next(-20, 21) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-20, 21) * 0.05f;
	        for (int index = 0; index < num6; ++index)
	        {
	            int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX * 1.05f, SpeedY * 1.05f, 440, (int)((double)damage * 0.75), knockBack, player.whoAmI, 0f, 0f);
	            Main.projectile[projectile].timeLeft = 120;
	        }
	        return false;
		}

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.LaserMachinegun);
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddIngredient(null, "BarofLife", 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
