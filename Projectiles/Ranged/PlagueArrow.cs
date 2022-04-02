using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlagueArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.timeLeft = 600;
            aiType = ProjectileID.WoodenArrowFriendly;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlagueExplosionFriendly>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                int num516 = 6;
                for (int num517 = 0; num517 < num516; num517++)
                {
                    if (num517 % 2 != 1 || Main.rand.NextBool(3))
                    {
                        Vector2 value20 = projectile.position;
                        Vector2 value21 = projectile.oldVelocity;
                        value21.Normalize();
                        value21 *= 8f;
                        float num518 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        float num519 = (float)Main.rand.Next(-35, 36) * 0.01f;
                        value20 -= value21 * (float)num517;
                        num518 += projectile.oldVelocity.X / 6f;
                        num519 += projectile.oldVelocity.Y / 6f;
                        int bee = Projectile.NewProjectile(value20.X, value20.Y, num518, num519, Main.player[projectile.owner].beeType(), Main.player[projectile.owner].beeDamage(projectile.damage / 2), Main.player[projectile.owner].beeKB(0f), Main.myPlayer);
                        if (bee.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[bee].penetrate = 2;
                            Main.projectile[bee].Calamity().forceRanged = true;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
