using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Astral Sand");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            // item.ammo = AmmoID.Sand;
            // item.shoot = ModContent.ProjectileType<AstralSandgun>();
            // item.notAmmo = true;
            // item.shootSpeed = 15f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.AstralDesert.AstralSand>();
            Item.noMelee = true;
        }
    }
}
