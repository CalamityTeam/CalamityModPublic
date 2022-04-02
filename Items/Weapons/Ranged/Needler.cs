using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Needler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Needler");
            Tooltip.SetDefault("Converts musket balls into needles that stick to enemies and explode");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.ranged = true;
            item.width = 44;
            item.height = 26;
            item.useTime = 18;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item108;
            item.autoReuse = true;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<NeedlerProj>();
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<NeedlerProj>(), damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
}
