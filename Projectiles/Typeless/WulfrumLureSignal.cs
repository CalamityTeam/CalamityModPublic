using System.Collections.Generic;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Tools;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Packets.Entities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumLureSignal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static List<int> LureSpawnPool => new List<int>()
        {
            ModContent.NPCType<WulfrumDrone>(),
            ModContent.NPCType<WulfrumGyrator>(),
            ModContent.NPCType<WulfrumHovercraft>(),
            ModContent.NPCType<WulfrumRover>()
        };

        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = WulfrumLureItem.SignalTime;
        }

        public override void AI()
        {
            Time++;

            // Only the server (or singleplayer) may spawn NPCs. Clients spawning them would create local-only enemies the server never knows about.
            if (Time % WulfrumLureItem.SpawnIntervals == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int enemiesToSpawn = Main.rand.Next(1, WulfrumLureItem.MaxEnemiesPerWave);

                Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];

                if (!player.active || (player.Center - Projectile.Center).Length() > 3500)
                    return;

                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    int tries = 0;
                    Vector2 spawnPosition;
                    do
                    {
                        Vector2 displacey = Main.rand.NextVector2Unit();
                        if (displacey.Y > 0)
                            displacey.Y *= -1;
                        spawnPosition = player.Center + displacey * Main.rand.NextFloat(600f, 1015f) * new Vector2(1.5f, 1f);
                        if (spawnPosition.Y > player.Center.Y)
                            spawnPosition.Y = player.Center.Y;
                        if (tries > 500)
                            break;
                        tries++;
                    }
                    while (WorldGen.SolidTile(CalamityUtils.ParanoidTileRetrieval((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16)));

                    if (tries < 500)
                    {
                        int npcToSpawn = LureSpawnPool[Main.rand.Next(LureSpawnPool.Count)];
                        int index = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, npcToSpawn, Target: player.whoAmI);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            if (index < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: index);
                            WulfrumLureSpawnSparksPacket.Send(spawnPosition);
                        }
                        else
                            SpawnArrivalSparks(spawnPosition);
                    }
                }
            }

            if (Time % 2 == 0 && CalamityUtils.IntoMorseCode("perimeter breached", Time / WulfrumLureItem.SignalTime))
            {
                float dustCount = MathHelper.TwoPi * 300 / 8f;
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / dustCount;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Vortex);
                    dust.position = Projectile.Center + angle.ToRotationVector2() * 300;
                    dust.scale = 0.7f;
                    dust.noGravity = true;
                    dust.velocity = Projectile.velocity;
                }
            }
        }

        public static void SpawnArrivalSparks(Vector2 position)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust zapDust = Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(1f, 1f) * 20f, DustID.Electric, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(1f, 2.3f) - Vector2.UnitY * 6f);
                zapDust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(WulfrumTreasurePinger.RechargeBeepSound, Projectile.Center);
        }
    }
}
