using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Prismatic
{
    [AutoloadEquip(EquipType.Legs)]
    public class PrismaticGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
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
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticGreaves = true;
            player.GetDamage<MagicDamageClass>() += 0.1f;
            player.GetCritChance<MagicDamageClass>() += 12;
            player.jumpSpeedBoost += 0.1f;
            player.GetDamage<GenericDamageClass>() -= 0.2f;
            player.GetDamage<MagicDamageClass>() += 0.2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>(3).
                AddIngredient<ExodiumCluster>(5).
                AddIngredient<DivineGeode>(6).
                AddIngredient(ItemID.Nanites, 300).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
