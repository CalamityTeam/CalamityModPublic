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
            this.HideFromBestiary();
            DisplayName.SetDefault("Desert Scourge");
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.width = 32;
            NPC.height = 36;
            NPC.defense = 6;
            NPC.DR_NERD(0.05f);
            NPC.LifeMaxNERB(2500, 3000, 1650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.boss = true;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("DesertScourge") ?? MusicID.Boss1;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                NPC.scale = 1.25f;
            else if (CalamityWorld.death)
                NPC.scale = 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale = 1.15f;
            else if (Main.expertMode)
                NPC.scale = 1.1f;

            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            float burrowTimeGateValue = death ? 420f : 540f;
            bool burrow = Main.npc[(int)NPC.ai[2]].Calamity().newAI[0] >= burrowTimeGateValue;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

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
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            if (Main.netMode != NetmodeID.MultiplayerClient && revenge && !burrow)
            {
                NPC.localAI[0] += malice ? 6f : Main.rand.Next(4);
                if (NPC.localAI[0] >= Main.rand.Next(1400, 26001))
                {
                    NPC.localAI[0] = 0f;
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                    {
                        Vector2 vector104 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
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
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), vector104.X, vector104.Y, num942, num943, projectileType, NPC.GetProjectileDamage(projectileType), 0f, Main.myPlayer, 0f, 0f);
                        }
                        NPC.netUpdate = true;
                    }
                }
            }

            Vector2 vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = NPC.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;

                if (num191 < 0f)
                    NPC.spriteDirection = 1;
                else if (num191 > 0f)
                    NPC.spriteDirection = -1;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = (float)(Main.rand.Next(-100, 100) / 100);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("ScourgeBody").Type, NPC.scale);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("ScourgeBody2").Type, NPC.scale);
                    Gore.NewGore(NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("ScourgeBody3").Type, NPC.scale);
                }
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 120, true);
        }
    }
}
