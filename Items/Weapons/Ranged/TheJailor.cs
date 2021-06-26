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
            item.damage = 360;
            item.ranged = true;
            item.width = 102;
            item.height = 70;
            item.useTime = item.useAnimation = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.UseSound = SoundID.Item14;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PrismMine>();
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-40f, -16f);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * item.scale * 45f;

            Projectile.NewProjectile(gunTip, shootVelocity, item.shoot, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
