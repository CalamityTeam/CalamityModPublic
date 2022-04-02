using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Dreadmine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mine");
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            float num945 = 1f;
            float num946 = 1f;
            if (projectile.identity % 6 == 0)
            {
                num946 *= -1f;
            }
            if (projectile.identity % 6 == 1)
            {
                num945 *= -1f;
            }
            if (projectile.identity % 6 == 2)
            {
                num946 *= -1f;
                num945 *= -1f;
            }
            if (projectile.identity % 6 == 3)
            {
                num946 = 0f;
            }
            if (projectile.identity % 6 == 4)
            {
                num945 = 0f;
            }
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 60f)
            {
                projectile.localAI[1] = -180f;
            }
            if (projectile.localAI[1] >= -60f)
            {
                projectile.velocity.X = projectile.velocity.X + 0.002f * num946;
                projectile.velocity.Y = projectile.velocity.Y + 0.002f * num945;
            }
            else
            {
                projectile.velocity.X = projectile.velocity.X - 0.002f * num946;
                projectile.velocity.Y = projectile.velocity.Y - 0.002f * num945;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5400f)
            {
                projectile.ai[1] = 1f;
                if (projectile.ai[0] < 5500f)
                {
                    return;
                }
                if (projectile.owner == Main.myPlayer)
                {
                    projectile.Kill();
                }
            }
            else
            {
                float num947 = (projectile.Center - Main.player[projectile.owner].Center).Length() / 100f;
                if (num947 > 4f)
                {
                    num947 *= 1.1f;
                }
                if (num947 > 5f)
                {
                    num947 *= 1.2f;
                }
                if (num947 > 6f)
                {
                    num947 *= 1.3f;
                }
                if (num947 > 7f)
                {
                    num947 *= 1.4f;
                }
                if (num947 > 8f)
                {
                    num947 *= 1.5f;
                }
                if (num947 > 9f)
                {
                    num947 *= 1.6f;
                }
                if (num947 > 10f)
                {
                    num947 *= 1.7f;
                }
                projectile.ai[0] += num947;
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 25;
                    if (projectile.alpha < 0)
                    {
                        projectile.alpha = 0;
                    }
                }
            }
            bool flag49 = false;
            Vector2 center12 = new Vector2(0f, 0f);
            float num948 = 600f;
            if (Main.player[projectile.owner].HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[Main.player[projectile.owner].MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float num950 = npc.position.X + (float)(npc.width / 2);
                    float num951 = npc.position.Y + (float)(npc.height / 2);
                    float num952 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num950) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num951);
                    if (num952 < num948)
                    {
                        num948 = num952;
                        center12 = npc.Center;
                        flag49 = true;
                    }
                }
            }
            if (!flag49)
            {
                for (int num949 = 0; num949 < Main.npc.Length; num949++)
                {
                    if (Main.npc[num949].CanBeChasedBy(projectile, false))
                    {
                        float num950 = Main.npc[num949].position.X + (float)(Main.npc[num949].width / 2);
                        float num951 = Main.npc[num949].position.Y + (float)(Main.npc[num949].height / 2);
                        float num952 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num950) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num951);
                        if (num952 < num948)
                        {
                            num948 = num952;
                            center12 = Main.npc[num949].Center;
                            flag49 = true;
                        }
                    }
                }
            }
            if (flag49)
            {
                Vector2 vector101 = center12 - projectile.Center;
                vector101.Normalize();
                vector101 *= 0.75f;
                projectile.velocity = (projectile.velocity * 10f + vector101) / 10.8f; //11
                return;
            }
            if ((double)projectile.velocity.Length() > 0.2)
            {
                projectile.velocity *= 0.98f;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 112;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }
    }
}
