using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
	public class Cuttlefish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cuttlefish");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.chaseable = false;
            npc.damage = 34;
            npc.width = 50;
            npc.height = 28;
            npc.defense = 8;
            npc.lifeMax = 110;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.HitSound = SoundID.NPCHit33;
            npc.DeathSound = SoundID.NPCDeath28;
            npc.knockBackResist = 0.3f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CuttlefishBanner>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = true;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
		}

        public override void AI()
        {
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            int num = 200;
            if (npc.ai[2] == 0f)
            {
                npc.alpha = num;
                npc.TargetClosest(true);
                if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[2] = -16f;
                }
                if (npc.justHit)
                {
                    npc.ai[2] = -16f;
                }
				if (npc.collideX)
				{
					npc.velocity.X = npc.velocity.X * -1f;
					npc.direction *= -1;
				}
				if (npc.collideY)
				{
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
						npc.directionY = -1;
						npc.ai[0] = -1f;
					}
					else if (npc.velocity.Y < 0f)
					{
						npc.velocity.Y = Math.Abs(npc.velocity.Y);
						npc.directionY = 1;
						npc.ai[0] = 1f;
					}
				}
				npc.velocity.X = npc.velocity.X + npc.direction * 0.02f;
				npc.rotation = npc.velocity.X * 0.4f;
				if (npc.velocity.X < -1f || npc.velocity.X > 1f)
				{
					npc.velocity.X = npc.velocity.X * 0.95f;
				}
				if (npc.ai[0] == -1f)
				{
					npc.velocity.Y = npc.velocity.Y - 0.01f;
					if (npc.velocity.Y < -1f)
					{
						npc.ai[0] = 1f;
					}
				}
				else
				{
					npc.velocity.Y = npc.velocity.Y + 0.01f;
					if (npc.velocity.Y > 1f)
					{
						npc.ai[0] = -1f;
					}
				}
				int num268 = (int)(npc.position.X + (npc.width / 2)) / 16;
				int num269 = (int)(npc.position.Y + (npc.height / 2)) / 16;
				if (Main.tile[num268, num269 - 1] == null)
				{
					Main.tile[num268, num269 - 1] = new Tile();
				}
				if (Main.tile[num268, num269 + 1] == null)
				{
					Main.tile[num268, num269 + 1] = new Tile();
				}
				if (Main.tile[num268, num269 + 2] == null)
				{
					Main.tile[num268, num269 + 2] = new Tile();
				}
				if (Main.tile[num268, num269 - 1].liquid > 128)
				{
					if (Main.tile[num268, num269 + 1].active())
					{
						npc.ai[0] = -1f;
					}
					else if (Main.tile[num268, num269 + 2].active())
					{
						npc.ai[0] = -1f;
					}
				}
				else
				{
					npc.ai[0] = 1f;
				}
				if (npc.velocity.Y > 1.2 || npc.velocity.Y < -1.2)
				{
					npc.velocity.Y = npc.velocity.Y * 0.99f;
				}
				return;
            }
            if (npc.ai[2] < 0f)
            {
                if (npc.alpha > 0)
                {
                    npc.alpha -= num / 16;
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
					npc.ai[0] = 0f;
					npc.ai[2] = 1f;
                    npc.velocity.X = npc.direction * 2;
                }
                return;
            }
            if (npc.ai[2] == 1f)
            {
                npc.chaseable = true;
                if (npc.direction == 0)
                {
                    npc.TargetClosest(true);
                }
                if (npc.wet || npc.noTileCollide)
                {
                    bool flag14 = false;
                    npc.TargetClosest(false);
                    if (Main.player[npc.target].wet && !Main.player[npc.target].dead)
                    {
                        flag14 = true;
                    }
                    if (!flag14)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.noTileCollide = false;
                        }
                        if (npc.collideX)
                        {
                            npc.velocity.X = npc.velocity.X * -1f;
                            npc.direction *= -1;
                            npc.netUpdate = true;
                        }
                        if (npc.collideY)
                        {
                            npc.netUpdate = true;
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                                npc.directionY = -1;
                                npc.ai[0] = -1f;
                            }
                            else if (npc.velocity.Y < 0f)
                            {
                                npc.velocity.Y = Math.Abs(npc.velocity.Y);
                                npc.directionY = 1;
                                npc.ai[0] = 1f;
                            }
                        }
                    }
                    if (flag14)
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            if (npc.ai[3] > 0f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                            {
                                npc.ai[3] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.ai[3] == 0f)
                        {
                            npc.ai[1] += 1f;
                        }
                        if (npc.ai[1] >= 150f)
                        {
                            npc.ai[3] = 1f;
                            npc.ai[1] = 0f;
                            npc.netUpdate = true;
                        }
                        if (npc.ai[3] == 0f)
                        {
                            npc.alpha = 0;
                            npc.noTileCollide = false;
                        }
                        else
                        {
                            npc.alpha = 200;
                            npc.noTileCollide = true;
                        }
                        npc.TargetClosest(true);
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.2f;
                        npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.2f;
                        if (npc.velocity.X > 9f)
                        {
                            npc.velocity.X = 9f;
                        }
                        if (npc.velocity.X < -9f)
                        {
                            npc.velocity.X = -9f;
                        }
                        if (npc.velocity.Y > 7f)
                        {
                            npc.velocity.Y = 7f;
                        }
                        if (npc.velocity.Y < -7f)
                        {
                            npc.velocity.Y = -7f;
                        }
                    }
                    else
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.noTileCollide = false;
                        }
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                        if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                        {
                            npc.velocity.X = npc.velocity.X * 0.95f;
                        }
                        if (npc.ai[0] == -1f)
                        {
                            npc.velocity.Y = npc.velocity.Y - 0.01f;
                            if ((double)npc.velocity.Y < -0.3)
                            {
                                npc.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y + 0.01f;
                            if ((double)npc.velocity.Y > 0.3)
                            {
                                npc.ai[0] = -1f;
                            }
                        }
                    }
                    int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                    int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                    if (Main.tile[num258, num259 - 1] == null)
                    {
                        Main.tile[num258, num259 - 1] = new Tile();
                    }
                    if (Main.tile[num258, num259 + 1] == null)
                    {
                        Main.tile[num258, num259 + 1] = new Tile();
                    }
                    if (Main.tile[num258, num259 + 2] == null)
                    {
                        Main.tile[num258, num259 + 2] = new Tile();
                    }
                    if (Main.tile[num258, num259 - 1].liquid > 128)
                    {
                        if (Main.tile[num258, num259 + 1].active())
                        {
                            npc.ai[0] = -1f;
                        }
                        else if (Main.tile[num258, num259 + 2].active())
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                    if ((double)npc.velocity.Y > 0.4 || (double)npc.velocity.Y < -0.4)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.95f;
                    }
                }
                else
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.94f;
                        if ((double)npc.velocity.X > -0.2 && (double)npc.velocity.X < 0.2)
                        {
                            npc.velocity.X = 0f;
                        }
                    }
                    npc.velocity.Y = npc.velocity.Y + 0.25f;
                    if (npc.velocity.Y > 7f)
                    {
                        npc.velocity.Y = 7f;
                    }
                    npc.ai[0] = 1f;
                }
                npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
                if ((double)npc.rotation < -0.2)
                {
                    npc.rotation = -0.2f;
                }
                if ((double)npc.rotation > 0.2)
                {
                    npc.rotation = 0.2f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (!npc.wet && !npc.noTileCollide)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
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
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/CuttlefishGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/CuttlefishGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Gold);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/CuttlefishGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Darkness, 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer1 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.3f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer2 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<AnechoicCoating>(), 2);
            int inkBombDropRate = Main.expertMode ? 50 : 100;
            DropHelper.DropItemChance(npc, ModContent.ItemType<InkBomb>(), inkBombDropRate, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
