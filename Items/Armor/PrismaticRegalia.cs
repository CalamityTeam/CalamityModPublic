using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class PrismaticRegalia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Regalia");
            Tooltip.SetDefault("12% increased magic damage and 15% increased magic crit\n" +
                "20% decreased non-magic damage\n" +
                "+20 max life and +40 max mana\n" +
                "Magic attacks occasionally fire a pair of homing rockets");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 33;

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticRegalia = true;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 40;
            player.magicDamage += 0.12f;
            player.magicCrit += 15;
            player.allDamage -= 0.2f;
            player.magicDamage += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 8);
            recipe.AddIngredient(ItemID.Nanites, 300);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
