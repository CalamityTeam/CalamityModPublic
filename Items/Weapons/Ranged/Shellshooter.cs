using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Shellshooter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shellshooter");
            Tooltip.SetDefault("Shoots slow, powerful shells");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.ranged = true;
            item.width = 30;
            item.height = 38;
            item.crit += 15;
            item.useTime = 70;
            item.useAnimation = 70;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Shell>();
            item.shootSpeed = 1.5f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Shell>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
