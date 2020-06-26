using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RadiantOoze : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Ooze");
            Tooltip.SetDefault("You emit light and regen life more quickly at night");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().rOoze = true;
            if (!Main.dayTime)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1f, 1f, 0.6f);
                player.lifeRegen += 1;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MurkySludge>(), 5);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
