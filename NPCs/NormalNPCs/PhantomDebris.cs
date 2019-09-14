using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomDebris : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantom Debris");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.damage = 30;
			npc.width = 44; //324
			npc.height = 22; //216
			npc.defense = 35;
			npc.lifeMax = 80;
            npc.aiStyle = 3;
            aiType = 67;
            npc.knockBackResist = 0.5f;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath36;
			banner = npc.type;
			bannerItem = mod.ItemType("PhantomDebrisBanner");
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
            float num78 = 1f;
            float num79 = (Main.player[npc.target].Center - npc.Center).Length();
            num79 *= 0.0025f;
            if ((double)num79 > 1.5)
            {
                num79 = 1.5f;
            }
            if (Main.expertMode)
            {
                num78 = 4f - num79;
            }
            else
            {
                num78 = 3f - num79;
            }
            num78 *= 0.8f;
            if (npc.velocity.X < -num78 || npc.velocity.X > num78)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= 0.8f;
                }
            }
            else if (npc.velocity.X < num78 && npc.direction == 1)
            {
                npc.velocity.X = npc.velocity.X + 1.5f;
                if (npc.velocity.X > num78)
                {
                    npc.velocity.X = num78;
                }
            }
            else if (npc.velocity.X > -num78 && npc.direction == -1)
            {
                npc.velocity.X = npc.velocity.X - 1.5f;
                if (npc.velocity.X < -num78)
                {
                    npc.velocity.X = -num78;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.GetCalamityPlayer().ZoneAbyss ||
				spawnInfo.player.GetCalamityPlayer().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Underground.Chance * 0.02f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(mod.BuffType("MarkedforDeath"), 120);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
		{
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AncientBoneDust"));
            if (NPC.downedMoonlord)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Phantoplasm"));
            }
		}
	}
}
