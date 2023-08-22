using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmbrosialAmpoule : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aAmpoule = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HoneyDew>().
                AddIngredient<RadiantOoze>().
                AddIngredient<LifeAlloy>(3).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
