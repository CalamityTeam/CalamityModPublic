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
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGBeamPortal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public bool start = true;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
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
                Projectile.active = false;
                Projectile.netUpdate = true;
                return;
            }

            Player player = Main.player[Main.npc[CalamityGlobalNPC.voidBoss].target];

            if (start)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

                for (int i = 0; i < 15; i++)
                {
                    int ectoDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 1.2f);
                    Main.dust[ectoDust].velocity *= 3f;
                    Main.dust[ectoDust].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[ectoDust].scale = 0.5f;
                        Main.dust[ectoDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    int ectoDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 1.7f);
                    Main.dust[ectoDust2].noGravity = true;
                    Main.dust[ectoDust2].velocity *= 5f;
                    ectoDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 1f);
                    Main.dust[ectoDust2].noGravity = true;
                    Main.dust[ectoDust2].velocity *= 2f;
                }

                Projectile.ai[1] = Projectile.ai[0];
                start = false;
            }

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            Lighting.AddLight(Projectile.Center, 0f, 0.95f, 1.15f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
                Projectile.frame = 0;

            double deg = Projectile.ai[1];
            double rad = deg * (Math.PI / 180);
            double dist = bossRush ? 360D : death ? 400D : revenge ? 420D : expertMode ? 440D : 480D;
            Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            Projectile.ai[1] += 1f;

            // Don't fire projectiles if Ceaseless Void is entering a new phase
            if (Projectile.timeLeft > 30)
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= 240f)
                {
                    Projectile.localAI[0] = 0f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        SoundEngine.PlaySound(SoundID.Item33, Projectile.Center);
                        float speed = death ? 5f : revenge ? 4.5f : expertMode ? 4f : 3f;
                        speed *= Main.npc[CalamityGlobalNPC.voidBoss].ai[1];
                        int totalProjectiles = 3;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 velocity = new Vector2(0f, -speed).RotatedBy(radians * i);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DoGBeam>(), 0, 0f, Main.myPlayer, Projectile.damage, 0f);
                        }
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target) => false;

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 30)
            {
                byte b2 = (byte)(Projectile.timeLeft * 8.5);
                byte a2 = (byte)(100f * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, 100);
        }
    }
}
