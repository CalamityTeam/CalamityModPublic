using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class PrismaticGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Greaves");
            Tooltip.SetDefault("10% increased magic damage and 12% increased magic crit\n" +
                "20% decreased non-magic damage\n" +
                "10% increased flight time and 2% increased jump speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 21;

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticGreaves = true;
            player.magicDamage += 0.1f;
            player.magicCrit += 12;
            player.jumpSpeedBoost += 0.1f;
            player.allDamage -= 0.2f;
            player.magicDamage += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 6);
            recipe.AddIngredient(ItemID.Nanites, 300);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
