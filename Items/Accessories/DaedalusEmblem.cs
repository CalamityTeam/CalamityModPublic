using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class DaedalusEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Emblem");
            Tooltip.SetDefault("10% increased ranged damage, 5% increased ranged critical strike chance, and 20% reduced ammo usage\n" +
                "Increases life regen, minion knockback, defense, and pick speed");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ammoCost80 = true;
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 5;
            player.pickSpeed -= 0.15f;
            player.minionKB += 0.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CelestialStone);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddIngredient(ItemID.RangerEmblem);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
