using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class RegulusRiotProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RegulusRiot";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regulus Riot");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 15;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 20;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }

            int blueDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100);
            Main.dust[blueDust].noGravity = true;
            Main.dust[blueDust].velocity = Vector2.Zero;
            int orangeDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100);
            Main.dust[orangeDust].noGravity = true;
            Main.dust[orangeDust].velocity = Vector2.Zero;

            projectile.ai[0] += 1f;
            int behaviorInt = 0;
            if (projectile.velocity.Length() <= 8f) //4
            {
                behaviorInt = 1;
            }
            if (behaviorInt == 0)
            {
                projectile.rotation -= 0.104719758f;

                if (projectile.ai[0] >= 30f)
                {
                    projectile.extraUpdates = 2;
                    projectile.velocity *= 0.98f;
                    projectile.rotation -= 0.0174532924f;
                }
                if (projectile.velocity.Length() < 8.2f) //4.1
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= 4f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = 1;
                }
            }
            else if (behaviorInt == 1)
            {
                projectile.rotation -= 0.104719758f;
                Vector2 targetCenter = projectile.Center;
                float homingRange = 300f;
                bool homeIn = false;
                int targetIndex = 0;
                if (projectile.ai[1] == 0f)
                {
                    for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                    {
                        NPC npc = Main.npc[npcIndex];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            Vector2 npcCenter = npc.Center;
                            if (projectile.Distance(npcCenter) < homingRange)
                            {
                                homingRange = projectile.Distance(npcCenter);
                                targetCenter = npcCenter;
                                homeIn = true;
                                targetIndex = npcIndex;
                                break;
                            }
                        }
                    }
                    if (homeIn)
                    {
                        if (projectile.ai[1] != (float)(targetIndex + 1))
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.ai[1] = (float)(targetIndex + 1);
                    }
                    homeIn = false;
                }
                if (projectile.ai[1] != 0f)
                {
                    int npcIndex2 = (int)(projectile.ai[1] - 1f);
                    NPC npc2 = Main.npc[npcIndex2];
                    if (npc2.active && npc2.CanBeChasedBy(projectile, true) && projectile.Distance(npc2.Center) < 1000f)
                    {
                        homeIn = true;
                        targetCenter = Main.npc[npcIndex2].Center;
                    }
                }
                if (!projectile.friendly)
                {
                    homeIn = false;
                }
                if (homeIn)
                {
                    float homeSpeed = 24f;
                    float turnMult = 10f;
                    Vector2 projCenter = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float xDist = targetCenter.X - projCenter.X;
                    float yDist = targetCenter.Y - projCenter.Y;
                    float totalDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                    totalDist = homeSpeed / totalDist;
                    xDist *= totalDist;
                    yDist *= totalDist;
                    projectile.velocity.X = (projectile.velocity.X * (turnMult - 1f) + xDist) / turnMult;
                    projectile.velocity.Y = (projectile.velocity.Y * (turnMult - 1f) + yDist) / turnMult;
                }
            }
            if (projectile.ai[0] >= 180f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int blueDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[blueDust].noGravity = true;
                Main.dust[blueDust].velocity = Vector2.Zero;
            }
            for (int i = 0; i < 10; i++)
            {
                int orangeDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[orangeDust].noGravity = true;
                Main.dust[orangeDust].velocity = Vector2.Zero;
            }
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    float spread = 60f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 6f;
                    double offsetAngle;
                    for (int i = 0; i < 3; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<RegulusEnergy>(), (int)(projectile.damage * 0.4), projectile.knockBack, projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<RegulusEnergy>(), (int)(projectile.damage * 0.4), projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
        }
    }
}
