using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class ReaverScaleMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Scale Mail");
            Tooltip.SetDefault("9% increased damage and 4% increased critical strike chance\n" +
                "+20 max life");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 22;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 19;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.allDamage += 0.09f;
            player.Calamity().AllCritBoost(4);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 15);
            recipe.AddIngredient(ItemID.JungleSpores, 12);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
