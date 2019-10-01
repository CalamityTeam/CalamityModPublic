using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Perforator
{
    public class PerforatorTailSmall : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Perforator");
		}

		public override void SetDefaults()
		{
			npc.damage = 10;
			npc.npcSlots = 5f;
			npc.width = 40;
			npc.height = 34;
			npc.defense = 8;
			npc.lifeMax = 1250;
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 600000 : 500000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.aiStyle = 6;
            aiType = -1;
			npc.knockBackResist = 0f;
			npc.scale = 0.9f;
			npc.alpha = 255;
			npc.buffImmune[mod.BuffType("GlacialState")] = true;
			npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
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
			if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
				}
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SmallPerf3"), 1f);
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

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(mod.BuffType("BurningBlood"), 60, true);
            player.AddBuff(BuffID.Bleeding, 60, true);
        }
    }
}
