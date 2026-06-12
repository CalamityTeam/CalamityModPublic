using CalamityMod.Projectiles.Typeless;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Astral
{
    public class AstralSand : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.SandBlock, 1);

            ItemID.Sets.SandgunAmmoProjectileData[Type] = new(ModContent.ProjectileType<AstralSandBallGun>(), 5);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AstralDesert.AstralSand>());
            Item.ammo = AmmoID.Sand;
            Item.notAmmo = true;
        }
    }
}
