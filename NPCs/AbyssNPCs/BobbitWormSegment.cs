using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
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
            npc.width = 26; //324
            npc.height = 26; //216
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
            CalamityGlobalNPC.bobbitWormBottom = npc.whoAmI;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[0] == 0f)
                {
                    npc.localAI[0] = 1f;
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<BobbitWormHead>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                }
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<BobbitWormHead>()))
            {
                npc.active = false;
                npc.netUpdate = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer4 && spawnInfo.water)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<BobbitWormSegment>()))
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
