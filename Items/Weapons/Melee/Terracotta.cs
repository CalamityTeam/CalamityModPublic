using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.width = 50;
            item.height = 62;
            item.scale = 1.5f;
            item.damage = 110;
            item.melee = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shootSpeed = 5f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
            {
                float randomSpeedX = Main.rand.Next(3);
                float randomSpeedY = Main.rand.Next(3, 5);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
            }
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<TerracottaExplosion>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), knockback, player.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
            {
                float randomSpeedX = Main.rand.Next(3);
                float randomSpeedY = Main.rand.Next(3, 5);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, -randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, randomSpeedX, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, -randomSpeedY, ModContent.ProjectileType<TerracottaProj>(), 0, 0f, player.whoAmI, player.whoAmI);
            }
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<TerracottaExplosion>(), (int)(item.damage * (player.allDamage + player.meleeDamage - 1f)), item.knockBack, player.whoAmI);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
        }
    }
}
