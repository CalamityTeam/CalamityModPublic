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
            Item.width = 18;
            Item.height = 18;
            Item.defense = 21;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticGreaves = true;
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetCritChance(DamageClass.Magic) += 12;
            player.jumpSpeedBoost += 0.1f;
            player.allDamage -= 0.2f;
            player.GetDamage(DamageClass.Magic) += 0.2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ArmoredShell>(), 3).AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 5).AddIngredient(ModContent.ItemType<DivineGeode>(), 6).AddIngredient(ItemID.Nanites, 300).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
