using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class MortarRound : ModItem, ILocalizedModType
    {
        internal static int TileBlastRadiusNormal = 3;
        internal static int TileBlastRadiusGFB = 5;
        public static int TileBlastRadius => Main.getGoodWorld ? TileBlastRadiusGFB : TileBlastRadiusNormal;
        public static int HitboxBlastRadius => TileBlastRadius * 16 + 12;

        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 7f;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.Yellow;
            Item.ammo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<MortarRoundProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.EmptyBullet, 100).
                AddIngredient(ItemID.ExplosivePowder, 4).
                AddIngredient<ScoriaBar>().
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
