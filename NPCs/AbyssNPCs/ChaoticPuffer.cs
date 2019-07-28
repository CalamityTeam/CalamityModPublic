using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.AbyssNPCs
{
    public class ChaoticPuffer : ModNPC
	{
		public bool puffedUp = false;
		public bool puffing = false;
		public bool unpuffing = false;
		public int puffTimer = 0;
		public int puffingTimer = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaotic Puffer");
			Main.npcFrameCount[npc.type] = 11;
		}

		public override void SetDefaults()
		{
			npc.noGravity = true;
			npc.lavaImmune = true;
			npc.width = 78;
			npc.height = 70;
			npc.defense = 25;
			npc.lifeMax = 7500;
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.buffImmune[mod.BuffType("CrushDepth")] = true;
			npc.value = Item.buyPrice(0, 0, 30, 0);
			npc.HitSound = SoundID.NPCHit23;
			npc.DeathSound = SoundID.NPCDeath28;
			banner = npc.type;
			bannerItem = mod.ItemType("ChaoticPufferBanner");
		}

		public override void AI()
		{
			npc.TargetClosest(true);
			npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.03f;
			npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.03f;

			npc.damage = puffedUp ? (Main.expertMode ? 230 : 115) : 0;


			if (!puffing || !unpuffing)
			{
				++puffTimer;
			}

			if (puffTimer >= 300)
			{

				if (!puffedUp)
				{
					puffing = true;
				}
				else
				{
					unpuffing = true;
				}

				puffTimer = 0;

			}
			else if (puffing || unpuffing)
			{

				++puffingTimer;

				if (puffingTimer > 16 && puffing)
				{

					puffing = false;
					puffedUp = true;
					puffingTimer = 0;

				}
				else if (puffingTimer > 16 && unpuffing)
				{

					unpuffing = false;
					puffedUp = false;
					puffingTimer = 0;

				}

			}

			if (npc.velocity.X >= 1 || npc.velocity.X <= -1)
			{

				npc.velocity.X = npc.velocity.X * 0.97f;

			}

			if (npc.velocity.Y >= 1 || npc.velocity.Y <= -1)
			{

				npc.velocity.Y = npc.velocity.Y * 0.97f;

			}

			npc.rotation += npc.velocity.X * 0.05f;

		}

		public void Boom()
		{
			Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 14);
			if (Main.netMode != 1 && puffedUp)
			{
				int damageBoom = 100;
				int projectileType = mod.ProjectileType("PufferExplosion");
				int boom = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
			}
			npc.netUpdate = true;
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
			vector -= new Vector2((float)mod.GetTexture("NPCs/AbyssNPCs/ChaoticPufferGlow").Width, (float)(mod.GetTexture("NPCs/AbyssNPCs/ChaoticPufferGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Yellow);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/AbyssNPCs/ChaoticPufferGlow"), vector,
				new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 1.0;
			if (npc.frameCounter > 6.0)
			{
				npc.frameCounter = 0.0;
				if (!unpuffing)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
				}
				else
				{
					npc.frame.Y = npc.frame.Y - frameHeight;
				}
			}
			if (puffing)
			{
				if (npc.frame.Y < frameHeight * 3)
				{
					npc.frame.Y = frameHeight * 3;
				}
				if (npc.frame.Y > frameHeight * 6)
				{
					npc.frame.Y = frameHeight * 3;
				}
			}
			else if (unpuffing)
			{
				if (npc.frame.Y > frameHeight * 6)
				{
					npc.frame.Y = frameHeight * 6;
				}
				if (npc.frame.Y < frameHeight * 3)
				{
					npc.frame.Y = frameHeight * 6;
				}
			}
			else if (!puffedUp)
			{
				if (npc.frame.Y > frameHeight * 3)
				{
					npc.frame.Y = 0;
				}
			}
			else
			{
				if (npc.frame.Y < frameHeight * 7)
				{
					npc.frame.Y = frameHeight * 7;
				}
				if (npc.frame.Y > frameHeight * 10)
				{
					npc.frame.Y = frameHeight * 7;
				}
			}

		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyssLayer4 && spawnInfo.water)
			{
				return SpawnCondition.CaveJellyfish.Chance * 0.6f;
			}
			return 0f;
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(1000000) == 0 && CalamityWorld.revenge)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HalibutCannon"));
			}
			if (NPC.downedGolemBoss)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ChaoticOre"), Main.rand.Next(10, 27));
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("BrimstoneFlames"), 300);

			if (puffedUp)
			{
				Boom();
				npc.active = false;
			}
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			npc.velocity.X = projectile.velocity.X;
			npc.velocity.Y = projectile.velocity.Y;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				Boom();

				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}
