using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHeadMedium : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 58;
            NPC.height = 68;
            NPC.defense = 2;
            NPC.LifeMaxNERB(150, 180, 7000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                NPC.scale = 1.25f;
            else if (CalamityWorld.death)
                NPC.scale = 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale = 1.15f;
            else if (Main.expertMode)
                NPC.scale = 1.1f;

            NPC.Calamity().SplittingWorm = true;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
            if ((NPC.position.Y / 16f) < Main.worldSurface || malice)
                enrageScale += 1f;
            if (!Main.player[NPC.target].ZoneCrimson || malice)
                enrageScale += 1f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float speed = 0.08f;
            float turnSpeed = 0.06f;

            if (expertMode)
            {
                float velocityScale = (death ? 0.08f : 0.07f) * enrageScale;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = (death ? 0.08f : 0.07f) * enrageScale;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            NPC.realLife = -1;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    int totalSegments = death ? 14 : revenge ? 13 : expertMode ? 12 : 10;
                    NPC.ai[2] = totalSegments;
                    NPC.ai[0] = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), ModContent.NPCType<PerforatorBodyMedium>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Splitting effect
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }

                if (!NPC.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }

            // Movement
            int num29 = (int)(NPC.position.X / 16f) - 1;
            int num30 = (int)((NPC.position.X + NPC.width) / 16f) + 2;
            int num31 = (int)(NPC.position.Y / 16f) - 1;
            int num32 = (int)((NPC.position.Y + NPC.height) / 16f) + 2;
            if (num29 < 0)
                num29 = 0;
            if (num30 > Main.maxTilesX)
                num30 = Main.maxTilesX;
            if (num31 < 0)
                num31 = 0;
            if (num32 > Main.maxTilesY)
                num32 = Main.maxTilesY;

            // Fly or not
            bool flag2 = false;
            if (!flag2)
            {
                for (int num33 = num29; num33 < num30; num33++)
                {
                    for (int num34 = num31; num34 < num32; num34++)
                    {
                        if (Main.tile[num33, num34] != null && ((Main.tile[num33, num34].HasUnactuatedTile && (Main.tileSolid[Main.tile[num33, num34].TileType] || (Main.tileSolidTop[Main.tile[num33, num34].TileType] && Main.tile[num33, num34].TileFrameY == 0))) || Main.tile[num33, num34].LiquidAmount > 64))
                        {
                            Vector2 vector;
                            vector.X = num33 * 16;
                            vector.Y = num34 * 16;
                            if (NPC.position.X + NPC.width > vector.X && NPC.position.X < vector.X + 16f && NPC.position.Y + NPC.height > vector.Y && NPC.position.Y < vector.Y + 16f)
                            {
                                flag2 = true;
                                if (Main.rand.NextBool(100) && Main.tile[num33, num34].HasUnactuatedTile)
                                {
                                    WorldGen.KillTile(num33, num34, true, true, false);
                                }
                            }
                        }
                    }
                }
            }
            if (!flag2)
            {
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int num35 = death ? 160 : revenge ? 200 : expertMode ? 240 : 300;
                bool flag3 = true;
                for (int num36 = 0; num36 < Main.maxPlayers; num36++)
                {
                    if (Main.player[num36].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[num36].position.X - num35, (int)Main.player[num36].position.Y - num35, num35 * 2, num35 * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            flag3 = false;
                            break;
                        }
                    }
                }
                if (flag3)
                    flag2 = true;
            }

            float fallSpeed = 16f;
            if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
            {
                NPC.TargetClosest(false);
                flag2 = false;
                NPC.velocity.Y += 1f;
                if (NPC.position.Y > Main.worldSurface * 16.0)
                {
                    NPC.velocity.Y += 1f;
                    fallSpeed = 32f;
                }
                if (NPC.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < Main.maxNPCs; num957++)
                    {
                        if (Main.npc[num957].type == NPC.type || Main.npc[num957].type == ModContent.NPCType<PerforatorBodyMedium>() || Main.npc[num957].type == ModContent.NPCType<PerforatorTailMedium>())
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }

            // Velocity and acceleration
            float num37 = speed;
            float num38 = turnSpeed;

            Vector2 vector2 = NPC.Center;
            float num39 = player.Center.X;
            float num40 = player.Center.Y;

            num39 = (int)(num39 / 16f) * 16;
            num40 = (int)(num40 / 16f) * 16;
            vector2.X = (int)(vector2.X / 16f) * 16;
            vector2.Y = (int)(vector2.Y / 16f) * 16;
            num39 -= vector2.X;
            num40 -= vector2.Y;
            float num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);

            // Prevent new heads from being slowed when they spawn
            if (NPC.Calamity().newAI[1] < 3f)
            {
                NPC.Calamity().newAI[1] += 1f;

                // Set velocity for when a new head spawns
                NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (num37 * (death ? 0.3f : 0.2f));
            }

            if (!flag2)
            {
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y > fallSpeed)
                    NPC.velocity.Y = fallSpeed;

                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= num38 * 1.1f;
                    else
                        NPC.velocity.X += num38 * 1.1f;
                }
                else if (NPC.velocity.Y == fallSpeed)
                {
                    if (NPC.velocity.X < num39)
                        NPC.velocity.X += num38;
                    else if (NPC.velocity.X > num39)
                        NPC.velocity.X -= num38;
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X += num38 * 0.9f;
                    else
                        NPC.velocity.X -= num38 * 0.9f;
                }
            }
            else
            {
                // Sound
                if (NPC.soundDelay == 0)
                {
                    float num54 = num52 / 40f;
                    if (num54 < 10f)
                        num54 = 10f;
                    if (num54 > 20f)
                        num54 = 20f;

                    NPC.soundDelay = (int)num54;
                    SoundEngine.PlaySound(SoundID.Roar, (int)NPC.position.X, (int)NPC.position.Y, 1, 1f, 0f);
                }

                num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
                float num55 = Math.Abs(num39);
                float num56 = Math.Abs(num40);
                float num57 = fallSpeed / num52;
                num39 *= num57;
                num40 *= num57;

                if (((NPC.velocity.X > 0f && num39 > 0f) || (NPC.velocity.X < 0f && num39 < 0f)) && ((NPC.velocity.Y > 0f && num40 > 0f) || (NPC.velocity.Y < 0f && num40 < 0f)))
                {
                    if (NPC.velocity.X < num39)
                        NPC.velocity.X += num38;
                    else if (NPC.velocity.X > num39)
                        NPC.velocity.X -= num38;

                    if (NPC.velocity.Y < num40)
                        NPC.velocity.Y += num38;
                    else if (NPC.velocity.Y > num40)
                        NPC.velocity.Y -= num38;
                }

                if ((NPC.velocity.X > 0f && num39 > 0f) || (NPC.velocity.X < 0f && num39 < 0f) || (NPC.velocity.Y > 0f && num40 > 0f) || (NPC.velocity.Y < 0f && num40 < 0f))
                {
                    if (NPC.velocity.X < num39)
                        NPC.velocity.X += num37;
                    else if (NPC.velocity.X > num39)
                        NPC.velocity.X -= num37;

                    if (NPC.velocity.Y < num40)
                        NPC.velocity.Y += num37;
                    else if (NPC.velocity.Y > num40)
                        NPC.velocity.Y -= num37;

                    if (Math.Abs(num40) < fallSpeed * 0.2 && ((NPC.velocity.X > 0f && num39 < 0f) || (NPC.velocity.X < 0f && num39 > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += num37 * 2f;
                        else
                            NPC.velocity.Y -= num37 * 2f;
                    }

                    if (Math.Abs(num39) < fallSpeed * 0.2 && ((NPC.velocity.Y > 0f && num40 < 0f) || (NPC.velocity.Y < 0f && num40 > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += num37 * 2f;
                        else
                            NPC.velocity.X -= num37 * 2f;
                    }
                }
                else if (num55 > num56)
                {
                    if (NPC.velocity.X < num39)
                        NPC.velocity.X += num37 * 1.1f;
                    else if (NPC.velocity.X > num39)
                        NPC.velocity.X -= num37 * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += num37;
                        else
                            NPC.velocity.Y -= num37;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < num40)
                        NPC.velocity.Y += num37 * 1.1f;
                    else if (NPC.velocity.Y > num40)
                        NPC.velocity.Y -= num37 * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < fallSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += num37;
                        else
                            NPC.velocity.X -= num37;
                    }
                }
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

            if (flag2)
            {
                if (NPC.localAI[0] != 1f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Perforator/PerforatorHeadMediumGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf").Type, NPC.scale);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf2").Type, NPC.scale);
                }
            }
        }

        public override void OnKill()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
                if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem(NPC.GetItemSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != NPC.whoAmI && Main.npc[i].active && (Main.npc[i].type == NPC.type || Main.npc[i].type == ModContent.NPCType<PerforatorBodyMedium>() || Main.npc[i].type == ModContent.NPCType<PerforatorTailMedium>()))
                    return;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "The Medium Perforator";
            potionType = ItemID.HealingPotion;
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                NPC.type,
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorTailMedium>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BurningBlood>(), 240, true);
        }
    }
}
