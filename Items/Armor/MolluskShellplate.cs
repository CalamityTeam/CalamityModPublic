using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class MolluskShellplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Shellplate");
            Tooltip.SetDefault("10% increased damage and 6% increased critical strike chance\n" +
                               "15% decreased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.1f;
            player.Calamity().AllCritBoost(6);
            player.moveSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MolluskHusk>(15)
                .AddIngredient<SeaPrism>(25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
