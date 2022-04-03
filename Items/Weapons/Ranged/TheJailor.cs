using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheJailor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Jailor");
            Tooltip.SetDefault("Releases electric mines outward that connect to each-other via arcs");
        }

        public override void SetDefaults()
        {
            Item.damage = 360;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 102;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.UseSound = SoundID.Item14;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrismMine>();
            Item.shootSpeed = 14.5f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-40f, -16f);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * Item.scale * 45f;

            Projectile.NewProjectile(gunTip, shootVelocity, Item.shoot, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
