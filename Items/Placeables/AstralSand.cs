using Terraria.ModLoader;
using CalamityMod.Projectiles;
namespace CalamityMod.Items.Placeables
{
    public class AstralSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Sand");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.AstralSand>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.shoot = ModContent.ProjectileType<AstralSandgun>();
            item.shootSpeed = 15f;
            item.ammo = 169;
        }
    }
}
