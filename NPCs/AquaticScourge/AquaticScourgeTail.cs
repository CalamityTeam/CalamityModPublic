using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AquaticScourge
{
    [HasPierceResist]
    [LongDistanceNetSync(SyncWith = typeof(AquaticScourgeHead))]
    public class AquaticScourgeTail : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.AquaticScourgeHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.damage = 36; // 72
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 25;
            NPC.DR_NERD(0.15f);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.LifeMaxNERB(80000, 96000, 1000000);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.chaseable = false;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 1.25f;

            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool getFuckedAI = Main.zenithWorld;

            // Adjust hostility and stats
            bool nonHostile = calamityGlobalNPC.newAI[0] == 0f;
            if (NPC.justHit || NPC.life <= NPC.lifeMax * 0.999 || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (nonHostile)
                {
                    // Kiss my motherfucking ass you piece of shit game
                    NPC.timeLeft *= 20;
                    NPC.npcSlots = 16f;
                    NPC.damage = NPC.defDamage;
                    calamityGlobalNPC.KillTime = CalamityNPCSets.BossKillTimes[NPC.type];
                    calamityGlobalNPC.newAI[0] = 1f;
                    nonHostile = false;
                    NPC.chaseable = true;
                    NPC.netUpdate = true;
                }
            }
            else
                NPC.damage = 0;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;

            // Set worm variable
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];
            if (NPC.life > Main.npc[(int)NPC.ai[1]].life)
                NPC.life = Main.npc[(int)NPC.ai[1]].life;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool notOcean = player.position.Y < 300f ||
                player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));

            // Check for the flipped Abyss
            if (Main.remixWorld)
            {
                notOcean = player.position.Y < Main.UnderworldLayer * 0.8f || player.position.Y > Main.UnderworldLayer ||
                    (player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));
            }

            bool biomeEnraged = NPC.localAI[2] <= 0f;
            float enrageScale = 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 2f;
            }

            // Kill body and tail
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AquaticScourgeHead>())
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

            float maxDistance = calamityGlobalNPC.newAI[0] == 1f ? 12800f : 6400f;
            if (player.dead || Vector2.Distance(NPC.Center, player.Center) > maxDistance || (nonHostile && biomeEnraged))
            {
                calamityGlobalNPC.newAI[1] = 1f;
                NPC.TargetClosest(false);
                NPC.velocity.Y += 2f;

                if (NPC.position.Y > Main.worldSurface * 16D)
                    NPC.velocity.Y += 2f;

                if (NPC.position.Y > Main.worldSurface * 16D)
                {
                    for (int a = 0; a < Main.npc.Length; a++)
                    {
                        int type = Main.npc[a].type;
                        if (CalamityNPCTypeSets.AquaticScourge.Contains(type))
                            Main.npc[a].active = false;
                    }
                }
            }
            else
                calamityGlobalNPC.newAI[1] = 0f;

            // Change direction
            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;

            // Alpha changes
            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            Vector2 scourgePosition = NPC.Center;
            Vector2 predictionVector = Main.getGoodWorld ? Main.player[NPC.target].velocity * 20f : Vector2.Zero;
            float scourgeTargetX = player.Center.X + predictionVector.X;
            float scourgeTargetY = player.Center.Y + predictionVector.Y;

            // Velocity and movement
            float scourgeMaxSpeed = 5f;
            if (calamityGlobalNPC.newAI[0] == 1f)
            {
                scourgeMaxSpeed = revenge ? 14.4f : 12f;
                if (expertMode)
                    scourgeMaxSpeed += 2.4f * (1f - lifeRatio);
                scourgeMaxSpeed += 3f * enrageScale;
                if (death || getFuckedAI)
                {
                    scourgeMaxSpeed += 5f;
                    scourgeMaxSpeed += Vector2.Distance(player.Center, NPC.Center) * 0.001f;
                }

                if (Main.getGoodWorld)
                    scourgeMaxSpeed *= 1.15f;
            }

            scourgeTargetX = (int)(scourgeTargetX / 16f) * 16;
            scourgeTargetY = (int)(scourgeTargetY / 16f) * 16;
            scourgePosition.X = (int)(scourgePosition.X / 16f) * 16;
            scourgePosition.Y = (int)(scourgePosition.Y / 16f) * 16;
            scourgeTargetX -= scourgePosition.X;
            scourgeTargetY -= scourgePosition.Y;

            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    scourgePosition = NPC.Center;
                    scourgeTargetX = Main.npc[(int)NPC.ai[1]].Center.X - scourgePosition.X;
                    scourgeTargetY = Main.npc[(int)NPC.ai[1]].Center.Y - scourgePosition.Y;
                }
                catch
                {
                }

                NPC.rotation = (float)Math.Atan2(scourgeTargetY, scourgeTargetX) + MathHelper.PiOver2;
                float scourgeTargetDist = (float)Math.Sqrt(scourgeTargetX * scourgeTargetX + scourgeTargetY * scourgeTargetY);
                int scourgeWidth = NPC.width;
                scourgeTargetDist = (scourgeTargetDist - scourgeWidth) / scourgeTargetDist;
                scourgeTargetX *= scourgeTargetDist;
                scourgeTargetY *= scourgeTargetDist;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + scourgeTargetX;
                NPC.position.Y = NPC.position.Y + scourgeTargetY;

                if (scourgeTargetX < 0f)
                    NPC.spriteDirection = -1;
                else if (scourgeTargetX > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 scaledDraw = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            drawLocation += scaledDraw * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (Main.npc[(int)NPC.ai[2]].Calamity().newAI[3] > 300f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((Main.npc[(int)NPC.ai[2]].Calamity().newAI[3] - 300f) / 180f, 0f, 1f));
                else if (Main.npc[(int)NPC.ai[2]].localAI[3] > 0f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp(Main.npc[(int)NPC.ai[2]].localAI[3] / 90f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, color, NPC.rotation, scaledDraw, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return NPC.Calamity().newAI[0] == 1f;
            }
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ASTail").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ASTail2").Type, NPC.scale);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 300, true);
        }
    }
}
