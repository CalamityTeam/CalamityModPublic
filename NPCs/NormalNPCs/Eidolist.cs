using CalamityMod.BiomeManagers;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.SummonItems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.PrimordialWyrm;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Eidolist : ModNPC
    {
        public bool hasBeenHit = false;

        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/EidolistDeath");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.75f,
                PortraitPositionYOverride = 16f
            };
            value.Position.Y += 10f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 50;
            NPC.width = 60;
            NPC.height = 80;
            NPC.lifeMax = 5000;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.Opacity = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = DeathSound;
            NPC.timeLeft = NPC.activeTime * 2;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<EidolistBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<AbyssLayer3Biome>().Type, ModContent.GetInstance<AbyssLayer4Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Eidolist")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }

            NPC.Opacity += 0.15f;
            if (NPC.Opacity > 1f)
                NPC.Opacity = 1f;

            Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), 0f, 0.4f, 0.5f);
            if (NPC.justHit || adultWyrmAlive)
            {
                hasBeenHit = true;
            }
            NPC.chaseable = hasBeenHit;
            if (!hasBeenHit)
            {
                if (NPC.collideX)
                {
                    NPC.velocity.X = NPC.velocity.X * -1f;
                    NPC.direction *= -1;
                    NPC.netUpdate = true;
                }
                if (NPC.collideY)
                {
                    NPC.netUpdate = true;
                    if (NPC.velocity.Y > 0f)
                    {
                        NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                        NPC.directionY = -1;
                        NPC.localAI[2] = -1f;
                    }
                    else if (NPC.velocity.Y < 0f)
                    {
                        NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                        NPC.directionY = 1;
                        NPC.localAI[2] = 1f;
                    }
                }
                NPC.velocity.X += NPC.direction * 0.1f;
                if (NPC.velocity.X < -2f || NPC.velocity.X > 2f)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.95f;
                }
                if (NPC.localAI[2] == -1f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    if (NPC.velocity.Y < -0.3f)
                    {
                        NPC.localAI[2] = 1f;
                    }
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    if (NPC.velocity.Y > 0.3f)
                    {
                        NPC.localAI[2] = -1f;
                    }
                }
                if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
                {
                    NPC.velocity.Y *= 0.95f;
                }
                return;
            }
            NPC.noTileCollide = true;
            float num1446 = adultWyrmAlive ? 14f : 7f;
            float num1447 = 480f;
            if (NPC.localAI[1] == 1f)
            {
                NPC.localAI[1] = 0f;
                if (Main.rand.NextBool(4))
                {
                    NPC.ai[0] = num1447;
                }
            }
            NPC.TargetClosest(true);
            NPC.rotation = Math.Abs(NPC.velocity.X) * (float)NPC.direction * 0.1f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            Vector2 value53 = NPC.Center + new Vector2((float)(NPC.direction * 20), 6f);
            Vector2 vector251 = Main.player[NPC.target].Center - value53;
            bool flag104 = Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1);

            if (NPC.justHit)
                NPC.localAI[0] = -90f;

            NPC.localAI[0] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 150f)
            {
                NPC.localAI[0] = -90f;
                NPC.netUpdate = true;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    float speed = adultWyrmAlive ? 10f : 5f;
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float xDist = Main.player[NPC.target].Center.X - vector.X + Main.rand.NextFloat(-10f, 10f);
                    float yDist = Main.player[NPC.target].Center.Y - vector.Y + Main.rand.NextFloat(-10f, 10f);
                    Vector2 targetVec = new Vector2(yDist, yDist);
                    float targetDist = targetVec.Length();
                    targetDist = speed / targetDist;
                    targetVec.X *= targetDist;
                    targetVec.Y *= targetDist;
                    int damage = adultWyrmAlive ? (Main.expertMode ? 150 : 200) : (Main.expertMode ? 30 : 40);
                    if (Main.rand.NextBool(2))
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, targetVec, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                    else
                    {
                        Vector2 vec = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center + Main.player[NPC.target].velocity * 20f);
                        if (vec.HasNaNs())
                        {
                            vec = new Vector2((float)NPC.direction, 0f);
                        }
                        for (int n = 0; n < 1; n++)
                        {
                            Vector2 vector4 = vec * 4f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vector4, ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }
                }
            }
            if (vector251.Length() > 400f || !flag104)
            {
                Vector2 value54 = vector251;
                if (value54.Length() > num1446)
                {
                    value54.Normalize();
                    value54 *= num1446;
                }
                int num1448 = 30;
                NPC.velocity = (NPC.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
            }
            else
            {
                NPC.velocity *= 0.98f;
            }
            if (NPC.ai[2] != 0f && NPC.ai[3] != 0f)
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int num1449 = 0; num1449 < 20; num1449++)
                {
                    int num1450 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 20, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[num1450];
                    dust.velocity *= 3f;
                    dust.noGravity = true;
                    dust.scale = 2.5f;
                }
                NPC.Center = new Vector2(NPC.ai[2] * 16f, NPC.ai[3] * 16f);
                NPC.velocity = Vector2.Zero;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int num1451 = 0; num1451 < 20; num1451++)
                {
                    int num1452 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 20, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[num1452];
                    dust.velocity *= 3f;
                    dust.noGravity = true;
                    dust.scale = 2.5f;
                }
            }
            NPC.ai[0] += 1f;
            if (NPC.ai[0] >= num1447 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[0] = 0f;
                Point point12 = NPC.Center.ToTileCoordinates();
                Point point13 = Main.player[NPC.target].Center.ToTileCoordinates();
                int num1453 = 20;
                int num1454 = 3;
                int num1455 = 10;
                int num1456 = 1;
                int num1457 = 0;
                bool flag106 = false;
                if (vector251.Length() > 2000f)
                {
                    flag106 = true;
                }
                while (!flag106 && num1457 < 100)
                {
                    num1457++;
                    int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
                    int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
                    if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].HasUnactuatedTile)
                    {
                        bool flag107 = true;
                        if (flag107 && Main.tile[num1458, num1459].LiquidType == LiquidID.Lava)
                        {
                            flag107 = false;
                        }
                        if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
                        {
                            flag107 = false;
                        }
                        if (flag107)
                        {
                            NPC.ai[2] = (float)num1458;
                            NPC.ai[3] = (float)num1459;
                            break;
                        }
                    }
                }
                NPC.netUpdate = true;
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
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            NPC.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // waffles% stipulation: eidolists are accessible by beating cal clone, even without beating WoF
            bool hardmodeOrCalClone = Main.hardMode || DownedBossSystem.downedCalamitasClone;
            if (!hardmodeOrCalClone || !spawnInfo.Player.InAbyss())
                return 0f;

            // Keep this as a separate if check, because it's a loop and we don't want to be checking it constantly.
            if (NPC.AnyNPCs(NPC.type))
                return 0f;

            if (spawnInfo.Player.Calamity().ZoneAbyssLayer3 && spawnInfo.Water)
                return Main.remixWorld ? 2.25f : 0.25f;

            if (spawnInfo.Player.Calamity().ZoneAbyssLayer4 && spawnInfo.Water)
                return Main.remixWorld ? 4.5f : 0.5f;

            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Eidolist").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Eidolist2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Eidolist3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Eidolist4").Type, NPC.scale);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Never drop anything if this Eidolon Wyrm is a minion during an AEW fight.
            var aewMinionCondition = npcLoot.DefineConditionalDropSet(PrimordialWyrmHead.CanMinionsDropThings);

            LeadingConditionRule notDuringCultistFight = new LeadingConditionRule(DropHelper.If(() => !NPC.LunarApocalypseIsUp));
            notDuringCultistFight.Add(ModContent.ItemType<EidolonTablet>(), 4);
            aewMinionCondition.Add(notDuringCultistFight);

            LeadingConditionRule postClone = new LeadingConditionRule(DropHelper.PostCal());
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Lumenyl>(), 1, 8, 10, 10, 14));
            aewMinionCondition.Add(postClone);

            LeadingConditionRule postPlant = new LeadingConditionRule(DropHelper.PostPlant());
            postPlant.Add(ItemID.Ectoplasm, 1, 3, 5);
            aewMinionCondition.Add(postPlant);
        }
    }
}
