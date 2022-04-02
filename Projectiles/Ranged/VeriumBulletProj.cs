using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VeriumBulletProj : ModProjectile
    {
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Verium Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override bool PreAI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                if (Main.rand.NextBool(2))
                {
                    int purple = Dust.NewDust(projectile.position, 1, 1, 70, 0f, 0f, 0, default, 0.5f);
                    Main.dust[purple].alpha = projectile.alpha;
                    Main.dust[purple].velocity *= 0f;
                    Main.dust[purple].noGravity = true;
                }
            }

            if (projectile.ai[0] > 0f)
                projectile.ai[0]--;
            if (speed == 0f)
                speed = projectile.velocity.Length();
            if (projectile.penetrate == 1 && projectile.ai[0] <= 0f)
            {
                float inertia = 15f;
                Vector2 center = projectile.Center;
                float maxDistance = 300f;
                bool homeIn = false;

                int targetIndex = (int)projectile.ai[1];
                NPC target = Main.npc[targetIndex];
                if (target.CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (target.width / 2) + (target.height / 2);

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, target.Center, 1, 1);

                    if (Vector2.Distance(target.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = target.Center;
                        homeIn = true;
                    }
                }

                if (!homeIn)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float extraDistance = (npc.width / 2) + (npc.height / 2);

                            bool canHit = true;
                            if (extraDistance < maxDistance)
                                canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                            if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                            {
                                center = npc.Center;
                                homeIn = true;
                                break;
                            }
                        }
                    }
                }

                if (!projectile.friendly)
                {
                    homeIn = false;
                }

                if (homeIn)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * inertia + moveDirection * speed) / (inertia + 1f);
                }
                return false;
            }
            return true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.ai[0] <= 0f && target.CanBeChasedBy(projectile);

        public override bool CanHitPvp(Player target) => projectile.ai[0] <= 0f;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.ai[0] = 10f;
            projectile.damage /= 2;
            if (target.life > 0)
                projectile.ai[1] = target.whoAmI;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.ai[0] = 10f;
            projectile.damage /= 2;
        }
    }
}
