using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AbyssalMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Mirror");
            Tooltip.SetDefault("Light does not reach the depths of the ocean\n" +
                "Significantly reduces enemy aggression, even in the abyss\n" +
                "Stealth generates 30% faster when standing still and 20% faster while moving\n" +
                "Grants the ability to evade attacks, releasing a cloud of lumenyl fluid which damages and stuns nearby enemies\n" +
                "Evading an attack grants a lot of stealth but has a 90 second cooldown\n" +
                "This cooldown is shared with all other dodges and reflects");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenStandstill += 0.3f;
            modPlayer.stealthGenMoving += 0.2f;
            modPlayer.abyssalMirror = true;
            player.aggro -= 450;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MirageMirror>()
                .AddIngredient<InkBomb>()
                .AddIngredient(ItemID.SpectreBar, 8)
                .AddIngredient<SeaPrism>(10)
                .AddIngredient<DepthCells>(5)
                .AddIngredient<Lumenite>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
