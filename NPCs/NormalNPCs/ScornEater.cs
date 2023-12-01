using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class ScornEater : ModNPC
    {

        public static readonly SoundStyle JumpSound = new("CalamityMod/Sounds/Custom/ScornJump");
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/ScornHurt"); 
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/ScornDeath");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.4f,
                PortraitScale = 0.67f,
                PortraitPositionYOverride = 4f
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.width = 160;
            NPC.height = 160;
            NPC.defense = 38;
            NPC.DR_NERD(0.05f);
            NPC.lifeMax = 9000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.lavaImmune = true;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.DeathSound = DeathSound;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ScornEaterBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ScornEater")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            if ((Main.player[NPC.target].position.Y > NPC.position.Y + (float)NPC.height && NPC.velocity.Y > 0f) || (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height && NPC.velocity.Y < 0f))
                NPC.noTileCollide = true;
            else
                NPC.noTileCollide = false;

            if (NPC.velocity.Y == 0f)
            {
                NPC.ai[2] += 1f;
                int decelerationDelay = 20;
                if (NPC.ai[1] == 0f)
                {
                    decelerationDelay = 12;
                }
                if (CalamityWorld.revenge)
                {
                    decelerationDelay /= 2;
                }
                if (CalamityWorld.death)
                {
                    decelerationDelay /= 2;
                }
                if (NPC.ai[2] < (float)decelerationDelay)
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                    return;
                }
                NPC.ai[2] = 0f;
                if (NPC.direction == 0)
                {
                    NPC.direction = -1;
                }
                NPC.spriteDirection = NPC.direction;
                NPC.ai[1] += 1f;
                NPC.ai[3] += 1f;
                if (NPC.ai[3] >= 4f)
                {
                    NPC.ai[3] = 0f;
                    NPC.noTileCollide = true;
                    if (NPC.ai[1] == 2f)
                    {
                        NPC.velocity.X = (float)NPC.direction * 15f;

                        if (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height)
                            NPC.velocity.Y = -12f;
                        else
                            NPC.velocity.Y = 12f;

                        NPC.ai[1] = 0f;
                    }
                    else
                    {
                        NPC.velocity.X = (float)NPC.direction * 21f;

                        if (Main.player[NPC.target].position.Y < NPC.position.Y + (float)NPC.height)
                            NPC.velocity.Y = -6f;
                        else
                            NPC.velocity.Y = 12f;
                    }
                    if (!Main.zenithWorld)
                    SoundEngine.PlaySound(JumpSound, NPC.Center);
                }
                NPC.netUpdate = true;
            }
            else
            {
                if (NPC.direction == 1 && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                    return;
                }
                if (NPC.direction == -1 && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (Math.Abs(NPC.velocity.X) <= 1f)
            {
                if (NPC.frameCounter > 9.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }
            else
            {
                if (NPC.frameCounter > 9.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = frameHeight * 5;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedMoonlord || spawnInfo.Player.Calamity().ZoneCalamity)
            {
                return 0f;
            }
            if (SpawnCondition.Underworld.Chance > 0f)
            {
                return SpawnCondition.Underworld.Chance / 4f;
            }
            return SpawnCondition.OverworldHallow.Chance / 4f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 7;
                SoundEngine.PlaySound(HitSound, NPC.Center);
            }
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScornEater").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScornEater2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScornEater3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScornEater4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScornEater5").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScornEater6").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<UnholyEssence>(), 1, 2, 4);
        }
    }
}
