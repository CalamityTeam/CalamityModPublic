using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Enemy;
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
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.Abyss
{
    public class ColossalSquid : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Colossal Squid");
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 9f;
            NPC.noGravity = true;
            NPC.damage = 150;
            NPC.width = 180;
            NPC.height = 180;
            NPC.defense = 50;
            NPC.DR_NERD(0.05f);
            NPC.lifeMax = 130000; // Previously 220,000
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.value = Item.buyPrice(0, 25, 0, 0);
            NPC.HitSound = SoundID.NPCHit20;
            NPC.DeathSound = SoundID.NPCDeath23;
            NPC.rarity = 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ColossalSquidBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AbyssLayer3,
                //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AbyssLayer4,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("With crushing tentacles that stretch out to over twice the length of a grown man, and toxic black ink, these are formidable predators indeed.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (NPC.localAI[1] == 1f)
            {
                NPC.localAI[3] += 1f;
                if (NPC.localAI[3] >= 180f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.localAI[0] = 0f;
                    NPC.localAI[1] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.localAI[3] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
                if (Main.rand.NextBool(300))
                {
                    SoundEngine.PlaySoundCommonCalamitySounds.GetZombieSound(34), NPC.position);
                }
                NPC.noTileCollide = false;
                if (NPC.ai[0] == 0f)
                {
                    NPC.TargetClosest(true);
                    if (Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[0] = 1f;
                    }
                    else
                    {
                        Vector2 value29 = Main.player[NPC.target].Center - NPC.Center;
                        value29.Y -= (float)(Main.player[NPC.target].height / 4);
                        float num1310 = value29.Length();
                        if (num1310 > 800f)
                        {
                            NPC.ai[0] = 2f;
                        }
                        else
                        {
                            Vector2 center26 = NPC.Center;
                            center26.X = Main.player[NPC.target].Center.X;
                            Vector2 vector230 = center26 - NPC.Center;
                            if (vector230.Length() > 8f && Collision.CanHit(NPC.Center, 1, 1, center26, 1, 1))
                            {
                                NPC.ai[0] = 3f;
                                NPC.ai[1] = center26.X;
                                NPC.ai[2] = center26.Y;
                                Vector2 center27 = NPC.Center;
                                center27.Y = Main.player[NPC.target].Center.Y;
                                if (vector230.Length() > 8f && Collision.CanHit(NPC.Center, 1, 1, center27, 1, 1) && Collision.CanHit(center27, 1, 1, Main.player[NPC.target].position, 1, 1))
                                {
                                    NPC.ai[0] = 3f;
                                    NPC.ai[1] = center27.X;
                                    NPC.ai[2] = center27.Y;
                                }
                            }
                            else
                            {
                                center26 = NPC.Center;
                                center26.Y = Main.player[NPC.target].Center.Y;
                                if ((center26 - NPC.Center).Length() > 8f && Collision.CanHit(NPC.Center, 1, 1, center26, 1, 1))
                                {
                                    NPC.ai[0] = 3f;
                                    NPC.ai[1] = center26.X;
                                    NPC.ai[2] = center26.Y;
                                }
                            }
                            if (NPC.ai[0] == 0f)
                            {
                                NPC.localAI[0] = 0f;
                                value29.Normalize();
                                value29 *= 0.5f;
                                NPC.velocity += value29;
                                NPC.ai[0] = 4f;
                                NPC.ai[1] = 0f;
                            }
                        }
                    }
                }
                else if (NPC.ai[0] == 1f)
                {
                    NPC.rotation += (float)NPC.direction * 0.1f;
                    Vector2 value30 = Main.player[NPC.target].Top - NPC.Center;
                    float num1311 = value30.Length();
                    float num1312 = 5f;
                    num1312 += num1311 / 100f;
                    int num1313 = 50;
                    value30.Normalize();
                    value30 *= num1312;
                    NPC.velocity = (NPC.velocity * (float)(num1313 - 1) + value30) / (float)num1313;
                    if (!Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                    }
                    if (num1311 < 160f && Main.player[NPC.target].active && !Main.player[NPC.target].dead)
                    {
                        NPC.Center = Main.player[NPC.target].Top;
                        NPC.velocity = Vector2.Zero;
                        NPC.ai[0] = 5f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[0] == 2f)
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    NPC.noTileCollide = true;
                    Vector2 value31 = Main.player[NPC.target].Center - NPC.Center;
                    float num1315 = value31.Length();
                    float scaleFactor11 = 3f;
                    int num1316 = 3;
                    value31.Normalize();
                    value31 *= scaleFactor11;
                    NPC.velocity = (NPC.velocity * (float)(num1316 - 1) + value31) / (float)num1316;
                    if (num1315 < 600f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.ai[0] = 0f;
                    }
                }
                else if (NPC.ai[0] == 3f)
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    Vector2 value32 = new Vector2(NPC.ai[1], NPC.ai[2]);
                    Vector2 value33 = value32 - NPC.Center;
                    float num1317 = value33.Length();
                    float num1318 = 2f;
                    float num1319 = 3f;
                    value33.Normalize();
                    value33 *= num1318;
                    NPC.velocity = (NPC.velocity * (num1319 - 1f) + value33) / num1319;
                    if (NPC.collideX || NPC.collideY)
                    {
                        NPC.ai[0] = 4f;
                        NPC.ai[1] = 0f;
                    }
                    if (num1317 < num1318 || num1317 > 800f || Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[0] = 0f;
                    }
                }
                else if (NPC.ai[0] == 4f)
                {
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    if (NPC.collideX)
                    {
                        NPC.velocity.X = NPC.velocity.X * -0.8f;
                    }
                    if (NPC.collideY)
                    {
                        NPC.velocity.Y = NPC.velocity.Y * -0.8f;
                    }
                    Vector2 value34;
                    if (NPC.velocity.X == 0f && NPC.velocity.Y == 0f)
                    {
                        value34 = Main.player[NPC.target].Center - NPC.Center;
                        value34.Y -= (float)(Main.player[NPC.target].height / 4);
                        value34.Normalize();
                        NPC.velocity = value34 * 0.1f;
                    }
                    float scaleFactor12 = 2f;
                    float num1320 = 20f;
                    value34 = NPC.velocity;
                    value34.Normalize();
                    value34 *= scaleFactor12;
                    NPC.velocity = (NPC.velocity * (num1320 - 1f) + value34) / num1320;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 180f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                    }
                    if (Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                    {
                        NPC.ai[0] = 0f;
                    }
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 5f && !Collision.SolidCollision(NPC.position - new Vector2(10f, 10f), NPC.width + 20, NPC.height + 20))
                    {
                        NPC.localAI[0] = 0f;
                        Vector2 center28 = NPC.Center;
                        center28.X = Main.player[NPC.target].Center.X;
                        if (Collision.CanHit(NPC.Center, 1, 1, center28, 1, 1) && Collision.CanHit(NPC.Center, 1, 1, center28, 1, 1) && Collision.CanHit(Main.player[NPC.target].Center, 1, 1, center28, 1, 1))
                        {
                            NPC.ai[0] = 3f;
                            NPC.ai[1] = center28.X;
                            NPC.ai[2] = center28.Y;
                        }
                        else
                        {
                            center28 = NPC.Center;
                            center28.Y = Main.player[NPC.target].Center.Y;
                            if (Collision.CanHit(NPC.Center, 1, 1, center28, 1, 1) && Collision.CanHit(Main.player[NPC.target].Center, 1, 1, center28, 1, 1))
                            {
                                NPC.ai[0] = 3f;
                                NPC.ai[1] = center28.X;
                                NPC.ai[2] = center28.Y;
                            }
                        }
                    }
                }
                else if (NPC.ai[0] == 5f)
                {
                    Player player7 = Main.player[NPC.target];
                    if (!player7.active || player7.dead)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.Center = ((player7.gravDir == 1f) ? player7.Top : player7.Bottom) + new Vector2((float)(player7.direction * 4), 0f);
                        NPC.gfxOffY = player7.gfxOffY;
                        NPC.velocity = Vector2.Zero;
                        player7.AddBuff(163, 59, true);
                    }
                }
                NPC.rotation = NPC.velocity.X * 0.05f;
            }
            else
            {
                if (NPC.direction == 0)
                {
                    NPC.TargetClosest(true);
                }
                if (!NPC.noTileCollide)
                {
                    if (NPC.collideX)
                    {
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.direction *= -1;
                    }
                    if (NPC.collideY)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                            NPC.directionY = -1;
                            NPC.ai[0] = -1f;
                        }
                        else if (NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                            NPC.directionY = 1;
                            NPC.ai[0] = 1f;
                        }
                    }
                }
                NPC.TargetClosest(false);
                if ((Main.player[NPC.target].wet && !Main.player[NPC.target].dead &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) &&
                    //(Main.player[npc.target].Center - npc.Center).Length() < ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
                    //Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 150f : 300f) *
                    //(Main.player[npc.target].GetCalamityPlayer().fishAlert ? 3f : 1f)) ||
                    (Main.player[NPC.target].Center - NPC.Center).Length() < Main.player[NPC.target].Calamity().GetAbyssAggro(300f, 150f)) ||
                    NPC.justHit)
                {
                    hasBeenHit = true;
                }
                NPC.chaseable = hasBeenHit;
                if (hasBeenHit)
                {
                    if (Main.rand.NextBool(300))
                    {
                        SoundEngine.PlaySound(CommonCalamitySounds.GetZombieSound(34), NPC.position);
                    }
                    if (NPC.ai[3] > 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        {
                            NPC.ai[3] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[3] == 0f)
                    {
                        NPC.ai[1] += 1f;
                    }
                    if (NPC.ai[1] >= 120f)
                    {
                        NPC.ai[3] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[3] == 0f)
                    {
                        NPC.noTileCollide = false;
                    }
                    else
                    {
                        NPC.noTileCollide = true;
                    }
                    NPC.localAI[3] += 1f;
                    if (NPC.localAI[3] >= 420f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 1f;
                        NPC.localAI[2] = 0f;
                        NPC.localAI[3] = 0f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                        return;
                    }
                    NPC.localAI[2] = 1f;
                    NPC.localAI[0] += 1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 150f)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.netUpdate = true;
                        int damage = 70;
                        if (Main.expertMode)
                        {
                            damage = 55;
                        }
                        SoundEngine.PlaySound(SoundID.Item111, NPC.position);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 60, 0f, 2f, ModContent.ProjectileType<InkBombHostile>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    NPC.velocity *= 0.975f;
                    float num263 = 2.5f;
                    if (NPC.velocity.X > -num263 && NPC.velocity.X < num263 && NPC.velocity.Y > -num263 && NPC.velocity.Y < num263)
                    {
                        NPC.TargetClosest(true);
                        float num264 = 20f;
                        Vector2 vector31 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                        float num265 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector31.X;
                        float num266 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector31.Y;
                        float num267 = (float)Math.Sqrt((double)(num265 * num265 + num266 * num266));
                        num267 = num264 / num267;
                        num265 *= num267;
                        num266 *= num267;
                        NPC.velocity.X = num265;
                        NPC.velocity.Y = num266;
                        return;
                    }
                }
                else
                {
                    if (Main.rand.NextBool(300))
                    {
                        SoundEngine.PlaySound(CommonCalamitySounds.GetZombieSound(35), NPC.position);
                    }
                    NPC.localAI[2] = 0f;
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.02f;
                    NPC.rotation = NPC.velocity.X * 0.2f;
                    if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.95f;
                    }
                    if (NPC.ai[0] == -1f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                        if (NPC.velocity.Y < -1f)
                        {
                            NPC.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                        if (NPC.velocity.Y > 1f)
                        {
                            NPC.ai[0] = -1f;
                        }
                    }
                    int num268 = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                    int num269 = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                    if (Main.tile[num268, num269 - 1].LiquidAmount > 128)
                    {
                        if (Main.tile[num268, num269 + 1].HasTile)
                        {
                            NPC.ai[0] = -1f;
                        }
                        else if (Main.tile[num268, num269 + 2].HasTile)
                        {
                            NPC.ai[0] = -1f;
                        }
                    }
                    else
                    {
                        NPC.ai[0] = 1f;
                    }
                    if ((double)NPC.velocity.Y > 1.2 || (double)NPC.velocity.Y < -1.2)
                    {
                        NPC.velocity.Y = NPC.velocity.Y * 0.99f;
                        return;
                    }
                }
            }
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 6400f)
            {
                NPC.active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] == 5f)
            {
                return false;
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] == 5f)
            {
                Color color = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16,
                    (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Player player = Main.player[NPC.target];
                player.invis = true;
                player.aggro = -750;
                if (player.gravDir == -1f)
                {
                    spriteEffects |= SpriteEffects.FlipVertically;
                }
                Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value,
                    new Vector2((float)(player.direction * 4), player.gfxOffY) + ((player.gravDir == 1f) ? player.Top : player.Bottom) - screenPos,
                    new Microsoft.Xna.Framework.Rectangle?(NPC.frame), NPC.GetAlpha(color), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects, 0f);
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if ((spawnInfo.Player.Calamity().ZoneAbyssLayer3 || spawnInfo.Player.Calamity().ZoneAbyssLayer4) && spawnInfo.Water && !NPC.AnyNPCs(ModContent.NPCType<ColossalSquid>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.BlackInk, 1, 3, 5);
            npcLoot.Add(ModContent.ItemType<InkBomb>(), 10);

            var postClone = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedCalamitas);
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<DepthCells>(), 2, 26, 38, 31, 45));

            npcLoot.AddIf(() => DownedBossSystem.downedPolterghast, ModContent.ItemType<CalamarisLament>(), 3);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ColossalSquid").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ColossalSquid2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ColossalSquid3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ColossalSquid4").Type, 1f);
                }
            }
        }
    }
}
