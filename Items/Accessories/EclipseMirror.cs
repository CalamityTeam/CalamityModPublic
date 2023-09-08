using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EclipseMirror : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 46;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenStandstill += 0.25f;
            modPlayer.rogueStealthMax += 0.1f;
            modPlayer.eclipseMirror = true;
            modPlayer.stealthStrikeHalfCost = true;
            player.GetCritChance<ThrowingDamageClass>() += 6;
            player.GetDamage<ThrowingDamageClass>() += 0.06f;
            player.aggro -= 700;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AbyssalMirror>().
                AddIngredient<DarkMatterSheath>().
                AddIngredient<DarksunFragment>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
