using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperHomingRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Rocket");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
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

            float colorScale = (float)projectile.alpha / 255f;
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);

            Player player = Main.player[projectile.owner];
            Vector2 center = projectile.Center;
            float maxDistance = 800f;
            bool homeIn = false;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                    if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            else if (Main.npc[(int)projectile.ai[0]].active && projectile.ai[0] != -1f)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                    if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            if (!homeIn)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }
            }

            if (homeIn)
            {
                Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);

                float homingInertia = 20f;
                float homingVelocity = 20f;
                projectile.velocity = (projectile.velocity * homingInertia + moveDirection * homingVelocity) / (homingInertia + 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                }
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale *= 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}
