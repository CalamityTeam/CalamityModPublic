using CalamityMod.Projectiles.Typeless;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items
{
    public class BrokenWaterFilter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Water Filter");
            Tooltip.SetDefault("Releases a bunch of caustic tears\n" +
                               "More tears are released in the water");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 28;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = 1;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = item.useTime = 25;
            item.noUseGraphic = true;
            item.shootSpeed = 7f;
            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<BrokenWaterFilterSpewer>();
        }
    }
}
