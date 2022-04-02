using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGBeamPortal : ModProjectile
    {
        public bool start = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam Portal");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(start);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            start = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                projectile.active = false;
                projectile.netUpdate = true;
                return;
            }

            Player player = Main.player[projectile.owner];

            if (start)
            {
                Main.PlaySound(SoundID.Item92, (int)projectile.Center.X, (int)projectile.Center.Y);

                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 1.2f);
                    Main.dust[num622].velocity *= 3f;
                    Main.dust[num622].noGravity = true;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 1.7f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 1f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 2f;
                }

                projectile.ai[1] = projectile.ai[0];
                start = false;
            }

            // Difficulty modes
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            Lighting.AddLight(projectile.Center, 0f, 0.95f, 1.15f);

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
                projectile.frame = 0;

            double deg = projectile.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = malice ? 800D : death ? 960D : revenge ? 1000D : expertMode ? 1040D : 1080D;
            projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;
            projectile.ai[1] += 1f;

            // Don't fire projectiles if Ceaseless Void is entering a new phase
            if (projectile.timeLeft > 30)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= 240f)
                {
                    projectile.localAI[0] = 0f;
                    if (projectile.owner == Main.myPlayer)
                    {
                        Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
                        float speed = death ? 5f : revenge ? 4.5f : expertMode ? 4f : 3f;
                        speed *= Main.npc[CalamityGlobalNPC.voidBoss].ai[1];
                        int totalProjectiles = 3;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 velocity = new Vector2(0f, -speed).RotatedBy(radians * i);
                            Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<DoGBeam>(), 0, 0f, projectile.owner, projectile.damage, 0f);
                        }
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target) => false;

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 30)
            {
                byte b2 = (byte)(projectile.timeLeft * 8.5);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
