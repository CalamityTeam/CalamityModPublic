using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CreationApparatus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Creation Apparatus");
            Tooltip.SetDefault("Increased max minions by 4 and 15% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Minions inflict a variety of debuffs\n" +
                "Minions spawn damaging sparks on enemy hits\n" + //subject to change to be "cooler"
                "Minion attacks have a chance to instantly kill normal enemies");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.creationApparatus = true;
            modPlayer.shadowMinions = true; //shadowflame
            modPlayer.tearMinions = true; //temporal sadness
            modPlayer.voltaicJelly = true; //electrified
            modPlayer.starTaintedGenerator = true; //astral infection
            player.minionKB += 2.5f;
            player.minionDamage += 0.15f;
            player.maxMinions += 4;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StarTaintedGenerator>());
            recipe.AddIngredient(ModContent.ItemType<StatisCurse>());
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 20);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
