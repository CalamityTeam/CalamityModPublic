using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class GodSlayerChestplate : ModItem
    {
        public const int DashIFrames = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Chestplate");
            Tooltip.SetDefault("+60 max life\n" +
                       "Enemies take damage when they hit you\n" +
                       "11% increased damage and 6% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 41;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.thorns += 0.5f;
            player.statLifeMax2 += 60;
            player.allDamage += 0.11f;
            modPlayer.AllCritBoost(6);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 23);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 3);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
