using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Frogfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.chaseable = false;
            NPC.damage = 25;
            NPC.width = 60;
            NPC.height = 50;
            NPC.defense = 10;
            NPC.lifeMax = 80;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 80);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FrogfishBanner>();
            NPC.chaseable = false;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Frogfish")
            });
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            int alphaControl = 200;
            if (NPC.ai[2] == 0f)
            {
                NPC.alpha = alphaControl;
                NPC.TargetClosest(true);
                if (!Main.player[NPC.target].dead && (Main.player[NPC.target].Center - NPC.Center).Length() < 170f &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.ai[2] = -16f;
                }
                if (NPC.velocity.X != 0f || NPC.velocity.Y < 0f || NPC.velocity.Y > 2f || NPC.justHit)
                {
                    NPC.ai[2] = -16f;
                }
                return;
            }
            if (NPC.ai[2] < 0f)
            {
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= alphaControl / 16;
                    if (NPC.alpha < 0)
                    {
                        NPC.alpha = 0;
                    }
                }
                NPC.ai[2] += 1f;
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[2] = 1f;
                    NPC.velocity.X = (float)(NPC.direction * 2);
                }
                return;
            }
            NPC.alpha = 0;
            if (NPC.ai[2] == 1f)
            {
                NPC.chaseable = true;
                CalamityAI.PassiveSwimmingAI(NPC, Mod, 0, 0f, 0.15f, 0.15f, 3.5f, 1.5f, 0.1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Venom, 180, true);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!NPC.wet && !NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<AnechoicCoating>(), 2);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Frogfish").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Frogfish2").Type, 1f);
                }
            }
        }
    }
}
