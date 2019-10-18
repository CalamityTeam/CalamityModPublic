using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Accessories
{
    public class DarkGodsSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark God's Sheath");
            Tooltip.SetDefault("+100 maximum stealth\n" +
                "Stealth regeneration rate increases while standing still\n" +
                "Stealth strikes have a 100% critical hit chance\n" +
                "Stealth strikes only expend 50% of your max stealth\n" +
                "6% increased rogue damage, and 6% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 62;
            item.value = Item.buyPrice(0, 26, 0, 0);
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthStrikeAlwaysCrits = true;
            modPlayer.stealthStrikeHalfCost = true;
            modPlayer.rogueStealthMax += 1;
            modPlayer.darkGodSheath = true;
            modPlayer.throwingCrit += 6;
            modPlayer.throwingDamage += 0.06f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilencingSheath>());
            recipe.AddIngredient(ModContent.ItemType<RuinMedallion>());
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 25);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
