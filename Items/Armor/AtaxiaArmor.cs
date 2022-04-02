using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class AtaxiaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Armor");
            Tooltip.SetDefault("+20 max life\n" +
                "8% increased damage and 4% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.allDamage += 0.08f;
            player.Calamity().AllCritBoost(4);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 8);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
