using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("FlashBullet")]
    public class FlashRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 12;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.15f;
            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<FlashRoundProj>();
            Item.shootSpeed = 12f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Grenade).
                AddIngredient(ItemID.Glass, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
