using CalamityMod.Projectiles.Typeless;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            // DisplayName.SetDefault("Astral Sand");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.ammo = AmmoID.Sand;
            Item.shoot = ModContent.ProjectileType<AstralSandBallGun>();
            Item.notAmmo = true;
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
