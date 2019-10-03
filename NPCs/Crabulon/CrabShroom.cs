using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Crabulon
{
    public class CrabShroom : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crab Shroom");
            Main.npcFrameCount[npc.type] = 4;
        }

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 25;
			npc.width = 14;
			npc.height = 14;
			npc.lifeMax = 25;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 90000;
            }
            aiType = -1;
			npc.knockBackResist = 0.75f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
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
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.2f, 0.4f);
            bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			float speed = revenge ? 1.5f : 1f;
			Player player = Main.player[npc.target];
			npc.velocity.Y = npc.velocity.Y + 0.02f;
			if (npc.velocity.Y > speed)
			{
				npc.velocity.Y = speed;
			}
			npc.TargetClosest(true);
			if (npc.position.X + (float)npc.width < player.position.X)
			{
				if (npc.velocity.X < 0f)
				{
					npc.velocity.X = npc.velocity.X * 0.98f;
				}
				npc.velocity.X = npc.velocity.X + (CalamityWorld.bossRushActive ? 0.2f : 0.1f);
			}
			else if (npc.position.X > player.position.X + (float)player.width)
			{
				if (npc.velocity.X > 0f)
				{
					npc.velocity.X = npc.velocity.X * 0.98f;
				}
				npc.velocity.X = npc.velocity.X - (CalamityWorld.bossRushActive ? 0.2f : 0.1f);
			}
			if (npc.velocity.X > (CalamityWorld.bossRushActive ? 15f : 5f) || npc.velocity.X < (CalamityWorld.bossRushActive ? -15f : -5f))
			{
				npc.velocity.X = npc.velocity.X * 0.97f;
			}
			npc.rotation = npc.velocity.X * 0.1f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 56, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 56, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}
