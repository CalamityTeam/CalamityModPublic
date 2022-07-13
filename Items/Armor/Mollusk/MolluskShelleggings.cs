using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Mollusk
{
    [AutoloadEquip(EquipType.Legs)]
    public class MolluskShelleggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Mollusk Shelleggings");
            Tooltip.SetDefault("12% increased damage and 4% increased critical strike chance\n" +
                               "7% decreased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetCritChance<GenericDamageClass>() += 4;
            player.moveSpeed -= 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MolluskHusk>(10).
                AddIngredient<SeaPrism>(20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
