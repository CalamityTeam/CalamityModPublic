using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.CeaselessVoid
{
    public class DarkEnergy : ModNPC
    {
        private int invinceTime = 180;
        private bool start = true;
        private const double minDistance = 10D;
        private double distance = minDistance;
        private const double minMaxDistance = 800D;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.npcFrameCount[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.aiStyle = -1;
            aiType = -1;
            npc.GetNPCDamage();
            npc.dontTakeDamage = true;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 50;
            npc.lifeMax = 3000;
            if (CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel1)
            {
                npc.lifeMax = 12000;
            }
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 4400;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(start);
            writer.Write(distance);
            writer.Write(npc.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            start = reader.ReadBoolean();
            distance = reader.ReadDouble();
            npc.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();
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
            // Set the degrees used for rotation
            if (start)
            {
                start = false;
                npc.ai[3] = npc.ai[0];
            }

            // Stay invincible for 3 seconds to avoid being instantly killed
            if (invinceTime > 0)
            {
                npc.damage = 0;
                invinceTime--;
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.dontTakeDamage = false;
            }

            // Force despawn if Ceaseless Void isn't active
            if (CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            // Set time left just in case something dumb happens with despawning
            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            // Difficulty modes
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Gets how enraged Ceaseless Void is
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.voidBoss].ai[1];

            // Distance from Ceaseless Void
            double maxDistance = malice ? 1200D : death ? 1040D : revenge ? 960D : expertMode ? 880D : minMaxDistance;
            double rateOfChangeIncrease = (maxDistance / minMaxDistance) - 1D;
            double rateOfChange = (npc.ai[1] * 0.5f) + 2D + (tileEnrageMult - 1f) + rateOfChangeIncrease;
            if (npc.Calamity().newAI[0] == 0f)
            {
                distance += rateOfChange;
                if (distance >= maxDistance)
                {
                    distance = maxDistance;
                    npc.Calamity().newAI[0] = 1f;
                }
            }
            else
            {
                distance -= rateOfChange;
                if (distance <= minDistance)
                {
                    distance = minDistance;
                    npc.Calamity().newAI[0] = 0f;
                }
            }

            // Rotation velocity
            float minRotationVelocity = 0.5f + tileEnrageMult - 1f;
            float rotationVelocityIncrease = death ? 0.2f : revenge ? 0.15f : expertMode ? 0.1f : 0f;
            rotationVelocityIncrease += rotationVelocityIncrease * (npc.ai[1] * 0.5f);

            // Rotate around Ceaseless Void
            NPC parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<CeaselessVoid>())];
            double degrees = npc.ai[3];
            double radians = degrees * (Math.PI / 180);
            npc.position.X = parent.Center.X - (int)(Math.Cos(radians) * distance) - npc.width / 2;
            npc.position.Y = parent.Center.Y - (int)(Math.Sin(radians) * distance) - npc.height / 2;
            npc.ai[3] += minRotationVelocity + rotationVelocityIncrease;

            // Flash and pulse effect
            if (npc.ai[2] == 0f)
            {
                npc.scale -= 0.01f;
                npc.alpha += 15;
                if (npc.alpha >= 125)
                {
                    npc.alpha = 130;
                    npc.ai[2] = 1f;
                }
            }
            else
            {
                npc.scale += 0.01f;
                npc.alpha -= 15;
                if (npc.alpha <= 0)
                {
                    npc.alpha = 0;
                    npc.ai[2] = 0f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow2");
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = npc.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow");
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
            Color color42 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = npc.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                    vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                    Color color43 = color42;
                    color43 = Color.Lerp(color43, color36, amount9);
                    color43 = npc.GetAlpha(color43);
                    color43 *= (num153 - num163) / 15f;
                    spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.VortexDebuff, 20, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 35f;
        }

        public override bool CheckDead() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}
