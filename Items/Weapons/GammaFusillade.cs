using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class GammaFusillade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Biofusillade");
			Tooltip.SetDefault("Unleashes a concentrated beam of life energy");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 110;
	        item.magic = true;
	        item.mana = 4;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 3;
	        item.useAnimation = 3;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("GammaLaser");
	        item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>().postMoonLordRarity = 12;
		}

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "UeliaceBar", 8);
	        recipe.AddIngredient(ItemID.SpellTome);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
