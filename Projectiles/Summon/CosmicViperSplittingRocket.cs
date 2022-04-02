using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperSplittingRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Splitting Rocket");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float colorScale = (float)projectile.alpha / 255f;
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);
            Vector2 center = projectile.Center;
            float maxDistance = 800f;
            float explode = 16f;
            bool homeIn = false;
            Player player = Main.player[projectile.owner];

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                    if (Vector2.Distance(npc.Center, projectile.Center) < (explode + extraDistance))
                    {
                        int numProj = 4;
                        if (projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                while (speed.X == 0f && speed.Y == 0f)
                                {
                                    speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                }
                                speed.Normalize();
                                speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
                                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<CosmicViperSplitRocket1>(), (int)(projectile.damage * 0.25), projectile.knockBack, projectile.owner, 0f, 0f);
                            }
                        }
                        Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
                        projectile.Kill();
                        return;
                    }
                    else if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
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

                    if (Vector2.Distance(npc.Center, projectile.Center) < (explode + extraDistance))
                    {
                        int numProj = 4;
                        if (projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                while (speed.X == 0f && speed.Y == 0f)
                                {
                                    speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                }
                                speed.Normalize();
                                speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
                                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<CosmicViperSplitRocket1>(), (int)(projectile.damage * 0.25), projectile.knockBack, projectile.owner, 0f, 0f);
                            }
                        }
                        Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
                        projectile.Kill();
                        return;
                    }
                    else if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            if (!homeIn)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                        if (Vector2.Distance(npc.Center, projectile.Center) < (explode + extraDistance))
                        {
                            int numProj = 4;
                            if (projectile.owner == Main.myPlayer)
                            {
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                    while (speed.X == 0f && speed.Y == 0f)
                                    {
                                        speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                    }
                                    speed.Normalize();
                                    speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
                                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<CosmicViperSplitRocket1>(), (int)(projectile.damage * 0.25f), projectile.knockBack, projectile.owner, Main.rand.Next(2), 0f);
                                }
                            }
                            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
                            projectile.Kill();
                            return;
                        }
                        else if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = npc.Center;
                            homeIn = true;
                        }
                    }
                }
            }
            if (homeIn)
            {
                Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);

                float homingInertia = 15f;
                float homingVelocity = 30f;
                projectile.velocity = (projectile.velocity * homingInertia + moveDirection * homingVelocity) / (homingInertia + 1f);
            }
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

        public override void Kill(int timeLeft)
        {
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
