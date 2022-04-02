using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class LostSoulFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lost Soul");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
            projectile.alpha = 150;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool? CanHitNPC(NPC target) => (projectile.timeLeft < 210 || projectile.ai[0] == 2f) && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            Lighting.AddLight(projectile.Center, 0.25f, 0.25f, 0.25f);
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            if (projectile.ai[0] != 1f && (projectile.timeLeft < 210 || projectile.ai[0] == 2f))
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 600f, 12f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!projectile.Calamity().rogue)
            {
                int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
                int cap = 5;
                int oldDamage = damage;
                if (projectileCount > cap)
                {
                    damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                    if (damage < 1)
                        damage = 1;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int j = 0; j <= 10; j++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default, 1f);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(150, 150, 150, 150);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
