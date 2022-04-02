using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class UniversalGenesisStarcaller : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starcaller Shot");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.23f, 0.19f, 0.25f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                if (Main.rand.NextBool(2))
                {
                    int idx = Dust.NewDust(projectile.position, 1, 1, 173, 0f, 0f, 0, default, 0.5f);
                    Main.dust[idx].alpha = projectile.alpha;
                    Main.dust[idx].velocity *= 0f;
                    Main.dust[idx].noGravity = true;
                }
            }

            projectile.rotation = projectile.velocity.ToRotation();

            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item11.WithPitchVariance(0.05f), projectile.Center);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor) => new Color(1f, 1f, 1f, 0f);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            SpawnStars();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
        }

        private void SpawnStars()
        {
            // The patron said 10 blocks, but 10 blocks is like nothing...  I'm sure they won't mind.
            int maxDistance = 480; // 30 blocks
            bool bossFound = false;
            int life = 0;
            Vector2 targetVec = projectile.Center;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (bossFound && !npc.IsABoss())
                    continue;
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                    if (targetDist < (maxDistance + extraDist) && (npc.IsABoss() || npc.life > life))
                    {
                        if (npc.IsABoss())
                            bossFound = true;
                        life = npc.life;
                        targetVec = npc.Center;
                    }
                }
            }
            for (int n = 0; n < 2; n++)
            {
                Projectile star = CalamityUtils.ProjectileRain(targetVec, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<UniversalGenesisStar>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                star.ai[0] = n;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            for (int g = 0; g < 3; g++)
            {
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
        }
    }
}
