using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VampiricTalisman : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 48;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.vampiricTalisman = true;
            player.GetDamage<ThrowingDamageClass>() += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RogueEmblem>().
                AddIngredient<SolarVeil>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
