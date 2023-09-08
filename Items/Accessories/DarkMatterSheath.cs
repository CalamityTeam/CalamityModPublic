using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("DarkGodsSheath")]
    public class DarkMatterSheath : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 62;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthStrikeHalfCost = true;
            modPlayer.rogueStealthMax += 0.1f;
            modPlayer.darkGodSheath = true;
            player.GetCritChance<ThrowingDamageClass>() += 6;
            player.GetDamage<ThrowingDamageClass>() += 0.06f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SilencingSheath>().
                AddIngredient<RuinMedallion>().
                AddIngredient<MeldConstruct>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
