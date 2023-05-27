using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("MeldiateBar")]
    public class MeldConstruct : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
            Item.rare = ItemRarityID.Cyan;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient<MeldBlob>(6).
                AddIngredient<Stardust>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
