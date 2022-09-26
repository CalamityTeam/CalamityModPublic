using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Tarragon
{
    [AutoloadEquip(EquipType.Legs)]
    public class TarragonLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Tarragon Leggings");
            Tooltip.SetDefault("Leggings of a fabled explorer\n" +
                "10% increased movement speed\n" +
                "8% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.defense = 32;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.GetCritChance<GenericDamageClass>() += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(11).
                AddIngredient<DivineGeode>(12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
