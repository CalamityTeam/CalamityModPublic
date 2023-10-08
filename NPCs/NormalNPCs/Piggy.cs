using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Critters;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class Piggy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.chaseable = false;
            NPC.damage = 0;
            NPC.width = 26;
            NPC.height = 26;
            NPC.lifeMax = 2000;
            NPC.aiStyle = NPCAIStyleID.Passive;
            AIType = NPCID.Squirrel;
            NPC.knockBackResist = 0.99f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PiggyBanner>();
            NPC.catchItem = (short)ModContent.ItemType<PiggyItem>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player is null || !player.active)
                    continue;

                if (NPC.Hitbox.Intersects(player.HitboxForBestiaryNearbyCheck))
                {
                    NPC nPC = new NPC();
                    nPC.SetDefaults(ModContent.NPCType<Piggy>());
                    Main.BestiaryTracker.Kills.RegisterKill(nPC);
                    break;
                }
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Piggy")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneSulphur || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.TownCritter.Chance * 0.005f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0f)
            {
                if (!NPC.IsABestiaryIconDummy)
                {
                    if (NPC.direction == 1)
                    {
                        NPC.spriteDirection = -1;
                    }
                    if (NPC.direction == -1)
                    {
                        NPC.spriteDirection = 1;
                    }

                    if (NPC.velocity.X == 0f)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0.0;
                        return;
                    }
                }
                NPC.frameCounter += NPC.IsABestiaryIconDummy ? 0.6f : Math.Abs(NPC.velocity.X) * 0.25f;
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type] - 1)
                {
                    NPC.frame.Y = frameHeight;
                }
            }
            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = frameHeight * 2;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemID.Bacon);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
