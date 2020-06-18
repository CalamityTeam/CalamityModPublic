using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Leviathan
{
	public class Parasea : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Parasea");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.damage = 50;
            npc.width = 90;
            npc.height = 20;
            npc.defense = 8;
            npc.lifeMax = 650;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 50000;
            }
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ParaseaBanner>();
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            float speed = revenge ? 16f : 13f;
            if (CalamityWorld.bossRushActive)
                speed = 24f;
            CalamityAI.DungeonSpiritAI(npc, mod, speed, MathHelper.Pi);
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur || (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas))
			{
				return 0f;
			}
			return SpawnCondition.OceanMonster.Chance * 0.06f;
		}

		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Wet, 60, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
