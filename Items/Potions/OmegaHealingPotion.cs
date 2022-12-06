using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class OmegaHealingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Healing Potion");
            SacrificeTotal = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 32;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.healLife = 300;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.potion = true;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient<SupremeHealingPotion>(4).
                AddIngredient<AscendantSpiritEssence>().
                AddTile(TileID.Bottles).
                Register();
        }
    }
}
