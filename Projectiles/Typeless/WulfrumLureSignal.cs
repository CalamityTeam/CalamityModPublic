using CalamityMod.NPCs.AstrumDeus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Items.Tools;

namespace CalamityMod.Projectiles.Typeless
{
    public class WulfrumLureSignal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

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

            if (Time % WulfrumLureItem.SpawnIntervals == 0)
            {
                WulfrumLureItem.MaxEnemiesPerWave = 5;
                int enemiesToSpawn = Main.rand.Next(1, WulfrumLureItem.MaxEnemiesPerWave);

                Player player = Main.LocalPlayer;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    float closestPlayerDistance = (Main.LocalPlayer.Center - Projectile.Center).Length();
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        float newDistance = (Main.player[i].Center - Projectile.Center).Length();
                        if (newDistance < closestPlayerDistance)
                        {
                            closestPlayerDistance = newDistance;
                            player = Main.player[i];
                        }
                    }
                }

                if ((player.Center - Projectile.Center).Length() > 3500)
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
                        int npcToSpawn = Main.rand.NextBool() ? ModContent.NPCType<WulfrumDrone>() : Main.rand.NextBool() ? ModContent.NPCType<WulfrumHovercraft>() : ModContent.NPCType<WulfrumGyrator>() ;
                        int index = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, npcToSpawn, Target:player.whoAmI);

                        for (int iy = 0; iy < 16; iy++)
                        {
                            Dust zapDust = Dust.NewDustPerfect(spawnPosition + Main.rand.NextVector2Circular(1f, 1f) * 20f, 226, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(1f, 2.3f) - Vector2.UnitY * 6f);
                            zapDust.noGravity = true;
                        }
                    }
                }
            }

            if (Time%2 == 0 && CalamityUtils.IntoMorseCode("perimeter breached", Time / WulfrumLureItem.SignalTime))
            {
                float dustCount = MathHelper.TwoPi * 300 / 8f;
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / dustCount;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 229);
                    dust.position = Projectile.Center + angle.ToRotationVector2() * 300;
                    dust.scale = 0.7f;
                    dust.noGravity = true;
                    dust.velocity = Projectile.velocity;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(WulfrumTreasurePinger.RechargeBeepSound, Projectile.Center);
        }
    }
}
