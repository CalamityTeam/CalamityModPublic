using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StarTaintedGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star-Tainted Generator");
            Tooltip.SetDefault("+2 max minions, does not stack with downgrades\n" +
                    "7% increased minion damage\n" +
                    "Minion attacks spawn astral explosions and inflict several debuffs");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
            player.Calamity().starbusterCore = true;
            player.Calamity().starTaintedGenerator = true;
            player.minionDamage += 0.07f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<JellyChargedBattery>());
            recipe.AddIngredient(ModContent.ItemType<NuclearRod>());
            recipe.AddIngredient(ModContent.ItemType<StarbusterCore>());
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
