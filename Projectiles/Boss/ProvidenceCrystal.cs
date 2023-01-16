using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = BossRushEvent.BossRushActive ? 1500 : CalamityWorld.death ? 2100 : 3600;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || NPC.CountNPCS(ModContent.NPCType<Providence>()) < 1)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
                return;
            }

            //Syncing to Night AI
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }

            Projectile.position.X = Main.player[Projectile.owner].Center.X - (Projectile.width / 2);
            Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (Projectile.height / 2) + Main.player[Projectile.owner].gfxOffY - 360f;
            if (Main.player[Projectile.owner].gravDir == -1f)
            {
                Projectile.position.Y = Projectile.position.Y + 400f;
                Projectile.rotation = 3.14f;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            Projectile.velocity = Vector2.Zero;
            Projectile.alpha -= 5;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.direction == 0)
            {
                Projectile.direction = Main.player[Projectile.owner].direction;
            }
            if (Projectile.alpha == 0 && Main.rand.NextBool(15))
            {
                Color BaseColor = ProvUtils.GetProjectileColor(Projectile.maxPenetrate, 0);
                float Brightness = 0.8f;
                Color DustColor = Color.Lerp(BaseColor, Color.White, Brightness);
                Dust dust34 = Main.dust[Dust.NewDust(Projectile.Top, 0, 0, 267, 0f, 0f, 100, DustColor, 1f)];
                dust34.velocity.X = 0f;
                dust34.noGravity = true;
                dust34.fadeIn = 1f;
                dust34.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (4f * Main.rand.NextFloat() + 26f);
                dust34.scale = 0.5f;
            }

            float lifeRatio = Projectile.ai[0];

            // Increment timer
            Projectile.localAI[0] += 1f;

            // At day, fires every 300 frames but lasts 1500-3600 frames.
            // At night, fires every 30 framges but lasts 210 frames.
            // In GFB, fires at night rate for the first 210 frames, then at day rate for the next 1500.
            bool dayAI = Projectile.maxPenetrate == (int)Providence.BossMode.Day || (Projectile.maxPenetrate >= (int)Providence.BossMode.Red && Projectile.timeLeft < 1500);

            if (Projectile.localAI[0] >= (dayAI ? 300f : 30f))
            {
                // Spawn shards every 30 frames at night or at 300 frames during day
                if (Projectile.localAI[0] % 30f == 0f || dayAI)
                {
                    SoundEngine.PlaySound(SoundID.Item109, Projectile.position);
                    Projectile.netUpdate = true;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        int totalProjectiles = dayAI ? 15 : (Projectile.localAI[0] % 60f == 0f ? 15 : 10);
                        float speedX = dayAI ? -21f : -15f;
                        float speedAdjustment = Math.Abs(speedX * 2f / (totalProjectiles - 1));
                        float speedY = -3f;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            float x4 = dayAI ? Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X : Main.rgbToHsl(new Color(Main.DiscoR, 200, 255)).X;
                            float randomSpread = dayAI ? 0f : Main.rand.Next(-150, 151) * 0.01f * (1f - lifeRatio);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speedX + speedAdjustment * i + randomSpread, speedY, ModContent.ProjectileType<ProvidenceCrystalShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner, x4, Projectile.whoAmI);
                        }
                    }

                    // Reset timer
                    if (Projectile.localAI[0] >= 60f)
                        Projectile.localAI[0] = 0f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)(Projectile.position.X + Projectile.width * 0.5) / 16, (int)((Projectile.position.Y + Projectile.height * 0.5) / 16.0));
            Vector2 vector59 = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangle17 = texture2D34.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color alpha5 = Projectile.GetAlpha(color25);
            Vector2 origin11 = rectangle17.Size() / 2f;
            float scaleFactor5 = (float)Math.Cos(MathHelper.TwoPi * (Projectile.localAI[0] / 60f)) + 3f + 3f;
            for (float num286 = 0f; num286 < 4f; num286 += 1f)
            {
                double angle = num286 * MathHelper.PiOver2;
                Vector2 center = default;
                Main.spriteBatch.Draw(texture2D34, vector59 + Vector2.UnitY.RotatedBy(angle, center) * scaleFactor5, new Microsoft.Xna.Framework.Rectangle?(rectangle17), alpha5 * 0.2f, Projectile.rotation, origin11, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
