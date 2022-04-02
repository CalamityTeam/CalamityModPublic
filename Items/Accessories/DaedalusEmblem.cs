using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DaedalusEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artemis Emblem");
            Tooltip.SetDefault("10% increased ranged damage, 5% increased ranged critical strike chance, and 20% reduced ammo usage\n" +
                "5 increased defense, 2 increased life regen, and 15% increased pick speed");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().rangedAmmoCost *= 0.8f;
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 5;
            player.pickSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CelestialStone);
            recipe.AddIngredient(ItemID.RangerEmblem);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
