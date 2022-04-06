using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class SulfurLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Leggings");
            Tooltip.SetDefault("Movement speed increased by 10%\n" +
                "Movement speed increased by 35% while submerged in liquid");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.defense = 5;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) ? 0.35f : 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<UrchinStinger>(30)
                .AddIngredient<Acidwood>(15)
                .AddIngredient<SulphurousSand>(15)
                .AddIngredient<SulfuricScale>(15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
