using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
	public class PerforatorBodyMedium : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator");
        }

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 40;
            npc.height = 40;
            npc.defense = 6;
			npc.LifeMaxNERB(150, 180, 7000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
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

			if (CalamityWorld.malice || BossRushEvent.BossRushActive)
				npc.scale = 1.25f;
			else if (CalamityWorld.death)
				npc.scale = 1.2f;
			else if (CalamityWorld.revenge)
				npc.scale = 1.15f;
			else if (Main.expertMode)
				npc.scale = 1.1f;

			npc.Calamity().SplittingWorm = true;

			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

        public override void AI()
        {
			npc.realLife = -1;

			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			if (Main.player[npc.target].dead)
				npc.TargetClosest(false);

			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
					npc.alpha = 0;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.ai[0] == 0f)
				{
					if (npc.ai[2] > 0f)
						npc.ai[0] = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + npc.height), npc.type, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					else
						npc.ai[0] = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + npc.height), ModContent.NPCType<PerforatorTailMedium>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);

					Main.npc[(int)npc.ai[0]].ai[1] = npc.whoAmI;
					Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
					npc.netUpdate = true;
				}

				// Splitting effect
				if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
				if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle)
				{
					npc.type = ModContent.NPCType<PerforatorHeadMedium>();
					int whoAmI = npc.whoAmI;
					float num25 = npc.life / (float)npc.lifeMax;
					float num26 = npc.ai[0];
					npc.SetDefaultsKeepPlayerInteraction(npc.type);
					npc.life = (int)(npc.lifeMax * num25);
					npc.ai[0] = num26;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					npc.whoAmI = whoAmI;
				}
				if (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle)
				{
					int whoAmI2 = npc.whoAmI;
					float num27 = npc.life / (float)npc.lifeMax;
					float num28 = npc.ai[1];
					npc.SetDefaultsKeepPlayerInteraction(npc.type);
					npc.life = (int)(npc.lifeMax * num27);
					npc.ai[1] = num28;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					npc.whoAmI = whoAmI2;
				}

				if (!npc.active && Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
			}

			Vector2 vector2 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num39 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
			float num40 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);

			num39 = (int)(num39 / 16f) * 16;
			num40 = (int)(num40 / 16f) * 16;
			vector2.X = (int)(vector2.X / 16f) * 16;
			vector2.Y = (int)(vector2.Y / 16f) * 16;
			num39 -= vector2.X;
			num40 -= vector2.Y;
			float num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);

			if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
			{
				try
				{
					vector2 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					num39 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector2.X;
					num40 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector2.Y;
				}
				catch
				{
				}

				npc.rotation = (float)Math.Atan2(num40, num39) + MathHelper.PiOver2;
				num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
				int num53 = npc.width;
				num53 = (int)(num53 * npc.scale);
				num52 = (num52 - num53) / num52;
				num39 *= num52;
				num40 *= num52;
				npc.velocity = Vector2.Zero;
				npc.position.X += num39;
				npc.position.Y += num40;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorBodyMediumGlow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			return false;
		}

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf3"), npc.scale);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BurningBlood>(), 120, true);
        }
    }
}
