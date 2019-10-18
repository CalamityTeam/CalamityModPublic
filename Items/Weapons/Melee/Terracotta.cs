using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Terracotta : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra-cotta");
            Tooltip.SetDefault("Causes enemies to erupt into healing projectiles on death\n" +
                "Enemies explode on hit");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.damage = 125;
            item.melee = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shootSpeed = 5f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                float randomSpeedX = (float)Main.rand.Next(3);
                float randomSpeedY = (float)Main.rand.Next(3, 5);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<Projectiles.Terracotta>(), 0, 0f, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<Projectiles.Terracotta>(), 0, 0f, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<Projectiles.Terracotta>(), 0, 0f, player.whoAmI);
            }
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<TerracottaExplosion>(), (int)((float)item.damage * player.meleeDamage), knockback, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
            }
        }
    }
}
