using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SunkenSeaNPCs
{
    public class GhostBellSmall : ModNPC
	{
		public bool hasBeenHit = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Baby Ghost Bell");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 0.1f;
			npc.noGravity = true;
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 28;
			npc.height = 36;
			npc.defense = 0;
			npc.lifeMax = 5;
			npc.knockBackResist = 1f;
			npc.alpha = 100;
			npc.HitSound = SoundID.NPCHit25;
			npc.DeathSound = SoundID.NPCDeath28;
			banner = npc.type;
			bannerItem = mod.ItemType("GhostBellSmallBanner");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.chaseable);
			writer.Write(hasBeenHit);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.chaseable = reader.ReadBoolean();
			hasBeenHit = reader.ReadBoolean();
		}

		public override void AI()
		{
			Lighting.AddLight(npc.Center, 0f, ((255 - npc.alpha) * 1f) / 255f, ((255 - npc.alpha) * 1f) / 255f);
			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				npc.velocity.Y = -3f;
				npc.netUpdate = true;
			}
			if (npc.wet)
			{
				npc.noGravity = true;
				if (npc.velocity.Y < 0f)
				{
					npc.velocity.Y += 0.1f;
				}
				if (npc.velocity.Y > 0f)
				{
					npc.velocity.Y = 0f;
				}
			}
			else
			{
				npc.noGravity = false;
			}
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (projectile.minion)
			{
				return hasBeenHit;
			}
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 0.15f;
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).clamity)
			{
				return SpawnCondition.CaveJellyfish.Chance * 1.5f;
			}
			return 0f;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Vector2 vector = center - Main.screenPosition;
			vector -= new Vector2((float)mod.GetTexture("NPCs/SunkenSeaNPCs/GhostBellSmallGlow").Width, (float)(mod.GetTexture("NPCs/SunkenSeaNPCs/GhostBellSmallGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightBlue);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/SunkenSeaNPCs/GhostBellSmallGlow"), vector,
				new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 2; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}
