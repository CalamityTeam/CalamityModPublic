using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EclipseMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse Mirror");
            Tooltip.SetDefault("Its reflection shows naught but darkness\n" +
                "+20 maximum stealth\n" +
                "6% increased rogue damage, and 6% increased rogue crit chance\n" +
                "Vastly reduces enemy aggression, even in the abyss\n" +
                "Stealth generates 20% faster when standing still\n" +
                "Mobile stealth generation exponentially accelerates while not attacking\n" +
                "Stealth strikes have a 100% critical hit chance\n" +
                "Stealth strikes only expend 50% of your max stealth\n" +
                "Grants the ability to evade attacks in a blast of darksun light, which inflicts extreme damage in a wide area\n" +
                "Evading an attack grants full stealth\n" +
				"This evade has a 75 second cooldown and does not work while a reflection effect is on cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;
			item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenStandstill += 0.2f;
            modPlayer.rogueStealthMax += 0.2f;
            modPlayer.eclipseMirror = true;
            modPlayer.stealthStrikeAlwaysCrits = true;
            modPlayer.stealthStrikeHalfCost = true;
            modPlayer.throwingCrit += 6;
            modPlayer.throwingDamage += 0.06f;
            player.aggro -= 700;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AbyssalMirror>());
            recipe.AddIngredient(ModContent.ItemType<DarkGodsSheath>());
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
