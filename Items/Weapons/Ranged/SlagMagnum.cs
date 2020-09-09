using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SlagMagnum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slag Magnum");
            Tooltip.SetDefault("Shoots fossil shards that split into additional shards on death");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.ranged = true;
            item.width = 58;
            item.height = 24;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 22f;
            item.shoot = ModContent.ProjectileType<SlagRound>();
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SlagRound>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
