using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class EnchantedPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Pearl");
            Tooltip.SetDefault("Increases fishing skill\n" +
                "Increases chance to catch crates");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.fishingSkill += 10;
            player.Calamity().enchantedPearl = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FishingPotion).
                AddIngredient(ItemID.CratePotion, 8).
                AddRecipeGroup("Boss2Material", 5).
                AddIngredient<SeaPrism>(10).
                AddIngredient<VictideBar>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
