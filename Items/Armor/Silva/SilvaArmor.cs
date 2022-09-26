using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Silva
{
    [AutoloadEquip(EquipType.Body)]
    public class SilvaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Silva Armor");
            Tooltip.SetDefault("+80 max life\n" +
                       "12% increased damage and 8% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 44;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetCritChance<GenericDamageClass>() += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EffulgentFeather>(10).
                AddRecipeGroup("AnyGoldBar", 10).
                AddIngredient<Tenebris>(12).
                AddIngredient<AscendantSpiritEssence>(3).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
