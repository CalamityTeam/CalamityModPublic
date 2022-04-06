using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AuricTeslaCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Cuisses");
            Tooltip.SetDefault("10% increased movement speed\n" +
                "12% increased damage and 5% increased critical strike chance\n" +
                "Magic carpet effect");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(1, 8, 0, 0);
            Item.defense = 44;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.carpet = true;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.Calamity().AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GodSlayerLeggings>()
                .AddIngredient<BloodflareCuisses>()
                .AddIngredient<TarragonLeggings>()
                .AddIngredient(ItemID.FlyingCarpet)
                .AddIngredient<AuricBar>(15)
                .AddTile<CosmicAnvil>()
                .Register();

            CreateRecipe()
                .AddIngredient<SilvaLeggings>()
                .AddIngredient<BloodflareCuisses>()
                .AddIngredient<TarragonLeggings>()
                .AddIngredient(ItemID.FlyingCarpet)
                .AddIngredient<AuricBar>(15)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
}
