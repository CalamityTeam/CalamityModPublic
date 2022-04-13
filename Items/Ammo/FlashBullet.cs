using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Ammo
{
    public class FlashBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flash Round");
            Tooltip.SetDefault("Gives off a concussive blast that confuses enemies in a large area for a short time");
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 12;
            Item.height = 18;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 1.15f;
            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<FlashBulletProj>();
            Item.shootSpeed = 12f;
            Item.ammo = AmmoID.Bullet;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
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
