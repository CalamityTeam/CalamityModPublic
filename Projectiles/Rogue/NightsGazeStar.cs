
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NightsGazeStar : ModProjectile
    {
        public static int lifetime = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Gaze Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = lifetime;
            projectile.Calamity().rogue = true;
            projectile.localAI[0] = 10f;
        }

        public override void AI()
        {
            projectile.rotation += projectile.direction * 0.05f;

            if (projectile.ai[0] == 0f)
            {
                if (projectile.timeLeft < (lifetime - projectile.ai[1]) && projectile.localAI[0] >= 0)
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= projectile.localAI[0];
                    projectile.localAI[0]--;
                }
                else if (projectile.timeLeft >= (lifetime - projectile.ai[1]))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        float dustVelocity = Main.rand.NextFloat(0f, 0.5f);
                        int dustType = Utils.SelectRandom(Main.rand, new int[]
                        {
                            109,
                            111,
                            132
                        });

                        int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= dustVelocity;
                    }
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                if (minDist < 999f)
                {
                    Vector2 velocityNew = Main.npc[index].Center - projectile.Center;
                    float speed = 10f;
                    velocityNew.Normalize();
                    projectile.velocity = velocityNew * speed;
                }
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item9, projectile.position);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], Color.White, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    109,
                    111,
                    132
                });

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
