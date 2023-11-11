using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class Gnasher : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 25;
            NPC.width = 50;
            NPC.height = 36;
            NPC.defense = 30;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 35;
            NPC.knockBackResist = 0.25f;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.Crab;
            NPC.value = Item.buyPrice(0, 0, 0, 60);
            NPC.HitSound = SoundID.NPCHit50;
            NPC.DeathSound = SoundID.NPCDeath54;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<GnasherBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Gnasher")
            });
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? -1 : 1;
            float targetDist = (Main.player[NPC.target].Center - NPC.Center).Length();
            targetDist *= 0.0025f;
            if ((double)targetDist > 1.5)
            {
                targetDist = 1.5f;
            }
            float maxVelocity;
            if (Main.expertMode)
            {
                maxVelocity = 2.5f - targetDist;
            }
            else
            {
                maxVelocity = 2.25f - targetDist;
            }
            maxVelocity *= (CalamityWorld.death ? 1.2f : CalamityWorld.revenge ? 1f : 0.8f);
            if (NPC.velocity.X < -maxVelocity || NPC.velocity.X > maxVelocity)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity *= 0.8f;
                }
            }
            else if (NPC.velocity.X < maxVelocity && NPC.direction == 1)
            {
                NPC.velocity.X = NPC.velocity.X + 1f;
                if (NPC.velocity.X > maxVelocity)
                {
                    NPC.velocity.X = maxVelocity;
                }
            }
            else if (NPC.velocity.X > -maxVelocity && NPC.direction == -1)
            {
                NPC.velocity.X = NPC.velocity.X - 1f;
                if (NPC.velocity.X < -maxVelocity)
                {
                    NPC.velocity.X = -maxVelocity;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
            {
                return 0f;
            }
            if (spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0.2f;
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddIf(() => Main.hardMode, ItemID.TurtleShell, 10);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Gnasher").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Gnasher2").Type, 1f);
                }
            }
        }
    }
}
