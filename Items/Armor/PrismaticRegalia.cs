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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Prismatic Regalia");
            Tooltip.SetDefault("12% increased magic damage and 15% increased magic crit\n" +
                "20% decreased non-magic damage\n" +
                "+20 max life and +40 max mana\n" +
                "Magic attacks occasionally fire a pair of homing rockets");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 33;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().prismaticRegalia = true;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 40;
            player.GetDamage<MagicDamageClass>() += 0.12f;
            player.GetCritChance<MagicDamageClass>() += 15;
            player.GetDamage<GenericDamageClass>() -= 0.2f;
            player.GetDamage<MagicDamageClass>() += 0.2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>(3).
                AddIngredient<ExodiumClusterOre>(5).
                AddIngredient<DivineGeode>(8).
                AddIngredient(ItemID.Nanites, 300).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
