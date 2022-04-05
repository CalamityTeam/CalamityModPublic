using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
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
                "Evading an attack grants full stealth but has a 90 second cooldown\n" +
                "This cooldown is shared with all other dodges and reflects");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.rare = ItemRarityID.Purple;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.accessory = true;
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
            CreateRecipe()
                .AddIngredient<AbyssalMirror>()
                .AddIngredient<DarkGodsSheath>()
                .AddIngredient<DarksunFragment>(20)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
}
