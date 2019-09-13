using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.AstrumDeus
{
	public class AstrumDeusProbe2 : ModNPC
	{
		public int timer = 0;
		public bool start = true;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus Probe");
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.width = 30;
			npc.height = 30;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.chaseable = false;
			npc.dontTakeDamage = true;
			npc.damage = 0;
			npc.defense = 0;
			npc.lifeMax = 100;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
		}

		public override bool PreAI()
		{
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			if (start)
			{
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
				}
				npc.ai[1] = npc.ai[0];
				start = false;
			}
			npc.TargetClosest(true);
			Vector2 direction = Main.player[npc.target].Center - npc.Center;
			direction.Normalize();
			direction *= 9f;
			npc.rotation = direction.ToRotation();
			npc.localAI[0] += 1f;
			if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 360f)
			{
				npc.localAI[0] = 0f;
				int num8 = expertMode ? 42 : 55;
				Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction.X * 5f, direction.Y * 5f, mod.ProjectileType("DeusMine"), num8, 0f, Main.myPlayer, 0f, 0f);
			}
			bool anySmallDeusHeads = NPC.AnyNPCs(mod.NPCType("AstrumDeusHead"));
			if (CalamityGlobalNPC.astrumDeusHeadMain < 0 || !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return false;
			}
			Player player = Main.player[npc.target];
			int npcType = (anySmallDeusHeads ? mod.NPCType("AstrumDeusHead") : mod.NPCType("AstrumDeusHeadSpectral"));
			NPC parent = Main.npc[NPC.FindFirstNPC(npcType)];
			double deg = (double)npc.ai[1];
			double rad = deg * (Math.PI / 180);
			double dist = 150;
			npc.position.X = parent.Center.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
			npc.position.Y = parent.Center.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;
			npc.ai[1] += 2f;
			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			if (npc.velocity != Vector2.Zero)
			{
				Texture2D texture = Main.npcTexture[npc.type];
				Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
				for (int i = 1; i < npc.oldPos.Length; ++i)
				{
					Vector2 vector2_2 = npc.oldPos[i];
					Microsoft.Xna.Framework.Color color2 = Color.White * npc.Opacity;
					color2.R = (byte)(0.5 * (double)color2.R * (double)(10 - i) / 20.0);
					color2.G = (byte)(0.5 * (double)color2.G * (double)(10 - i) / 20.0);
					color2.B = (byte)(0.5 * (double)color2.B * (double)(10 - i) / 20.0);
					color2.A = (byte)(0.5 * (double)color2.A * (double)(10 - i) / 20.0);
					Main.spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(npc.oldPos[i].X - Main.screenPosition.X + (npc.width / 2),
						npc.oldPos[i].Y - Main.screenPosition.Y + npc.height / 2), new Rectangle?(npc.frame), color2, npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0.0f);
				}
			}
			return true;
		}
	}
}
