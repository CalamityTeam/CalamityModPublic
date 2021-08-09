using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.DesertScourge
{
	public class DesertScourgeBodySmall : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Scourge");
        }

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 4;
            npc.lifeMax = 800;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 35000;
            }
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            npc.dontCountMe = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
			// Check if other segments are still alive, if not, die
			bool shouldDespawn = true;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<DesertScourgeHeadSmall>())
				{
					shouldDespawn = false;
					break;
				}
			}
			if (!shouldDespawn)
			{
				if (npc.ai[1] > 0f)
					shouldDespawn = false;
				else if (Main.npc[(int)npc.ai[1]].life > 0)
					shouldDespawn = false;
			}
			if (shouldDespawn)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
			}

			if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                float randomSpread = (float)(Main.rand.Next(-100, 100) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody"), 0.65f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody2"), 0.65f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody3"), 0.65f);
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 60, true);
        }
    }
}
