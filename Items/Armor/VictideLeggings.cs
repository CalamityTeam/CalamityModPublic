using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class VictideLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Leggings");
            Tooltip.SetDefault("Movement speed increased by 8%\n" +
                "Movement speed increased by 30% while submerged in liquid");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Green;
            item.defense = 4; //9
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) ? 0.3f : 0.08f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
