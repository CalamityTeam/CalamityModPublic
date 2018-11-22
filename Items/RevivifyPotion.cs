using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class RevivifyPotion : ModItem
	{
		public override void SetStaticDefaults()
	 	{
	 		DisplayName.SetDefault("Revivify Potion");
	 		Tooltip.SetDefault("Causes enemy attacks to heal you for a fraction of their damage for 15 seconds");
	 	}
	
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useTurn = true;
			item.maxStack = 30;
			item.rare = 3;
			item.useAnimation = 17;
			item.useTime = 17;
			item.useStyle = 2;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
			item.value = 20000;
		}

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
                modPlayer.revivifyTimer = 900;
            }
            return true;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Stardust", 20);
            recipe.AddIngredient(ItemID.HolyWater, 5);
            recipe.AddIngredient(ItemID.CrystalShard, 5);
            recipe.AddIngredient(null, "EssenceofCinder", 3);
            recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodOrb", 50);
            recipe.AddIngredient(ItemID.HolyWater, 5);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}