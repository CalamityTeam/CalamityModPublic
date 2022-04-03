using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VampiricTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vampiric Talisman");
            Tooltip.SetDefault(@"Rogue projectiles give lifesteal on crits
12% increased rogue damage");
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.vampiricTalisman = true;
            player.Calamity().throwingDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<RogueEmblem>()).AddIngredient(ModContent.ItemType<SolarVeil>(), 10).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
