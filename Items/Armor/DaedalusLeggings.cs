using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class DaedalusLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Leggings");
            Tooltip.SetDefault("3% increased critical strike chance\n" +
                "10% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.defense = 15; //41
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().AllCritBoost(3);
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 10);
            recipe.AddIngredient(ItemID.CrystalShard, 4);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
