using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SilvaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Silva Armor");
            Tooltip.SetDefault("+80 max life\n" +
                       "12% increased damage and 8% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 72, 0, 0);
            Item.defense = 44;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.Calamity().AllCritBoost(8);
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
