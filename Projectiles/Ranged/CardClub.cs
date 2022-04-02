using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardClub : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Club");
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
                int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, 30, 0f, 0f, 0, default, 0.5f);
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
            for (int i = -8; i <= 8; i += 8)
            {
                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CardClubSplit>(), projectile.damage / 2, 0f, projectile.owner, 0f, 0f);
            }
            Main.PlaySound(SoundID.Item110, projectile.Center);
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
