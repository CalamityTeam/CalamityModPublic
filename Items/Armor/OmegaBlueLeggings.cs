using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class OmegaBlueLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Blue Tentacles");
            Tooltip.SetDefault(@"12% increased movement speed
12% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 35, 25, 0);
            Item.rare = ItemRarityID.Red;
            Item.defense = 22;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.12f;
            player.Calamity().AllCritBoost(12);
            player.moveSpeed += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ReaperTooth>(), 10).AddIngredient(ModContent.ItemType<Lumenite>(), 6).AddIngredient(ModContent.ItemType<Tenebris>(), 6).AddIngredient(ModContent.ItemType<RuinousSoul>(), 3).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
