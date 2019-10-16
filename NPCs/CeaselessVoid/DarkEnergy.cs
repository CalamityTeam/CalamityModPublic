using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class DarkEnergy : ModNPC
    {
        public int invinceTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.dontTakeDamage = true;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 50;
            npc.lifeMax = 6000;
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                npc.lifeMax = 24000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 44000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.knockBackResist = 0.25f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            if (invinceTime > 0)
            {
                invinceTime--;
            }
            else
            {
                npc.damage = expertMode ? 240 : 120;
                if (CalamityWorld.revenge)
                    npc.damage = 300;
                npc.dontTakeDamage = false;
            }
            Vector2 vectorCenter = npc.Center;
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            double mult = 0.5 +
                (CalamityWorld.revenge ? 0.2 : 0.0) +
                (CalamityWorld.death ? 0.2 : 0.0);
            if ((double)npc.life < (double)npc.lifeMax * mult || CalamityWorld.bossRushActive)
            {
                npc.knockBackResist = 0f;
            }

            if (npc.ai[1] == 0f)
            {
                npc.scale -= 0.01f;
                npc.alpha += 15;
                if (npc.alpha >= 125)
                {
                    npc.alpha = 130;
                    npc.ai[1] = 1f;
                }
            }
            else if (npc.ai[1] == 1f)
            {
                npc.scale += 0.01f;
                npc.alpha -= 15;
                if (npc.alpha <= 0)
                {
                    npc.alpha = 0;
                    npc.ai[1] = 0f;
                }
            }
            if (!player.active || player.dead || CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 2400)
            {
                npc.timeLeft = 2400;
            }
            if (npc.ai[0] == 0f)
            {
                Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                float num784 = Main.npc[CalamityGlobalNPC.voidBoss].Center.X - vector96.X;
                float num785 = Main.npc[CalamityGlobalNPC.voidBoss].Center.Y - vector96.Y;
                float num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
                if (num786 > 90f)
                {
                    num786 = (CalamityWorld.bossRushActive ? 24f : 16f) / num786;
                    num784 *= num786;
                    num785 *= num786;
                    npc.velocity.X = (npc.velocity.X * 15f + num784) / 16f;
                    npc.velocity.Y = (npc.velocity.Y * 15f + num785) / 16f;
                    return;
                }
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 16f)
                {
                    npc.velocity.Y = npc.velocity.Y * 1.1f;
                    npc.velocity.X = npc.velocity.X * 1.1f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && ((expertMode && Main.rand.NextBool(50)) || Main.rand.NextBool(100)))
                {
                    npc.TargetClosest(true);
                    vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                    num784 = player.Center.X - vector96.X;
                    num785 = player.Center.Y - vector96.Y;
                    num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
                    num786 = (CalamityWorld.bossRushActive ? 16f : 12f) / num786;
                    npc.velocity.X = num784 * num786;
                    npc.velocity.Y = num785 * num786;
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
            }
            else
            {
                Vector2 value4 = player.Center - npc.Center;
                value4.Normalize();
                value4 *= CalamityWorld.bossRushActive ? 16f : 11f;
                npc.velocity = (npc.velocity * 99f + value4) / 100f;
                Vector2 vector97 = new Vector2(npc.Center.X, npc.Center.Y);
                float num787 = Main.npc[CalamityGlobalNPC.voidBoss].Center.X - vector97.X;
                float num788 = Main.npc[CalamityGlobalNPC.voidBoss].Center.Y - vector97.Y;
                float num789 = (float)Math.Sqrt((double)(num787 * num787 + num788 * num788));
                npc.ai[2] += 1f;
                if (num789 > 700f || npc.ai[2] >= 150f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[2] = 0f;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 300, true);
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
