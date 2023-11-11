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

        // Texture info. Also used for the projectile variants.
        public const int HitboxSize = 64;
        public const int FrameCount = 8;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = FrameCount;
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
            NPC.width = NPC.height = HitboxSize;
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

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DarkEnergy")
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
            Texture2D mainTexture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawOrigin = new Vector2(mainTexture.Width / 2, mainTexture.Height / Main.npcFrameCount[NPC.type] / 2);

            if (NPC.IsABestiaryIconDummy)
            {
                float scale = 1f;
                Main.EntitySpriteDraw(mainTexture, drawPos, NPC.frame, Color.White, NPC.rotation, drawOrigin, scale, spriteEffects, 0);
                return false;
            }
            
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Color white = Color.White * NPC.Opacity;
            int trailCount = 5;

            drawPos -= new Vector2(mainTexture.Width, mainTexture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < trailCount; i += 2)
                {
                    Color trailColor = drawColor;
                    trailColor = Color.Lerp(trailColor, white, 0.5f);
                    trailColor = NPC.GetAlpha(trailColor);
                    trailColor *= (trailCount - i) / 15f;
                    Vector2 trailPos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    trailPos -= new Vector2(mainTexture.Width, mainTexture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    trailPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(mainTexture, trailPos, NPC.frame, trailColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(mainTexture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);

            if (NPC.dontTakeDamage)
                return false;

            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow").Value;
            Color glowColor = Color.Lerp(Color.White, Color.Fuchsia, 0.5f) * NPC.Opacity;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < trailCount; i++)
                {
                    Color trailColor2 = glowColor;
                    trailColor2 = Color.Lerp(trailColor2, white, 0.5f);
                    trailColor2 = NPC.GetAlpha(trailColor2);
                    trailColor2 *= (trailCount - i) / 15f;
                    Vector2 trailPos2 = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    trailPos2 -= new Vector2(glowTexture.Width, glowTexture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    trailPos2 += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(glowTexture, trailPos2, NPC.frame, trailColor2, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(glowTexture, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * balance);
        }

        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            int debufftype = Main.zenithWorld ? BuffID.Obstructed : BuffID.VortexDebuff;
            int duration = Main.zenithWorld ? 30 : 60;
            if (hurtInfo.Damage > 0)
                target.AddBuff(debufftype, duration, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= 35f * NPC.scale;
        }

        public override bool CheckDead() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}
