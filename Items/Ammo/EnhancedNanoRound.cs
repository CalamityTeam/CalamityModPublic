using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class EnhancedNanoRound : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 5.5f;
            Item.value = Item.sellPrice(copper: 16);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<EnhancedNanoRoundProj>();
            Item.shootSpeed = 8f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(250).
                AddIngredient(ItemID.NanoBullet, 250).
                AddIngredient<EssenceofEleum>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
