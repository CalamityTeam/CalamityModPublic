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
    public class ProvidenceCrystal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
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
            if (CalamityGlobalNPC.holyBoss < 0 || !Main.npc[CalamityGlobalNPC.holyBoss].active)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
                return;
            }

            Player proviTarget = Main.player[Main.npc[CalamityGlobalNPC.holyBoss].target];

            Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];

            Projectile.position.X = proviTarget.Center.X - (Projectile.width / 2);
            Projectile.position.Y = proviTarget.Center.Y - (Projectile.height / 2) + proviTarget.gfxOffY - 360f;
            if (proviTarget.gravDir == -1f)
            {
                Projectile.position.Y += 400f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
                Projectile.rotation = 0f;

            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            Projectile.velocity = Vector2.Zero;

            Projectile.alpha -= 5;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.alpha == 0 && Main.rand.NextBool(15))
            {
                Color BaseColor = ProvUtils.GetProjectileColor(Projectile.maxPenetrate, 0);
                float Brightness = 0.8f;
                Color DustColor = Color.Lerp(BaseColor, Color.White, Brightness);
                Dust crystalDust = Main.dust[Dust.NewDust(Projectile.Top, 0, 0, 267, 0f, 0f, 100, DustColor, 1f)];
                crystalDust.velocity.X = 0f;
                crystalDust.noGravity = true;
                crystalDust.fadeIn = 1f;
                crystalDust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * (4f * Main.rand.NextFloat() + 26f);
                crystalDust.scale = 0.5f;
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
                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
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
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speedX + speedAdjustment * i + randomSpread, speedY, ModContent.ProjectileType<ProvidenceCrystalShard>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, x4, Projectile.whoAmI);
                        }
                    }

                    // Reset timer
                    if (Projectile.localAI[0] >= 60f)
                        Projectile.localAI[0] = 0f;
                }
            }
        }

        public override bool CanHitPlayer(Player target) => false;

        public override Color? GetAlpha(Color lightColor) => new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Color colorArea = Lighting.GetColor((int)(Projectile.position.X + Projectile.width * 0.5) / 16, (int)((Projectile.position.Y + Projectile.height * 0.5) / 16.0));
            Vector2 drawArea = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D34 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle textureRect = texture2D34.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color colorAlpha = Projectile.GetAlpha(colorArea);
            Vector2 halfRect = textureRect.Size() / 2f;
            float scaleFactor = (float)Math.Cos(MathHelper.TwoPi * (Projectile.localAI[0] / 60f)) + 3f + 3f;
            for (float i = 0f; i < 4f; i += 1f)
            {
                double angle = i * MathHelper.PiOver2;
                Vector2 center = default;
                Main.spriteBatch.Draw(texture2D34, drawArea + Vector2.UnitY.RotatedBy(angle, center) * scaleFactor, new Microsoft.Xna.Framework.Rectangle?(textureRect), colorAlpha * 0.2f, Projectile.rotation, halfRect, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
