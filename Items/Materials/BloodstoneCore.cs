using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BloodstoneCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Bloodstone Core");
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient<Bloodstone>(5).
                AddIngredient<BloodOrb>().
                AddIngredient<Phantoplasm>().
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
