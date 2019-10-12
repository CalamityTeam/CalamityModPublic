using CalamityMod.Utilities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AbyssNPCs
{
    [AutoloadBossHead]
    public class AquaticScourgeHead : ModNPC
    {
        private bool detectsPlayer = false;
        private const int minLength = 30;
        private const int maxLength = 31;
        private float speed = 5f; //10
        private float turnSpeed = 0.08f; //0.15
        private bool TailSpawned = false;
        private bool despawning = false;
        private bool charging = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 16f;
            npc.damage = 80;
            npc.width = 100;
            npc.height = 90;
            npc.defense = 10;
            npc.Calamity().RevPlusDR(0.1f);
            npc.aiStyle = -1;
            aiType = -1;
            npc.lifeMax = CalamityWorld.revenge ? 85000 : 73000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 100000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 4300000 : 4000000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 12, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            bossBag = mod.ItemType("AquaticScourgeBag");
            if (Main.expertMode)
            {
                npc.scale = 1.15f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawning);
            writer.Write(detectsPlayer);
            writer.Write(npc.chaseable);
            writer.Write(charging);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawning = reader.ReadBoolean();
            detectsPlayer = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
            charging = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (npc.justHit || (double)npc.life <= (double)npc.lifeMax * 0.99 || CalamityWorld.bossRushActive)
            {
                detectsPlayer = true;
                npc.damage = npc.defDamage;
                npc.boss = true;
                Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AquaticScourge");
                else
                    music = MusicID.Boss2;
            }
            else
            {
                npc.damage = 0;
            }
            npc.chaseable = detectsPlayer;
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol = 0;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            if (num36 % 2 == 0)
                            {
                                lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AquaticScourgeBody"), npc.whoAmI);
                            }
                            else
                            {
                                lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AquaticScourgeBodyAlt"), npc.whoAmI);
                            }
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AquaticScourgeTail"), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (detectsPlayer)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= ((npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 120f : 180f))
                    {
                        int npcPoxX = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                        int npcPoxY = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                        if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 300f &&
                            !Main.tile[npcPoxX, npcPoxY].active())
                        {
                            npc.localAI[0] = 0f;
                            npc.TargetClosest(true);
                            npc.netUpdate = true;
                            int random = Main.rand.Next(3);
                            Main.PlaySound(3, (int)npc.Center.X, (int)npc.Center.Y, 8);
                            Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                            if (random == 0 && NPC.CountNPCS(mod.NPCType("AquaticSeekerHead")) < 1)
                            {
                                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("AquaticSeekerHead"));
                            }
                            else if (random == 1 && NPC.CountNPCS(mod.NPCType("AquaticUrchin")) < 3)
                            {
                                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("AquaticUrchin"));
                            }
                            else if (NPC.CountNPCS(mod.NPCType("AquaticParasite")) < 8)
                            {
                                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("AquaticParasite"));
                            }
                        }
                    }
                    npc.localAI[1] += 1f;
                    if (Main.player[npc.target].gravDir == -1f)
                    {
                        npc.localAI[1] += 2f;
                    }
                    if (npc.localAI[1] >= (CalamityWorld.bossRushActive ? 270f : 390f))
                    {
                        int npcPoxX = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                        int npcPoxY = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                        if (!Main.tile[npcPoxX, npcPoxY].active() && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 300f)
                        {
                            npc.localAI[1] = 0f;
                            npc.TargetClosest(true);
                            npc.netUpdate = true;
                            BarfShitUp();
                        }
                    }
                }
            }
            bool notOcean = Main.player[npc.target].position.Y < 800f ||
                (double)Main.player[npc.target].position.Y > Main.worldSurface * 16.0 ||
                (Main.player[npc.target].position.X > 6400f && Main.player[npc.target].position.X < (float)(Main.maxTilesX * 16 - 6400));
            if (Main.player[npc.target].dead || (notOcean && !CalamityWorld.bossRushActive))
            {
                despawning = true;
                npc.TargetClosest(false);
                npc.velocity.Y = npc.velocity.Y + 2f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 2f;
                }
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].aiStyle == npc.aiStyle)
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }
            else
            {
                despawning = false;
            }
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            if (!NPC.AnyNPCs(mod.NPCType("AquaticScourgeTail")))
            {
                npc.active = false;
            }
            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            int num42 = -1;
            int num43 = (int)(Main.player[npc.target].Center.X / 16f);
            int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
            for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
            {
                for (int num46 = num44; num46 <= num44 + 15; num46++)
                {
                    if (WorldGen.SolidTile2(num45, num46))
                    {
                        num42 = num46;
                        break;
                    }
                }
                if (num42 > 0)
                {
                    break;
                }
            }
            if (num42 > 0)
            {
                num42 *= 16;
                float num47 = (float)(num42 + (notOcean ? 800 : 400)); //800
                if (!detectsPlayer)
                {
                    num192 = num47;
                    if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < (notOcean ? 500f : 400f)) //500
                    {
                        if (npc.velocity.X > 0f)
                        {
                            num191 = Main.player[npc.target].Center.X + (notOcean ? 600f : 480f); //600
                        }
                        else
                        {
                            num191 = Main.player[npc.target].Center.X - (notOcean ? 600f : 480f); //600
                        }
                    }
                }
            }
            if (detectsPlayer)
            {
                num188 = CalamityWorld.revenge ? 15f : 13f;
                num189 = 0.16f;
                if (notOcean)
                {
                    num188 = 17f;
                    num189 = 0.18f;
                }
                if (Main.player[npc.target].gravDir == -1f)
                {
                    num188 = 20f;
                    num189 = 0.2f;
                }
                if (CalamityWorld.bossRushActive)
                {
                    num188 = 22f;
                    num189 = 0.24f;
                }
            }
            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = npc.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num49;
                }
            }
            if (!detectsPlayer)
            {
                for (int num51 = 0; num51 < 200; num51++)
                {
                    if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
                    {
                        Vector2 vector3 = Main.npc[num51].Center - npc.Center;
                        if (vector3.Length() < 400f)
                        {
                            vector3.Normalize();
                            vector3 *= 1000f;
                            num191 -= vector3.X;
                            num192 -= vector3.Y;
                        }
                    }
                }
            }
            else
            {
                for (int num52 = 0; num52 < 200; num52++)
                {
                    if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
                    {
                        Vector2 vector4 = Main.npc[num52].Center - npc.Center;
                        if (vector4.Length() < 60f)
                        {
                            vector4.Normalize();
                            vector4 *= 200f;
                            num191 -= vector4.X;
                            num192 -= vector4.Y;
                        }
                    }
                }
            }
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            float num196 = System.Math.Abs(num191);
            float num197 = System.Math.Abs(num192);
            float num198 = num188 / num193;
            num191 *= num198;
            num192 *= num198;
            if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
            {
                if (npc.velocity.X < num191)
                {
                    npc.velocity.X = npc.velocity.X + num189;
                }
                else
                {
                    if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                if (npc.velocity.Y < num192)
                {
                    npc.velocity.Y = npc.velocity.Y + num189;
                }
                else
                {
                    if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189;
                    }
                }
                if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                    }
                }
                if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 2f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 2f;
                    }
                }
            }
            else
            {
                if (num196 > num197)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                    }
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                    }
                    else if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                    }
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                }
            }
            npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
        }

        public void BarfShitUp()
        {
            Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 13);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 valueBoom = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float spreadBoom = 15f * 0.0174f;
                double startAngleBoom = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spreadBoom / 2;
                double deltaAngleBoom = spreadBoom / 8f;
                double offsetAngleBoom;
                int iBoom;
                int damageBoom = Main.expertMode ? 23 : 28;
                float velocity = CalamityWorld.revenge ? 7.5f : 6.5f;
                if (CalamityWorld.bossRushActive)
                    velocity = 11f;

                for (iBoom = 0; iBoom < 15; iBoom++)
                {
                    int projectileType = Main.rand.NextBool(2) ? mod.ProjectileType("SandTooth") : mod.ProjectileType("SandBlast");
                    if (projectileType == mod.ProjectileType("SandTooth"))
                    {
                        damageBoom = Main.expertMode ? 25 : 30;
                    }
                    offsetAngleBoom = startAngleBoom + deltaAngleBoom * (iBoom + iBoom * iBoom) / 2f + 32f * iBoom;
                    int boom1 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(Math.Sin(offsetAngleBoom) * velocity), (float)(Math.Cos(offsetAngleBoom) * velocity), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                    int boom2 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(-Math.Sin(offsetAngleBoom) * velocity), (float)(-Math.Cos(offsetAngleBoom) * velocity), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                }
                damageBoom = Main.expertMode ? 28 : 33;
                int num320 = Main.rand.Next(5, 9);
                int num3;
                for (int num321 = 0; num321 < num320; num321 = num3 + 1)
                {
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(50, 401) * (CalamityWorld.bossRushActive ? 0.02f : 0.01f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, vector15.X, vector15.Y, mod.ProjectileType("SandPoisonCloud"), damageBoom, 0f, Main.myPlayer, 0f, (float)Main.rand.Next(-45, 1));
                    num3 = num321;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
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

            return minDist <= 50f;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if ((projectile.penetrate == -1 || projectile.penetrate > 1) && !projectile.minion)
                damage = (int)((double)damage * 0.5);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return detectsPlayer;
            }
            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water)
            {
                if (!NPC.AnyNPCs(mod.NPCType("AquaticScourgeHead")))
                    return 0.01f;
            }
            return 0f;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = mod.ItemType("SulphurousSand");
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                mod.NPCType("AquaticScourgeHead"),
                mod.NPCType("AquaticScourgeBody"),
                mod.NPCType("AquaticScourgeBodyAlt"),
                mod.NPCType("AquaticScourgeTail"));
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ItemID.GreaterHealingPotion, 8, 14);
            // there is no Aquatic Scourge trophy yet
            DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeAquaticScourge"), true, !CalamityWorld.downedAquaticScourge);
            DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeSulphurSea"), true, !CalamityWorld.downedAquaticScourge);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedAquaticScourge, 4, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemCondition(npc, ItemID.SoulofSight, Main.hardMode, 20, 40);
                DropHelper.DropItem(npc, mod.ItemType("VictoryShard"), 11, 20);
                DropHelper.DropItem(npc, ItemID.Coral, 5, 9);
                DropHelper.DropItem(npc, ItemID.Seashell, 5, 9);
                DropHelper.DropItem(npc, ItemID.Starfish, 5, 9);

                // Weapons (Hardmode only)
                DropHelper.DropItemCondition(npc, mod.ItemType("SubmarineShocker"), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, mod.ItemType("Barinautical"), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, mod.ItemType("Downpour"), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, mod.ItemType("DeepseaStaff"), Main.hardMode, 4, 1, 1);

                // Equipment
                DropHelper.DropItemChance(npc, mod.ItemType("AeroStone"), 9);

                // Vanity
                // there is no Aquatic Scourge mask yet

                // Fishing
                DropHelper.DropItemChance(npc, ItemID.HighTestFishingLine, 12);
                DropHelper.DropItemChance(npc, ItemID.AnglerTackleBag, 12);
                DropHelper.DropItemChance(npc, ItemID.TackleBox, 12);
                DropHelper.DropItemChance(npc, ItemID.AnglerEarring, 9);
                DropHelper.DropItemChance(npc, ItemID.FishermansGuide, 9);
                DropHelper.DropItemChance(npc, ItemID.WeatherRadio, 9);
                DropHelper.DropItemChance(npc, ItemID.Sextant, 9);
                DropHelper.DropItemChance(npc, ItemID.AnglerHat, 4);
                DropHelper.DropItemChance(npc, ItemID.AnglerVest, 4);
                DropHelper.DropItemChance(npc, ItemID.AnglerPants, 4);
                DropHelper.DropItemChance(npc, ItemID.FishingPotion, 4, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.SonarPotion, 4, 2, 3);
                DropHelper.DropItemChance(npc, ItemID.CratePotion, 4, 2, 3);
                DropHelper.DropItemCondition(npc, ItemID.GoldenBugNet, NPC.downedBoss3, 15, 1, 1);
            }

            // Mark Aquatic Scourge as dead
            CalamityWorld.downedAquaticScourge = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASHead"), 1f);
            }
        }

        public override bool CheckActive()
        {
            if (detectsPlayer && !Main.player[npc.target].dead && !despawning)
            {
                return false;
            }
            if (npc.timeLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = (int)npc.ai[0]; k > 0; k = (int)Main.npc[k].ai[0])
                {
                    if (Main.npc[k].active)
                    {
                        Main.npc[k].active = false;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            Main.npc[k].life = 0;
                            Main.npc[k].netSkip = -1;
                            NetMessage.SendData(23, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 360, true);
            player.AddBuff(BuffID.Venom, 360, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(mod.BuffType("MarkedforDeath"), 180);
            }
        }
    }
}
