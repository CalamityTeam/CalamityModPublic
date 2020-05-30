using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LivingDew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Dew");
            Tooltip.SetDefault("5% increased damage reduction, +5 defense, and increased life regen while in the Jungle");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneJungle)
            {
                player.lifeRegen += 1;
                player.statDefense += 5;
                player.endurance += 0.05f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ManeaterBulb>(), 2);
            recipe.AddIngredient(ModContent.ItemType<TrapperBulb>(), 2);
            recipe.AddIngredient(ModContent.ItemType<MurkyPaste>(), 5);
            recipe.AddIngredient(ModContent.ItemType<GypsyPowder>());
            recipe.AddIngredient(ModContent.ItemType<BeetleJuice>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
