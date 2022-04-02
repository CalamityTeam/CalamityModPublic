using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class CryoSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryo Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            aiType = NPCID.DungeonSlime;
            npc.damage = 30;
            npc.width = 40;
            npc.height = 30;
            npc.defense = 8;
            npc.lifeMax = 120;
            npc.knockBackResist = 0f;
            animationType = NPCID.CorruptSlime;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.alpha = 50;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CryoSlimeBanner>();
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToCold = false;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !CalamityWorld.downedCryogen || (!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1) || spawnInfo.player.Calamity().ZoneAbyss ||
                spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.08f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 120, true);
            player.AddBuff(BuffID.Chilled, 90, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            // TODO -- This dust was an invalid dust. Replaced with a random dust.
            int dustType = 91;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<CryonicOre>(), 10, 26);
        }
    }
}
