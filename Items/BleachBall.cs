using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items
{
    public class BleachBall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bleach Ball");
            Tooltip.SetDefault("Favorite this item to prevent the Aquatic Scourge from naturally spawning near you");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 46;
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
                player.Calamity().disableNaturalScourgeSpawns = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlightedGel>(5).
                AddIngredient(ItemID.CalmingPotion).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
