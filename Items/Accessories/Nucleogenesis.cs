using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Nucleogenesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nucleogenesis");
            Tooltip.SetDefault("Increases max minions by 4, does not stack with downgrades\n" +
                "15% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Minions inflict a variety of debuffs\n" +
                "Minions spawn damaging sparks on enemy hits"); //subject to change to be "cooler"
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nucleogenesis = true;
            modPlayer.shadowMinions = true; //shadowflame
            modPlayer.holyMinions = true; //holy flames
            modPlayer.voltaicJelly = true; //electrified
            modPlayer.starTaintedGenerator = true; //astral infection and irradiated
            player.minionKB += 3f;
            player.GetDamage(DamageClass.Summon) += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StarTaintedGenerator>()
                .AddIngredient<StatisCurse>()
                .AddIngredient(ItemID.LunarBar, 8)
                .AddIngredient<GalacticaSingularity>(4)
                .AddIngredient<AscendantSpiritEssence>(4)
                .AddTile(ModContent.TileType<CosmicAnvil>())
                .Register();
        }
    }
}
