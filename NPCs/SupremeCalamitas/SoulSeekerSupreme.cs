using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace CalamityMod.NPCs.SupremeCalamitas
{
	public class SoulSeekerSupreme : ModNPC
    {
        private int timer = 0;
        private bool start = true;
        public NPC SCal => Main.npc[CalamityGlobalNPC.SCal];
        public Player Target => Main.player[npc.target];
        public Vector2 EyePosition => npc.Center + new Vector2(npc.spriteDirection == -1 ? 40f : -36f, 16f);
        public ref float RotationalDegreeOffset => ref npc.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Seeker");
            Main.npcFrameCount[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.width = 40;
            npc.height = 40;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.damage = 0;
            npc.defense = 60;
			npc.DR_NERD(0.25f);
			npc.LifeMaxNERB(Main.expertMode ? 24000 : 15000, 28000);
            npc.DeathSound = SoundID.DD2_SkeletonDeath;
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToCold = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter % 5 == 4)
                npc.frame.Y += frameHeight;
            if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type])
                npc.frame.Y = 0;
        }

        public override bool PreAI()
        {
            // Die if SCal is no longer present.
            if (CalamityGlobalNPC.SCal < 0 || !SCal.active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return false;
            }

            if (start)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                RotationalDegreeOffset = npc.ai[0];
                start = false;
            }

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Target.dead || !Target.active)
				npc.TargetClosest();

			// Target another player if the current player target is too far away
			if (!npc.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance200Tiles))
				npc.TargetClosest();

            npc.spriteDirection = (Target.Center.X < npc.Center.X).ToDirectionInt();

            timer++;
			int shootRate = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 120 : 180;
            if (timer > shootRate)
            {
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC seeker = Main.npc[i];
					if (seeker.type == npc.type)
					{
						if (seeker == npc)
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SCalSounds/BrimstoneShoot"), SCal.Center);
						break;
					}
				}
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
					int type = ModContent.ProjectileType<BrimstoneBarrage>();
					int damage = npc.GetProjectileDamage(type);
                    Vector2 shootVelocity = (Target.Center - EyePosition).SafeNormalize(Vector2.UnitY) * 9f;
					Projectile.NewProjectile(EyePosition, shootVelocity, type, damage, 1f, Main.myPlayer);
                }
                timer = 0;
                npc.netUpdate = true;
            }

            npc.position = SCal.Center - MathHelper.ToRadians(RotationalDegreeOffset).ToRotationVector2() * 300f - npc.Size * 0.5f;
            RotationalDegreeOffset += 0.5f;
            return false;
        }

		public override void NPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
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
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                for (int i = 1; i <= 5; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot($"Gores/SoulSeekerSupremeGores/SupremeSoulSeeker_Gore{i}"), npc.scale);
            }
        }

        public override bool CheckActive() => false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 2;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SoulSeekerSupremeGlow");
			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}
	}
}
