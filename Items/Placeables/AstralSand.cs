using Terraria.ID;
using Terraria.ModLoader;

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
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            // item.ammo = AmmoID.Sand;
            // item.shoot = ModContent.ProjectileType<AstralSandgun>();
            // item.notAmmo = true;
            // item.shootSpeed = 15f;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.AstralDesert.AstralSand>();
            item.noMelee = true;
        }
    }
}
