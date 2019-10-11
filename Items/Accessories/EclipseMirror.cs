using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EclipseMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse Mirror");
            Tooltip.SetDefault("Its reflection shows naught but darkness\n" +
                "+100 maximum stealth\n" +
                "8% increased rogue damage, and 8% increased rogue crit chance\n" +
                "Vastly reduces enemy aggression, even in the abyss\n" +
                "20% increased stealth regeneration while moving\n" +
                "Stealth regeneration rate exponentially increases while standing still\n" +
                "Stealth strikes have a 100% critical hit chance\n" +
                "Stealth strikes only expend 50% of your max stealth\n" +
                "Grants a small chance to evade attacks in a blast of darksun light, which inflicts extreme damage in a wide area\n" +
                "Evading an attack grants full stealth\n" +
                "This evade has a 20s cooldown before it can occur again");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 22, 0, 0);
            item.Calamity().postMoonLordRarity = 15;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenMoving += 0.2f;
            modPlayer.rogueStealthMax += 1;
            modPlayer.eclipseMirror = true;
            modPlayer.stealthStrikeAlwaysCrits = true;
            modPlayer.stealthStrikeHalfCost = true;
            modPlayer.throwingCrit += 8;
            modPlayer.throwingDamage += 0.08f;
            player.aggro -= 700;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.GetItem("AbyssalMirror"));
            recipe.AddIngredient(mod.GetItem("DarkGodsSheath"));
            recipe.AddIngredient(mod.GetItem("DarksunFragment"), 10);
            recipe.AddIngredient(mod.GetItem("NightmareFuel"), 5);
            recipe.AddIngredient(mod.GetItem("EndothermicEnergy"), 5);
            recipe.AddTile(mod.GetTile("DraedonsForge"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
