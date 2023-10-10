using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class AeroSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y -= 6;
            value.PortraitPositionYOverride = -36f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.damage = 18;
            NPC.width = 40;
            NPC.height = 30;
            NPC.defense = 6;
            NPC.lifeMax = 50;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.Slimer;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.alpha = 50;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AeroSlimeBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AeroSlime")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || (!DownedBossSystem.downedHiveMind && !DownedBossSystem.downedPerforator))
            {
                return 0f;
            }
            
            return SpawnCondition.Sky.Chance * 0.2f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 59, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 59, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<AerialiteOre>(), 1, 10, 26);

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = "YuH";
            }
        }
    }
}
