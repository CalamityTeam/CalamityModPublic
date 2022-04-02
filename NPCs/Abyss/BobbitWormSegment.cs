using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class BobbitWormSegment : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bobbit Worm");
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            npc.damage = 0;
            npc.alpha = 255;
            npc.width = 26;
            npc.height = 26;
            npc.defense = 0;
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.dontTakeDamage = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            if (npc.ai[0] == 0f)
            {
                for (int i = 0; i < CalamityGlobalNPC.bobbitWormBottom.Length; i++)
                {
                    if (CalamityGlobalNPC.bobbitWormBottom[i] == -1)
                    {
                        CalamityGlobalNPC.bobbitWormBottom[i] = npc.whoAmI;
                        npc.ai[0] = (float)i;
                        break;
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = 1f;
                    int spawnedNPC = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<BobbitWormHead>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    Main.npc[spawnedNPC].ai[2] = (float)CalamityGlobalNPC.bobbitWormBottom[(int)npc.ai[0]];
                    npc.ai[2] = (float)spawnedNPC;
                }
            }

            if (!Main.npc[(int)npc.ai[2]].active || Main.npc[(int)npc.ai[2]].life <= 0)
            {
                npc.active = false;
                npc.netUpdate = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer4 && spawnInfo.water)
            {
                if (CalamityGlobalNPC.bobbitWormBottom.Contains(-1))
                    return SpawnCondition.CaveJellyfish.Chance * 0.3f;
            }
            return 0f;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
