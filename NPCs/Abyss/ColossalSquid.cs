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
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
	public class ColossalSquid : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Colossal Squid");
            Main.npcFrameCount[npc.type] = 11;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 9f;
            npc.noGravity = true;
            npc.damage = 150;
            npc.width = 180;
            npc.height = 180;
            npc.defense = 50;
            npc.DR_NERD(0.05f);
            npc.lifeMax = 130000; // Previously 220,000
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.timeLeft = NPC.activeTime * 30;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.HitSound = SoundID.NPCHit20;
            npc.DeathSound = SoundID.NPCDeath23;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ColossalSquidBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(npc.chaseable);
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (npc.localAI[1] == 1f)
            {
                npc.localAI[3] += 1f;
                if (npc.localAI[3] >= 180f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.localAI[0] = 0f;
                    npc.localAI[1] = 0f;
                    npc.localAI[2] = 0f;
                    npc.localAI[3] = 0f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
                if (Main.rand.NextBool(300))
                {
                    Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 34);
                }
                npc.noTileCollide = false;
                if (npc.ai[0] == 0f)
                {
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[0] = 1f;
                    }
                    else
                    {
                        Vector2 value29 = Main.player[npc.target].Center - npc.Center;
                        value29.Y -= (float)(Main.player[npc.target].height / 4);
                        float num1310 = value29.Length();
                        if (num1310 > 800f)
                        {
                            npc.ai[0] = 2f;
                        }
                        else
                        {
                            Vector2 center26 = npc.Center;
                            center26.X = Main.player[npc.target].Center.X;
                            Vector2 vector230 = center26 - npc.Center;
                            if (vector230.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center26, 1, 1))
                            {
                                npc.ai[0] = 3f;
                                npc.ai[1] = center26.X;
                                npc.ai[2] = center26.Y;
                                Vector2 center27 = npc.Center;
                                center27.Y = Main.player[npc.target].Center.Y;
                                if (vector230.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center27, 1, 1) && Collision.CanHit(center27, 1, 1, Main.player[npc.target].position, 1, 1))
                                {
                                    npc.ai[0] = 3f;
                                    npc.ai[1] = center27.X;
                                    npc.ai[2] = center27.Y;
                                }
                            }
                            else
                            {
                                center26 = npc.Center;
                                center26.Y = Main.player[npc.target].Center.Y;
                                if ((center26 - npc.Center).Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center26, 1, 1))
                                {
                                    npc.ai[0] = 3f;
                                    npc.ai[1] = center26.X;
                                    npc.ai[2] = center26.Y;
                                }
                            }
                            if (npc.ai[0] == 0f)
                            {
                                npc.localAI[0] = 0f;
                                value29.Normalize();
                                value29 *= 0.5f;
                                npc.velocity += value29;
                                npc.ai[0] = 4f;
                                npc.ai[1] = 0f;
                            }
                        }
                    }
                }
                else if (npc.ai[0] == 1f)
                {
                    npc.rotation += (float)npc.direction * 0.1f;
                    Vector2 value30 = Main.player[npc.target].Top - npc.Center;
                    float num1311 = value30.Length();
                    float num1312 = 5f;
                    num1312 += num1311 / 100f;
                    int num1313 = 50;
                    value30.Normalize();
                    value30 *= num1312;
                    npc.velocity = (npc.velocity * (float)(num1313 - 1) + value30) / (float)num1313;
                    if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                    }
                    if (num1311 < 160f && Main.player[npc.target].active && !Main.player[npc.target].dead)
                    {
                        npc.Center = Main.player[npc.target].Top;
                        npc.velocity = Vector2.Zero;
                        npc.ai[0] = 5f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[0] == 2f)
                {
                    npc.rotation = npc.velocity.X * 0.05f;
                    npc.noTileCollide = true;
                    Vector2 value31 = Main.player[npc.target].Center - npc.Center;
                    float num1315 = value31.Length();
                    float scaleFactor11 = 3f;
                    int num1316 = 3;
                    value31.Normalize();
                    value31 *= scaleFactor11;
                    npc.velocity = (npc.velocity * (float)(num1316 - 1) + value31) / (float)num1316;
                    if (num1315 < 600f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 0f;
                    }
                }
                else if (npc.ai[0] == 3f)
                {
                    npc.rotation = npc.velocity.X * 0.05f;
                    Vector2 value32 = new Vector2(npc.ai[1], npc.ai[2]);
                    Vector2 value33 = value32 - npc.Center;
                    float num1317 = value33.Length();
                    float num1318 = 2f;
                    float num1319 = 3f;
                    value33.Normalize();
                    value33 *= num1318;
                    npc.velocity = (npc.velocity * (num1319 - 1f) + value33) / num1319;
                    if (npc.collideX || npc.collideY)
                    {
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                    }
                    if (num1317 < num1318 || num1317 > 800f || Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[0] = 0f;
                    }
                }
                else if (npc.ai[0] == 4f)
                {
                    npc.rotation = npc.velocity.X * 0.05f;
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * -0.8f;
                    }
                    if (npc.collideY)
                    {
                        npc.velocity.Y = npc.velocity.Y * -0.8f;
                    }
                    Vector2 value34;
                    if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
                    {
                        value34 = Main.player[npc.target].Center - npc.Center;
                        value34.Y -= (float)(Main.player[npc.target].height / 4);
                        value34.Normalize();
                        npc.velocity = value34 * 0.1f;
                    }
                    float scaleFactor12 = 2f;
                    float num1320 = 20f;
                    value34 = npc.velocity;
                    value34.Normalize();
                    value34 *= scaleFactor12;
                    npc.velocity = (npc.velocity * (num1320 - 1f) + value34) / num1320;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 180f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                    }
                    if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[0] = 0f;
                    }
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 5f && !Collision.SolidCollision(npc.position - new Vector2(10f, 10f), npc.width + 20, npc.height + 20))
                    {
                        npc.localAI[0] = 0f;
                        Vector2 center28 = npc.Center;
                        center28.X = Main.player[npc.target].Center.X;
                        if (Collision.CanHit(npc.Center, 1, 1, center28, 1, 1) && Collision.CanHit(npc.Center, 1, 1, center28, 1, 1) && Collision.CanHit(Main.player[npc.target].Center, 1, 1, center28, 1, 1))
                        {
                            npc.ai[0] = 3f;
                            npc.ai[1] = center28.X;
                            npc.ai[2] = center28.Y;
                        }
                        else
                        {
                            center28 = npc.Center;
                            center28.Y = Main.player[npc.target].Center.Y;
                            if (Collision.CanHit(npc.Center, 1, 1, center28, 1, 1) && Collision.CanHit(Main.player[npc.target].Center, 1, 1, center28, 1, 1))
                            {
                                npc.ai[0] = 3f;
                                npc.ai[1] = center28.X;
                                npc.ai[2] = center28.Y;
                            }
                        }
                    }
                }
                else if (npc.ai[0] == 5f)
                {
                    Player player7 = Main.player[npc.target];
                    if (!player7.active || player7.dead)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        npc.Center = ((player7.gravDir == 1f) ? player7.Top : player7.Bottom) + new Vector2((float)(player7.direction * 4), 0f);
                        npc.gfxOffY = player7.gfxOffY;
                        npc.velocity = Vector2.Zero;
                        player7.AddBuff(163, 59, true);
                    }
                }
                npc.rotation = npc.velocity.X * 0.05f;
            }
            else
            {
                if (npc.direction == 0)
                {
                    npc.TargetClosest(true);
                }
                if (!npc.noTileCollide)
                {
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * -1f;
                        npc.direction *= -1;
                    }
                    if (npc.collideY)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }
                npc.TargetClosest(false);
                if ((Main.player[npc.target].wet && !Main.player[npc.target].dead &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
                    //(Main.player[npc.target].Center - npc.Center).Length() < ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
                    //Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 150f : 300f) *
                    //(Main.player[npc.target].GetCalamityPlayer().fishAlert ? 3f : 1f)) ||
                    (Main.player[npc.target].Center - npc.Center).Length() < Main.player[npc.target].Calamity().GetAbyssAggro(300f, 150f)) ||
                    npc.justHit)
                {
                    hasBeenHit = true;
                }
                npc.chaseable = hasBeenHit;
                if (hasBeenHit)
                {
                    if (Main.rand.NextBool(300))
                    {
                        Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 34);
                    }
                    if (npc.ai[3] > 0f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            npc.ai[3] = 0f;
                            npc.ai[1] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                    else if (npc.ai[3] == 0f)
                    {
                        npc.ai[1] += 1f;
                    }
                    if (npc.ai[1] >= 120f)
                    {
                        npc.ai[3] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                    if (npc.ai[3] == 0f)
                    {
                        npc.noTileCollide = false;
                    }
                    else
                    {
                        npc.noTileCollide = true;
                    }
                    npc.localAI[3] += 1f;
                    if (npc.localAI[3] >= 420f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.localAI[0] = 0f;
                        npc.localAI[1] = 1f;
                        npc.localAI[2] = 0f;
                        npc.localAI[3] = 0f;
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                        return;
                    }
                    npc.localAI[2] = 1f;
                    npc.localAI[0] += 1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 150f)
                    {
                        npc.localAI[0] = 0f;
                        npc.netUpdate = true;
                        int damage = 70;
                        if (Main.expertMode)
                        {
                            damage = 55;
                        }
                        Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 111);
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y + 60, 0f, 2f, ModContent.ProjectileType<InkBombHostile>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    npc.rotation = npc.velocity.X * 0.05f;
                    npc.velocity *= 0.975f;
                    float num263 = 2.5f;
                    if (npc.velocity.X > -num263 && npc.velocity.X < num263 && npc.velocity.Y > -num263 && npc.velocity.Y < num263)
                    {
                        npc.TargetClosest(true);
                        float num264 = 20f;
                        Vector2 vector31 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num265 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector31.X;
                        float num266 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector31.Y;
                        float num267 = (float)Math.Sqrt((double)(num265 * num265 + num266 * num266));
                        num267 = num264 / num267;
                        num265 *= num267;
                        num266 *= num267;
                        npc.velocity.X = num265;
                        npc.velocity.Y = num266;
                        return;
                    }
                }
                else
                {
                    if (Main.rand.NextBool(300))
                    {
                        Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 35);
                    }
                    npc.localAI[2] = 0f;
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.02f;
                    npc.rotation = npc.velocity.X * 0.2f;
                    if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.95f;
                    }
                    if (npc.ai[0] == -1f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                        if (npc.velocity.Y < -1f)
                        {
                            npc.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                        if (npc.velocity.Y > 1f)
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                    int num268 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                    int num269 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                    if (Main.tile[num268, num269 - 1] == null)
                    {
                        Main.tile[num268, num269 - 1] = new Tile();
                    }
                    if (Main.tile[num268, num269 + 1] == null)
                    {
                        Main.tile[num268, num269 + 1] = new Tile();
                    }
                    if (Main.tile[num268, num269 + 2] == null)
                    {
                        Main.tile[num268, num269 + 2] = new Tile();
                    }
                    if (Main.tile[num268, num269 - 1].liquid > 128)
                    {
                        if (Main.tile[num268, num269 + 1].active())
                        {
                            npc.ai[0] = -1f;
                        }
                        else if (Main.tile[num268, num269 + 2].active())
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                    else
                    {
                        npc.ai[0] = 1f;
                    }
                    if ((double)npc.velocity.Y > 1.2 || (double)npc.velocity.Y < -1.2)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.99f;
                        return;
                    }
                }
            }
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6400f)
            {
                npc.active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] == 5f)
            {
                return false;
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] == 5f)
            {
                Color color = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16,
                    (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Player player = Main.player[npc.target];
                player.invis = true;
                player.aggro = -750;
                if (player.gravDir == -1f)
                {
                    spriteEffects |= SpriteEffects.FlipVertically;
                }
                Main.spriteBatch.Draw(Main.npcTexture[npc.type],
                    new Vector2((float)(player.direction * 4), player.gfxOffY) + ((player.gravDir == 1f) ? player.Top : player.Bottom) - Main.screenPosition,
                    new Microsoft.Xna.Framework.Rectangle?(npc.frame), npc.GetAlpha(color), npc.rotation, npc.frame.Size() / 2f, npc.scale, spriteEffects, 0f);
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
            npc.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if ((spawnInfo.player.Calamity().ZoneAbyssLayer3 || spawnInfo.player.Calamity().ZoneAbyssLayer4) && spawnInfo.water && !NPC.AnyNPCs(ModContent.NPCType<ColossalSquid>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ItemID.BlackInk, 3, 5);
            int minCells = Main.expertMode ? 31 : 26;
            int maxCells = Main.expertMode ? 45 : 38;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<DepthCells>(), CalamityWorld.downedCalamitas, 0.5f, minCells, maxCells);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<CalamarisLament>(), CalamityWorld.downedPolterghast, 3, 1, 1);
            DropHelper.DropItemChance(npc, ModContent.ItemType<InkBomb>(), 10, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ColossalSquid"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ColossalSquid2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ColossalSquid3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ColossalSquid4"), 1f);
            }
        }
    }
}
