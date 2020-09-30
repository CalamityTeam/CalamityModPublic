using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Nucleogenesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nucleogenesis");
            Tooltip.SetDefault("Increased max minions by 4 and 15% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Minions inflict a variety of debuffs\n" +
                "Minions spawn damaging sparks on enemy hits"); //subject to change to be "cooler"
		}

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nucleogenesis = true;
            modPlayer.shadowMinions = true; //shadowflame
            modPlayer.tearMinions = true; //temporal sadness
            modPlayer.voltaicJelly = true; //electrified
            modPlayer.starTaintedGenerator = true; //astral infection and irradiated
            player.minionKB += 3f;
            player.minionDamage += 0.15f;
            player.maxMinions += 4;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StarTaintedGenerator>());
            recipe.AddIngredient(ModContent.ItemType<StatisCurse>());
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
