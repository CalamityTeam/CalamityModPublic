using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.NPCs.AcidRain
{
    public class Radiator : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Snail;
            NPC.damage = 10;
            NPC.width = 24;
            NPC.height = 24;
            NPC.defense = 5;
            NPC.lifeMax = 50;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.damage = 60;
                NPC.lifeMax = 3250;
                NPC.defense = 20;
            }
            else if (DownedBossSystem.downedAquaticScourge)
            {
                NPC.damage = 30;
                NPC.lifeMax = 130;
                NPC.defense = 10;
            }

            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            AIType = NPCID.GlowingSnail;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<RadiatorBanner>();
            NPC.catchItem = (short)ModContent.ItemType<RadiatingCrystal>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            { 
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Radiator")
            });
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.3f, 1.5f, 0.3f);

			if (Main.netMode != NetmodeID.Server)
			{
				int auraSize = 200; //roughly 12 blocks (half the size of Wither Beast aura)
				Player player = Main.player[Main.myPlayer];
				if (!player.dead && player.active && (player.Center - NPC.Center).Length() < auraSize && !player.creativeGodMode)
				{
					player.AddBuff(ModContent.BuffType<Irradiated>(), 3, false);
					player.AddBuff(BuffID.Poisoned, 2, false);
					if (DownedBossSystem.downedPolterghast)
					{
						player.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 3, false);
						player.AddBuff(BuffID.Venom, 2, false);
					}
				}
			}
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * 2)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulphuricScale>(), 2, 1, 3);
        }
    }
}
