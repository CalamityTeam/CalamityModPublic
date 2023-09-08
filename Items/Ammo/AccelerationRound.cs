using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    [LegacyName("AccelerationBullet")]
    public class AccelerationRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 10;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 1.25f;
            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<AccelerationRoundProj>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MusketBall, 150).
                AddIngredient<PearlShard>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
