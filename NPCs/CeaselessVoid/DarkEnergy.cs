using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.CeaselessVoid
{
    public class DarkEnergy : ModNPC
    {
        private bool start = true;
        private const double minDistance = 10D;
        private double distance = minDistance;
        private const double minMaxDistance = 800D;

        public const int MaxHP = 12000;
        public const int MaxBossRushHP = 20000;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.dontTakeDamage = true;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 50;
            NPC.lifeMax = BossRushEvent.BossRushActive ? MaxBossRushHP : MaxHP;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit53;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<CeaselessVoid>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("One of the many rifts created in the fabric of space by the Ceaseless Void. Some say you could even see a glimpse of another world if you gaze into it.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(start);
            writer.Write(distance);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.Opacity);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            start = reader.ReadBoolean();
            distance = reader.ReadDouble();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.Opacity = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            // Set the degrees used for rotation
            if (start)
            {
                start = false;
                NPC.ai[3] = NPC.ai[0];
            }

            // Stay invincible for 200 frames to avoid being instantly killed and don't deal damage to avoid unfair hits
            if (NPC.Opacity < 1f && NPC.dontTakeDamage)
            {
                NPC.damage = 0;

                NPC.Opacity += 0.005f;
                if (NPC.Opacity > 1f)
                    NPC.Opacity = 1f;

                NPC.scale = MathHelper.Lerp(0.05f, Main.getGoodWorld ? 0.5f : 1f, NPC.Opacity);
            }
            else
            {
                if (NPC.dontTakeDamage)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0, 0, 0, default, 1f);
                        Main.dust[dust].noGravity = true;
                    }
                }

                NPC.damage = NPC.defDamage;
                NPC.dontTakeDamage = false;
                float scalar = (float)Math.Cos(NPC.Calamity().newAI[1] * 0.33f) / 2f + 0.5f;
                NPC.scale = MathHelper.Lerp(0.8f, 1f, scalar);
                NPC.Opacity = MathHelper.Lerp(0.5f, 1f, scalar);
                NPC.Calamity().newAI[1] += 1f;
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.8f * NPC.Opacity, 0f, 1.2f * NPC.Opacity);

            // Force despawn if Ceaseless Void isn't active
            if (CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Set time left just in case something dumb happens with despawning
            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Gets how enraged Ceaseless Void is
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.voidBoss].ai[1];

            // Distance from Ceaseless Void
            double maxDistance = bossRush ? 1200D : death ? 1040D : revenge ? 960D : expertMode ? 880D : minMaxDistance;
            double rateOfChangeIncrease = (maxDistance / minMaxDistance) - 1D;
            double rateOfChange = (NPC.ai[1] * 0.5f) + 2D + (tileEnrageMult - 1f) + rateOfChangeIncrease;
            if (NPC.Calamity().newAI[0] == 0f)
            {
                distance += rateOfChange;
                if (distance >= maxDistance)
                {
                    distance = maxDistance;
                    NPC.Calamity().newAI[0] = 1f;
                }
            }
            else
            {
                distance -= rateOfChange;
                if (distance <= minDistance)
                {
                    distance = minDistance;
                    NPC.Calamity().newAI[0] = 0f;
                }
            }

            // Rotation velocity
            float minRotationVelocity = 0.5f + tileEnrageMult - 1f;
            float rotationVelocityIncrease = death ? 0.2f : revenge ? 0.15f : expertMode ? 0.1f : 0f;
            rotationVelocityIncrease += rotationVelocityIncrease * (NPC.ai[1] * 0.5f);

            // Rotate around Ceaseless Void
            NPC parent = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<CeaselessVoid>())];
            double degrees = NPC.ai[3];
            double radians = degrees * (Math.PI / 180);
            NPC.position.X = parent.Center.X - (int)(Math.Cos(radians) * distance) - NPC.width / 2;
            NPC.position.Y = parent.Center.Y - (int)(Math.Sin(radians) * distance) - NPC.height / 2;
            NPC.ai[3] += minRotationVelocity + rotationVelocityIncrease;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D16 = ModContent.Request<Texture2D>("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow2").Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White * NPC.Opacity;
            float amount9 = 0.5f;
            int num153 = 5;

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            if (NPC.dontTakeDamage)
                return false;

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f) * NPC.Opacity;
            Color color42 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f) * NPC.Opacity;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = NPC.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                    Color color43 = color42;
                    color43 = Color.Lerp(color43, color36, amount9);
                    color43 = NPC.GetAlpha(color43);
                    color43 *= (num153 - num163) / 15f;
                    spriteBatch.Draw(texture2D16, vector44, NPC.frame, color43, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            spriteBatch.Draw(texture2D16, vector43, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            int debufftype = CalamityWorld.getFixedBoi ? BuffID.Obstructed : BuffID.VortexDebuff;
            int duration = CalamityWorld.getFixedBoi ? 30 : 60;
            if (damage > 0)
                player.AddBuff(debufftype, duration, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 35f * NPC.scale;
        }

        public override bool CheckDead() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}
