using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardDiamond : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diamond");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0f / 255f);
            projectile.rotation -= (MathHelper.ToRadians(90) * projectile.direction);
            projectile.spriteDirection = projectile.direction;
            if (Main.rand.NextBool(2))
            {
                int num137 = Dust.NewDust(projectile.position, 1, 1, 30, 0f, 0f, 0, default, 0.5f);
                Main.dust[num137].velocity *= 0f;
                Main.dust[num137].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 50;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.Center);
            for (int d = 0; d < 10; d++)
            {
                int paper = Dust.NewDust(projectile.position, projectile.width, projectile.height, 30, 0f, 0f, 100, default, 2f);
                Main.dust[paper].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[paper].scale = 0.5f;
                    Main.dust[paper].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 15; d++)
            {
                int paper = Dust.NewDust(projectile.position, projectile.width, projectile.height, 30, 0f, 0f, 100, default, 3f);
                Main.dust[paper].noGravity = true;
                Main.dust[paper].velocity *= 5f;
                paper = Dust.NewDust(projectile.position, projectile.width, projectile.height, 30, 0f, 0f, 100, default, 2f);
                Main.dust[paper].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
