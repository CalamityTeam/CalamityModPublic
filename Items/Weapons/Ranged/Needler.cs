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
            Tooltip.SetDefault("Fires needles that stick to enemies and explode");
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
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
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
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<NeedlerProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
