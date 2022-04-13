using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Ammo
{
    public class AcidBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Round");
            Tooltip.SetDefault("Explodes into acid that inflicts the plague\n" +
                "Does more damage the higher the target's defense");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 16);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<AcidBulletProj>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Bullet;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MusketBall, 150).
                AddIngredient<PlagueCellCluster>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
