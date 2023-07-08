using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DeificAmulet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dAmulet = true;
            player.longInvince = true;
            player.pStone = true;
            player.lifeRegen += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CharmofMyths).
                AddIngredient(ItemID.StarVeil).
                AddIngredient<AstralBar>(10).
                AddIngredient<SeaPrism>(15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
