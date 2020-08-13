using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveBlob : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Blob");
            //Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.1f;
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.width = 25;
            npc.height = 25;
            npc.lifeMax = 75;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 13000;
            }
            aiType = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.chaseable = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode;
            bool revenge = CalamityWorld.revenge;
            if (CalamityGlobalNPC.hiveMind < 0 || !Main.npc[CalamityGlobalNPC.hiveMind].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            int num750 = CalamityGlobalNPC.hiveMind;
            if (npc.ai[3] > 0f)
            {
                num750 = (int)npc.ai[3] - 1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] -= 1f;
                if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = (float)Main.rand.Next(120, 480);
                    npc.ai[0] = (float)Main.rand.Next(-100, 101);
                    npc.ai[1] = (float)Main.rand.Next(-100, 101);
                    npc.netUpdate = true;
                }
            }
            npc.TargetClosest(true);
            float num751 = 0.01f;
            float num752 = 400f;
            if ((double)Main.npc[CalamityGlobalNPC.hiveMind].life < (double)Main.npc[CalamityGlobalNPC.hiveMind].lifeMax * 0.5)
            {
                num752 += 60f;
            }
            if ((double)Main.npc[CalamityGlobalNPC.hiveMind].life < (double)Main.npc[CalamityGlobalNPC.hiveMind].lifeMax * 0.15)
            {
                num752 += 120f;
            }
            if (expertMode)
            {
                float num753 = 1f - (float)npc.life / (float)npc.lifeMax;
                num752 += num753 * 100f;
                num751 += 0.02f;
            }
            if (revenge)
            {
                num751 += 0.1f;
            }
            if (!Main.npc[num750].active || CalamityGlobalNPC.hiveMind < 0)
            {
                npc.active = false;
                return;
            }
            Vector2 vector22 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
            float num189 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - (float)(npc.width / 2) - vector22.X;
            float num190 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - (float)(npc.height / 2) - vector22.Y;
            float num191 = (float)Math.Sqrt((double)(num189 * num189 + num190 * num190));
            float num754 = Main.npc[num750].position.X + (float)(Main.npc[num750].width / 2);
            float num755 = Main.npc[num750].position.Y + (float)(Main.npc[num750].height / 2);
            Vector2 vector93 = new Vector2(num754, num755);
            float num756 = num754 + npc.ai[0];
            float num757 = num755 + npc.ai[1];
            float num758 = num756 - vector93.X;
            float num759 = num757 - vector93.Y;
            float num760 = (float)Math.Sqrt((double)(num758 * num758 + num759 * num759));
            num760 = num752 / num760;
            num758 *= num760;
            num759 *= num760;
            if (npc.position.X < num754 + num758)
            {
                npc.velocity.X = npc.velocity.X + num751;
                if (npc.velocity.X < 0f && num758 > 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
            }
            else if (npc.position.X > num754 + num758)
            {
                npc.velocity.X = npc.velocity.X - num751;
                if (npc.velocity.X > 0f && num758 < 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
            }
            if (npc.position.Y < num755 + num759)
            {
                npc.velocity.Y = npc.velocity.Y + num751;
                if (npc.velocity.Y < 0f && num759 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.9f;
                }
            }
            else if (npc.position.Y > num755 + num759)
            {
                npc.velocity.Y = npc.velocity.Y - num751;
                if (npc.velocity.Y > 0f && num759 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.9f;
                }
            }
            if (npc.velocity.X > 4f)
            {
                npc.velocity.X = 4f;
            }
            if (npc.velocity.X < -4f)
            {
                npc.velocity.X = -4f;
            }
            if (npc.velocity.Y > 4f)
            {
                npc.velocity.Y = 4f;
            }
            if (npc.velocity.Y < -4f)
            {
                npc.velocity.Y = -4f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.localAI[1] = 180f;
                }
                npc.localAI[1] += 1f;
                if (npc.localAI[1] >= 600f)
                {
                    npc.localAI[1] = 0f;
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num941 = revenge ? 6f : 5f; //speed
                        if (CalamityWorld.death || BossRushEvent.BossRushActive)
                        {
                            num941 = 7f;
                        }
                        Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X;
                        float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y;
                        float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
                        num944 = num941 / num944;
                        num942 *= num944;
                        num943 *= num944;
                        int num945 = expertMode ? 12 : 15;
                        int num946 = ModContent.ProjectileType<VileClot>();
                        vector104.X += num942 * 5f;
                        vector104.Y += num943 * 5f;
                        int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
                        npc.netUpdate = true;
                    }
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        /*public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }*/

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
