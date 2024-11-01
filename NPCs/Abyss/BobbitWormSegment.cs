using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Abyss
{
    [LongDistanceNetSync(SyncWith = typeof(BobbitWormHead))]
    public class BobbitWormSegment : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.BobbitWormHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.alpha = 255;
            NPC.width = 26;
            NPC.height = 26;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            if (NPC.ai[0] == 0f)
            {
                for (int i = 0; i < CalamityGlobalNPC.bobbitWormBottom.Length; i++)
                {
                    if (CalamityGlobalNPC.bobbitWormBottom[i] == -1)
                    {
                        CalamityGlobalNPC.bobbitWormBottom[i] = NPC.whoAmI;
                        NPC.ai[0] = (float)i;
                        break;
                    }
                }
            }

            if (NPC.ai[1] == 0f)
            {
                NPC.ai[1] = 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int spawnedNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BobbitWormHead>(), NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                    Main.npc[spawnedNPC].ai[2] = (float)CalamityGlobalNPC.bobbitWormBottom[(int)NPC.ai[0]];
                    NPC.ai[2] = (float)spawnedNPC;
                }
            }

            if (!Main.npc[(int)NPC.ai[2]].active || Main.npc[(int)NPC.ai[2]].life <= 0)
            {
                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneAbyssLayer4 && spawnInfo.Water)
            {
                if (CalamityGlobalNPC.bobbitWormBottom.Contains(-1))
                    return Main.remixWorld ? 8.25f : SpawnCondition.CaveJellyfish.Chance * 0.85f;
            }
            return 0f;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
