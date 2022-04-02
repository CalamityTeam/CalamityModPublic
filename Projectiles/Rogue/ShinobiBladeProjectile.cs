using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShinobiBladeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShinobiBlade";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shinobi Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * projectile.spriteDirection;

            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0)
                CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], 10, ModContent.ProjectileType<ShinobiHealOrb>(), 1200f, 0f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (target.statLife <= 0)
                CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], 10, ModContent.ProjectileType<ShinobiHealOrb>(), 1200f, 0f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = 42;
                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X / 2, projectile.velocity.Y / 2, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
