using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class DaedalusBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Breastplate");
            Tooltip.SetDefault("3% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 20, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.defense = 19; //41
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.03f;
            player.Calamity().AllCritBoost(3);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 15);
            recipe.AddIngredient(ItemID.CrystalShard, 6);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
