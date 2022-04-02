using CalamityMod.Events;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class ProvidenceCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Crystal");
        }

        public override void SetDefaults()
        {
            projectile.width = 160;
            projectile.height = 160;
            projectile.ignoreWater = true;
            projectile.timeLeft = BossRushEvent.BossRushActive ? 1500 : ((CalamityWorld.death || CalamityWorld.malice) ? 2100 : 3600);
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.dead || NPC.CountNPCS(ModContent.NPCType<Providence>()) < 1)
            {
                projectile.active = false;
                projectile.netUpdate = true;
                return;
            }

            bool dayTime = Main.dayTime && !CalamityWorld.malice;

            projectile.position.X = Main.player[projectile.owner].Center.X - (projectile.width / 2);
            projectile.position.Y = Main.player[projectile.owner].Center.Y - (projectile.height / 2) + Main.player[projectile.owner].gfxOffY - 360f;
            if (Main.player[projectile.owner].gravDir == -1f)
            {
                projectile.position.Y = projectile.position.Y + 400f;
                projectile.rotation = 3.14f;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;
            projectile.velocity = Vector2.Zero;
            projectile.alpha -= 5;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.direction == 0)
            {
                projectile.direction = Main.player[projectile.owner].direction;
            }
            if (projectile.alpha == 0 && Main.rand.NextBool(15))
            {
                Dust dust34 = Main.dust[Dust.NewDust(projectile.Top, 0, 0, 267, 0f, 0f, 100, dayTime ? new Color(255, 200, Main.DiscoB) : new Color(Main.DiscoR, 200, 255), 1f)];
                dust34.velocity.X = 0f;
                dust34.noGravity = true;
                dust34.fadeIn = 1f;
                dust34.position = projectile.Center + Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (4f * Main.rand.NextFloat() + 26f);
                dust34.scale = 0.5f;
            }

            float lifeRatio = projectile.ai[0];

            // Increment timer
            projectile.localAI[0] += 1f;

            // Spawn daytime shards every 300 frames
            // Spawn nighttime shards every 30 frames
            if (projectile.localAI[0] >= (dayTime ? 300f : 30f))
            {
                // Spawn shards every 30 frames at night or at 300 frames during day
                if (projectile.localAI[0] % 30f == 0f || dayTime)
                {
                    Main.PlaySound(SoundID.Item109, projectile.position);
                    projectile.netUpdate = true;
                    if (projectile.owner == Main.myPlayer)
                    {
                        int totalProjectiles = dayTime ? 15 : (projectile.localAI[0] % 60f == 0f ? 15 : 10);
                        float speedX = dayTime ? -21f : -15f;
                        float speedAdjustment = Math.Abs(speedX * 2f / (totalProjectiles - 1));
                        float speedY = -3f;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            float x4 = dayTime ? Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X : Main.rgbToHsl(new Color(Main.DiscoR, 200, 255)).X;
                            float randomSpread = dayTime ? 0f : Main.rand.Next(-150, 151) * 0.01f * (1f - lifeRatio);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speedX + speedAdjustment * i + randomSpread, speedY, ModContent.ProjectileType<ProvidenceCrystalShard>(), projectile.damage, projectile.knockBack, projectile.owner, x4, projectile.whoAmI);
                        }
                    }

                    // Reset timer
                    if (projectile.localAI[0] >= 60f)
                        projectile.localAI[0] = 0f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)(projectile.position.X + projectile.width * 0.5) / 16, (int)((projectile.position.Y + projectile.height * 0.5) / 16.0));
            Vector2 vector59 = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = Main.projectileTexture[projectile.type];
            Rectangle rectangle17 = texture2D34.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color alpha5 = projectile.GetAlpha(color25);
            Vector2 origin11 = rectangle17.Size() / 2f;
            float scaleFactor5 = (float)Math.Cos(MathHelper.TwoPi * (projectile.localAI[0] / 60f)) + 3f + 3f;
            for (float num286 = 0f; num286 < 4f; num286 += 1f)
            {
                double angle = num286 * MathHelper.PiOver2;
                Vector2 center = default;
                Main.spriteBatch.Draw(texture2D34, vector59 + Vector2.UnitY.RotatedBy(angle, center) * scaleFactor5, new Microsoft.Xna.Framework.Rectangle?(rectangle17), alpha5 * 0.2f, projectile.rotation, origin11, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
