using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class IrradiatedSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Irradiated Slime");
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 50;
			npc.width = 40;
			npc.height = 30;
			npc.defense = 10;
			npc.lifeMax = 300;
			npc.knockBackResist = 0f;
			animationType = 81;
			npc.value = Item.buyPrice(0, 0, 5, 0);
			npc.alpha = 50;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("IrradiatedSlimeBanner");
		}

		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.6f, 0.8f, 0.6f);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !spawnInfo.player.GetCalamityPlayer().ZoneSulphur || !Main.raining)
			{
				return 0f;
			}
			return 0.05f;
		}

		/*public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Vector2 vector = center - Main.screenPosition;
			vector -= new Vector2((float)mod.GetTexture("NPCs/NormalNPCs/IrradiatedSlimeGlow").Width, (float)(mod.GetTexture("NPCs/NormalNPCs/IrradiatedSlimeGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightYellow);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/NormalNPCs/IrradiatedSlimeGlow"), vector,
				new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
		}*/

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 75, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 75, hitDirection, -1f, 0, default, 1f);
				}
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/IrradiatedSlime"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/IrradiatedSlime2"), 1f);
			}
		}

		public override void NPCLoot()
		{
			if (Main.rand.NextBool(10))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LeadCore"));
			}
		}
	}
}
