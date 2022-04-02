using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    public class DesertScourgeBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Scourge");
        }

        public override void SetDefaults()
        {
            npc.GetNPCDamage();
            npc.width = 32;
            npc.height = 36;
            npc.defense = 6;
            npc.DR_NERD(0.05f);
            npc.LifeMaxNERB(2500, 3000, 1650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.boss = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("DesertScourge") ?? MusicID.Boss1;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            npc.dontCountMe = true;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                npc.scale = 1.25f;
            else if (CalamityWorld.death)
                npc.scale = 1.2f;
            else if (CalamityWorld.revenge)
                npc.scale = 1.15f;
            else if (Main.expertMode)
                npc.scale = 1.1f;

            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            float burrowTimeGateValue = death ? 420f : 540f;
            bool burrow = Main.npc[(int)npc.ai[2]].Calamity().newAI[0] >= burrowTimeGateValue;

            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest(true);

            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<DesertScourgeHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (npc.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)npc.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
                npc.active = false;
            }

            if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            if (Main.player[npc.target].dead)
                npc.TargetClosest(false);

            if (Main.netMode != NetmodeID.MultiplayerClient && revenge && !burrow)
            {
                npc.localAI[0] += malice ? 6f : Main.rand.Next(4);
                if (npc.localAI[0] >= Main.rand.Next(1400, 26001))
                {
                    npc.localAI[0] = 0f;
                    if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num942 = player.position.X + (float)player.width * 0.5f - vector104.X;
                        float num943 = player.position.Y + (float)player.height * 0.5f - vector104.Y;
                        float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
                        int projectileType = ModContent.ProjectileType<SandBlast>();
                        float num941 = malice ? 9f : 6f;
                        num944 = num941 / num944;
                        num942 *= num944;
                        num943 *= num944;
                        vector104.X += num942 * 5f;
                        vector104.Y += num943 * 5f;
                        if (Main.rand.NextBool(2) || malice)
                        {
                            Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, projectileType, npc.GetProjectileDamage(projectileType), 0f, Main.myPlayer, 0f, 0f);
                        }
                        npc.netUpdate = true;
                    }
                }
            }

            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                }
                catch
                {
                }
                npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = npc.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;

                if (num191 < 0f)
                    npc.spriteDirection = 1;
                else if (num191 > 0f)
                    npc.spriteDirection = -1;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                float randomSpread = (float)(Main.rand.Next(-100, 100) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody3"), npc.scale);
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 120, true);
        }
    }
}
