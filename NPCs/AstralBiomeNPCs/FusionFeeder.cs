using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class FusionFeeder : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fusion Feeder");
            if (!Main.dedServ)
				glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/FusionFeederGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 120;
            npc.height = 24;
            npc.damage = 64;
            npc.aiStyle = -1;
            npc.lifeMax = 600;
            npc.defense = 22;
            npc.value = 900f;
            npc.knockBackResist = 0.7f;
            npc.behindTiles = true;
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit");
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");

            animationType = NPCID.SandShark;
        }
		
		public override void AI()
		{
			if (npc.direction == 0)
			{
				npc.TargetClosest(true);
			}
			Point pt = npc.Center.ToTileCoordinates();
			Tile tileSafely = Framing.GetTileSafely(pt);
			bool flag121 = tileSafely.nactive() && (TileID.Sets.Conversion.Sand[(int)tileSafely.type] || TileID.Sets.Conversion.Sandstone[(int)tileSafely.type] || TileID.Sets.Conversion.HardenedSand[(int)tileSafely.type]);
			flag121 |= npc.wet;
			bool flag122 = false;
			npc.TargetClosest(false);
			Vector2 vector224 = npc.targetRect.Center.ToVector2();
			if (Main.player[npc.target].velocity.Y > -0.1f && !Main.player[npc.target].dead && npc.Distance(vector224) > 150f)
			{
				flag122 = true;
			}
			if (npc.localAI[0] == -1f && !flag121)
			{
				npc.localAI[0] = 20f;
			}
			if (npc.localAI[0] > 0f)
			{
				npc.localAI[0] -= 1f;
			}
			if (flag121)
			{
				if (npc.soundDelay == 0)
				{
					float num1485 = npc.Distance(vector224) / 40f;
					if (num1485 < 10f)
					{
						num1485 = 10f;
					}
					if (num1485 > 20f)
					{
						num1485 = 20f;
					}
					npc.soundDelay = (int)num1485;
				}
				float arg_46CA6_0 = npc.ai[1];
				bool flag123 = false;
				pt = (npc.Center + new Vector2(0f, 24f)).ToTileCoordinates();
				tileSafely = Framing.GetTileSafely(pt.X, pt.Y - 2);
				if (tileSafely.nactive() && (TileID.Sets.Conversion.Sand[(int)tileSafely.type] || TileID.Sets.Conversion.Sandstone[(int)tileSafely.type] || TileID.Sets.Conversion.HardenedSand[(int)tileSafely.type]))
				{
					flag123 = true;
				}
				npc.ai[1] = (float)flag123.ToInt();
				if (npc.ai[2] < 30f)
				{
					npc.ai[2] += 1f;
				}
				if (flag122)
				{
					npc.TargetClosest(true);
					npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.15f;
					npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.15f;
					if (npc.velocity.X > 5f)
					{
						npc.velocity.X = 5f;
					}
					if (npc.velocity.X < -5f)
					{
						npc.velocity.X = -5f;
					}
					if (npc.velocity.Y > 3f)
					{
						npc.velocity.Y = 3f;
					}
					if (npc.velocity.Y < -3f)
					{
						npc.velocity.Y = -3f;
					}
					Vector2 vec6 = npc.Center + npc.velocity.SafeNormalize(Vector2.Zero) * npc.Size.Length() / 2f + npc.velocity;
					pt = vec6.ToTileCoordinates();
					tileSafely = Framing.GetTileSafely(pt);
					bool flag124 = tileSafely.nactive() && (TileID.Sets.Conversion.Sand[(int)tileSafely.type] || TileID.Sets.Conversion.Sandstone[(int)tileSafely.type] || TileID.Sets.Conversion.HardenedSand[(int)tileSafely.type]);
					if (!flag124 && npc.wet)
					{
						flag124 = (tileSafely.liquid > 0);
					}
					if (!flag124 && Math.Sign(npc.velocity.X) == npc.direction && npc.Distance(vector224) < 400f && (npc.ai[2] >= 30f || npc.ai[2] < 0f))
					{
						if (npc.localAI[0] == 0f)
						{
							Main.PlaySound(14, npc.Center, 542);
							npc.localAI[0] = -1f;
						}
						npc.ai[2] = -30f;
						Vector2 vector225 = npc.DirectionTo(vector224 + new Vector2(0f, -80f));
						npc.velocity = vector225 * 12f;
					}
				}
				else
				{
					if (npc.collideX)
					{
						npc.velocity.X = npc.velocity.X * -1f;
						npc.direction *= -1;
						npc.netUpdate = true;
					}
					if (npc.collideY)
					{
						npc.netUpdate = true;
						npc.velocity.Y = npc.velocity.Y * -1f;
						npc.directionY = Math.Sign(npc.velocity.Y);
						npc.ai[0] = (float)npc.directionY;
					}
					float num1486 = 6f;
					npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
					if (npc.velocity.X < -num1486 || npc.velocity.X > num1486)
					{
						npc.velocity.X = npc.velocity.X * 0.95f;
					}
					if (flag123)
					{
						npc.ai[0] = -1f;
					}
					else
					{
						npc.ai[0] = 1f;
					}
					float num1487 = 0.06f;
					float num1488 = 0.01f;
					if (npc.ai[0] == -1f)
					{
						npc.velocity.Y = npc.velocity.Y - num1488;
						if (npc.velocity.Y < -num1487)
						{
							npc.ai[0] = 1f;
						}
					}
					else
					{
						npc.velocity.Y = npc.velocity.Y + num1488;
						if (npc.velocity.Y > num1487)
						{
							npc.ai[0] = -1f;
						}
					}
					if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
					{
						npc.velocity.Y = npc.velocity.Y * 0.95f;
					}
				}
			}
			else
			{
				if (npc.velocity.Y == 0f)
				{
					if (flag122)
					{
						npc.TargetClosest(true);
					}
					float num1489 = 1f;
					npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
					if (npc.velocity.X < -num1489 || npc.velocity.X > num1489)
					{
						npc.velocity.X = npc.velocity.X * 0.95f;
					}
				}
				npc.velocity.Y = npc.velocity.Y + 0.3f;
				if (npc.velocity.Y > 10f)
				{
					npc.velocity.Y = 10f;
				}
				npc.ai[0] = 1f;
			}
			npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
			if (npc.rotation < -0.2f)
			{
				npc.rotation = -0.2f;
			}
			if (npc.rotation > 0.2f)
			{
				npc.rotation = 0.2f;
				return;
			}
		}
		
        public override void FindFrame(int frameHeight)
        {
            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 134, frameHeight, mod.DustType("AstralOrange"), new Rectangle(46, 4, 60, 6), Vector2.Zero, 0.55f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : mod.DustType("AstralEnemy"), 1f, 4, 25);

            //if dead do gores
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                    Gore.NewGore(npc.position + new Vector2(Main.rand.NextFloat(0f, npc.width), Main.rand.NextFloat(0f, npc.height)), npc.velocity * rand, mod.GetGoreSlot("Gores/FusionFeeder/FusionFeederGore" + i));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 offset = new Vector2(0f, 10f);
            Vector2 origin = new Vector2(67f, 23f);

            //draw shark
            spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition + offset, npc.frame, drawColor, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + offset, npc.frame, Color.White * 0.6f, npc.rotation, origin, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && spawnInfo.player.ZoneDesert)
            {
                return 0.14f;
            }
            return 0f;
        }
    }
}
