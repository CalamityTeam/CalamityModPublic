using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AntiCystOintment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-Cyst Ointment");
            Tooltip.SetDefault("Favorite this item to prevent perforator cysts from spawning near you");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.SpawnPrevention;
		}

        public override void UpdateInventory(Player player)
        {
            if (Item.favorited)
                player.Calamity().disablePerfCystSpawns = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrimtaneBar, 5).
                AddIngredient(ItemID.BottledWater).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
