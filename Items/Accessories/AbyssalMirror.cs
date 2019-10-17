using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class AbyssalMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Mirror");
            Tooltip.SetDefault("Light does not reach the depths of the ocean\n" +
                "Significantly reduces enemy aggression, even in the abyss\n" +
                "15% increased stealth regeneration while moving\n" +
                "Grants a slight chance to evade attacks, releasing a cloud of lumenyl fluid which damages and stuns nearby enemies\n" +
                "Evading an attack grants a lot of stealth\n" +
                "This evade has a 20s cooldown before it can occur again");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenMoving += 0.15f;
            modPlayer.abyssalMirror = true;
            player.aggro -= 450;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.GetItem("MirageMirror"));
            recipe.AddIngredient(mod.GetItem("InkBomb"));
            recipe.AddIngredient(ItemID.SpectreBar, 8);
            recipe.AddIngredient(ItemID.BlackBelt);
            recipe.AddIngredient(mod.GetItem("DepthCells"), 5);
            recipe.AddIngredient(mod.GetItem("Lumenite"), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
