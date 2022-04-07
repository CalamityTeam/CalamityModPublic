using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Polterghast
{
    public class PolterghastHook : ModNPC
    {
        private int despawnTimer = 300;
        private bool phase2 = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast Hook");
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 40;
            NPC.height = 40;
            NPC.lifeMax = 50000;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit34;
            NPC.DeathSound = SoundID.NPCDeath39;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2);
            writer.Write(despawnTimer);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2 = reader.ReadBoolean();
            despawnTimer = reader.ReadInt32();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.3f, 0.3f);

            // Bools
            bool speedBoost = false;
            bool despawnBoost = false;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Despawn if Polter is gone
            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Player player = Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target];

            if (!player.active || player.dead)
            {
                speedBoost = true;
                despawnBoost = true;
            }

            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= 420f;

            // Percent life remaining, Polter
            float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / (float)Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];

            // Despawn
            if (CalamityGlobalNPC.ghostBoss != -1 && !player.ZoneDungeon &&
                player.position.Y < Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                NPC.localAI[0] -= 6f;
                speedBoost = true;
            }
            else
                despawnTimer++;

            // Phase 2
            bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
            phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f) && !phase3;
            if (phase2)
            {
                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.TargetClosest();

                // Despawn safety, make sure to target another player if the current player target is too far away
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                    NPC.TargetClosest();

                Movement(phase2, expertMode, revenge, death, speedBoost, despawnBoost, lifeRatio, tileEnrageMult, player);

                // Fire projectiles
                Vector2 vector17 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                float num147 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - vector17.X;
                float num148 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - vector17.Y;
                float num149 = (float)Math.Sqrt(num147 * num147 + num148 * num148);

                if (chargePhase)
                {
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    return;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[3] == 0f)
                {
                    if (NPC.ai[2] > 120f)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 1f;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    if (NPC.ai[2] > 40f)
                        NPC.ai[3] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == 20f)
                    {
                        float num151 = 10f * tileEnrageMult;
                        int type = ModContent.ProjectileType<PhantomHookShot>();
                        int damage = NPC.GetProjectileDamage(type);
                        num149 = num151 / num149;
                        num147 *= num149;
                        num148 *= num149;
                        int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector17.X, vector17.Y, num147, num148, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                return;
            }

            // Phase 1 or 3
            Movement(phase2, expertMode, revenge, death, speedBoost, despawnBoost, lifeRatio, tileEnrageMult, player);
        }

        private void Movement(bool phase2, bool expertMode, bool revenge, bool death, bool speedBoost, bool despawnBoost, float lifeRatio, float tileEnrageMult, Player player)
        {
            bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= 420f;

            if (phase2)
            {
                float num740 = Main.player[NPC.target].Center.X - NPC.Center.X;
                float num741 = Main.player[NPC.target].Center.Y - NPC.Center.Y;
                NPC.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                    NPC.ai[0] = (int)(NPC.Center.X / 16f);
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = (int)(NPC.Center.X / 16f);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                    NPC.localAI[0] = 0f;

                if (chargePhase)
                {
                    NPC.localAI[0] -= 10f;
                }
                else
                {
                    float shootBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                    NPC.localAI[0] -= 1f + shootBoost * tileEnrageMult;
                    if (expertMode)
                        NPC.localAI[0] -= Vector2.Distance(NPC.Center, player.Center) * 0.002f;
                    if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] >= 300f)
                        NPC.localAI[0] -= 3f;
                    if (speedBoost)
                        NPC.localAI[0] -= 6f;
                }

                if (!despawnBoost && NPC.localAI[0] <= 0f && NPC.ai[0] != 0f)
                {
                    for (int num763 = 0; num763 < Main.maxNPCs; num763++)
                    {
                        if (num763 != NPC.whoAmI && Main.npc[num763].active && Main.npc[num763].type == NPC.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
                            NPC.localAI[0] = 180f;
                    }
                }

                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = 450f;
                    bool flag50 = false;
                    int num764 = 0;
                    while (!flag50 && num764 <= 1000)
                    {
                        num764++;
                        int num765 = (int)(player.Center.X / 16f);
                        int num766 = (int)(player.Center.Y / 16f);
                        if (NPC.ai[0] == 0f)
                        {
                            num765 = (int)((player.Center.X + Main.npc[CalamityGlobalNPC.ghostBoss].Center.X) / 32f);
                            num766 = (int)((player.Center.Y + Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y) / 32f);
                        }
                        if (despawnBoost)
                        {
                            num765 = (int)Main.npc[CalamityGlobalNPC.ghostBoss].position.X / 16;
                            num766 = (int)(Main.npc[CalamityGlobalNPC.ghostBoss].position.Y + 400f) / 16;
                        }
                        int num767 = 20;
                        num767 += (int)(100f * (num764 / 1000f));
                        int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
                        int num769 = num766 + Main.rand.Next(-num767, num767 + 1);
                        try
                        {
                            if (WorldGen.SolidTile(num768, num769) || Main.tile[num768, num769].WallType > 0 || chargePhase)
                            {
                                flag50 = true;
                                NPC.ai[0] = num768;
                                NPC.ai[1] = num769;
                                NPC.localAI[1] = Vector2.Distance(NPC.Center, player.Center) * 0.01f;
                                NPC.netUpdate = true;
                            }
                        } catch
                        {
                        }
                    }
                }
            }

            if (NPC.ai[0] > 0f && NPC.ai[1] > 0f)
            {
                float velocityBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                float velocity = (8f + velocityBoost) * tileEnrageMult;
                if (expertMode)
                    velocity += NPC.localAI[1];
                if (revenge)
                    velocity += 1f;
                if (speedBoost)
                    velocity *= 2f;
                if (despawnBoost)
                    velocity *= 2f;

                Vector2 vector95 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num773 = NPC.ai[0] * 16f - 8f - vector95.X;
                float num774 = NPC.ai[1] * 16f - 8f - vector95.Y;
                float num775 = (float)Math.Sqrt(num773 * num773 + num774 * num774);
                if (num775 < 12f + velocity)
                {
                    NPC.velocity.X = num773;
                    NPC.velocity.Y = num774;
                }
                else
                {
                    num775 = velocity / num775;
                    NPC.velocity.X = num773 * num775;
                    NPC.velocity.Y = num774 * num775;
                }

                if (!phase2)
                {
                    Vector2 vector96 = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float num776 = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - vector96.X;
                    float num777 = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - vector96.Y;
                    NPC.rotation = (float)Math.Atan2(num777, num776) - 1.57f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color lightRed = new Color(255, 100, 100, 255);
            if (Main.npc[CalamityGlobalNPC.ghostBoss].active && !phase2)
            {
                Vector2 center = NPC.Center;
                float bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
                float bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
                float rotation2 = (float)Math.Atan2(bossCenterY, bossCenterX) - 1.57f;
                bool draw = true;
                while (draw)
                {
                    int chainWidth = 20; //16 24
                    int chainHeight = 52; //32 16
                    float num10 = (float)Math.Sqrt(bossCenterX * bossCenterX + bossCenterY * bossCenterY);
                    if (num10 < chainHeight)
                    {
                        chainWidth = (int)num10 - chainHeight + chainWidth;
                        draw = false;
                    }
                    num10 = chainWidth / num10;
                    bossCenterX *= num10;
                    bossCenterY *= num10;
                    center.X += bossCenterX;
                    center.Y += bossCenterY;
                    bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
                    bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;

                    Color color2 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
                    if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f)
                        color2 = Color.Lerp(color2, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value, new Vector2(center.X - screenPos.X, center.Y - screenPos.Y),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value.Width, chainWidth)), color2, rotation2,
                        new Vector2(ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value.Width * 0.5f, ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastChain").Value.Height * 0.5f), 1f, SpriteEffects.None, 0f);
                }
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

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

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/PolterghastHookGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f)
                color37 = Color.Lerp(color37, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

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
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (phase2)
            {
                if (NPC.ai[3] == 0f)
                {
                    if (NPC.frame.Y < 1)
                    {
                        NPC.frameCounter += 1.0;
                        if (NPC.frameCounter > 4.0)
                        {
                            NPC.frameCounter = 0.0;
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                        }
                    }
                }
                else if (NPC.frame.Y > 0)
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 4.0)
                    {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y = NPC.frame.Y - frameHeight;
                    }
                }
                return;
            }
            if (NPC.velocity.X == 0f && NPC.velocity.Y == 0f)
            {
                if (NPC.frame.Y < 1)
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 4.0)
                    {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                    }
                }
            }
            else if (NPC.frame.Y > 0)
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 4.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y - frameHeight;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 180, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 180, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
