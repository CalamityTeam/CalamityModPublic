using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
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
            npc.width = 40;
            npc.height = 30;

            npc.damage = 40;
            npc.lifeMax = 300;
            npc.defense = 5;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 245;
                npc.lifeMax = 5995;
                npc.defense = 60;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 75;
                npc.lifeMax = 640;
            }

            npc.knockBackResist = 0f;
            animationType = 81;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.alpha = 50;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<IrradiatedSlimeBanner>();
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.6f, 0.8f, 0.6f);
			if (CalamityWorld.downedAquaticScourge && !Main.player[npc.target].dead && CalamityWorld.rainingAcid)
			{
				npc.aiStyle = aiType = -1;

				npc.noTileCollide = false;
				npc.noGravity = false;
				if (Main.player[npc.target].dead)
				{
					npc.TargetClosest(true);
					if (Main.player[npc.target].dead)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}
				// Hop around
				if (npc.ai[0] == 0f)
				{
					// Lunge towards player if too far away
					if ((Main.player[npc.target].Center - npc.Center).Length() > 900f)
					{
						npc.ai[0] = 2f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					if (npc.velocity.Y == 0f)
					{
						npc.TargetClosest(true);
						npc.velocity.X = npc.velocity.X * 0.85f;
						npc.ai[1] += 1f;
						float hopRate = 15f + 30f * ((float)npc.life / (float)npc.lifeMax);
						float lungeForwardSpeed = 3f + 4f * (1f - (float)npc.life / (float)npc.lifeMax);
						float jumpSpeed = 9f;
						if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
						{
							jumpSpeed += 6f;
						}
						if (npc.ai[1] > hopRate)
						{
							npc.ai[3] += 1f;
							if (npc.ai[3] >= 3f)
							{
								npc.ai[3] = 0f;
								jumpSpeed *= 2f;
								lungeForwardSpeed /= 2f;
							}
							npc.ai[1] = 0f;
							npc.velocity.Y -= jumpSpeed;
							npc.velocity.X = lungeForwardSpeed * npc.direction;
						}
					}
					else
					{
						npc.knockBackResist = 0f;
						npc.velocity.X = npc.velocity.X * 0.99f;
						if (npc.direction < 0 && npc.velocity.X > -1f)
						{
							npc.velocity.X = -1f;
						}
						if (npc.direction > 0 && npc.velocity.X < 1f)
						{
							npc.velocity.X = 1f;
						}
					}
					npc.ai[2]++;
					if (npc.ai[2] > 210f && npc.velocity.Y == 0f && Main.netMode != 1)
					{
						if (Main.rand.NextBool(2))
						{
							npc.ai[0] = 2f;
							npc.noTileCollide = true;
							npc.velocity.Y = -8f;
						}
						else
						{
							npc.ai[0] = 3f;
						}
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						return;
					}
				}
				// Attempt to fly above the player
				else if (npc.ai[0] == 1f)
				{
					npc.noTileCollide = true;
					npc.noGravity = true;
					npc.knockBackResist = 0f;
					npc.direction = (npc.velocity.X > 0).ToDirectionInt();
					npc.TargetClosest(true);
					Vector2 idealPosition = Main.player[npc.target].Center;
					idealPosition.Y -= 350f;
					Vector2 idealDistanceVector = idealPosition - npc.Center;
					if (npc.ai[2] == 1f)
					{
						npc.ai[1] += 1f;
						npc.velocity = (npc.velocity * 4f + npc.DirectionTo(Main.player[npc.target].Center) * 10f) / 5f;
						// And slam downward
						if (npc.ai[1] > 6f)
						{
							npc.ai[1] = 0f;
							npc.ai[0] = 1.1f;
							npc.ai[2] = 0f;
							npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 10f;
							return;
						}
					}
					else
					{
						if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 40f && npc.Center.Y < Main.player[npc.target].Center.Y - 300f)
						{
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
							return;
						}
						npc.velocity = (npc.velocity * 5f + Vector2.Normalize(idealDistanceVector) * 10f) / 6f;
						return;
					}
				}
				// Slam onto the player
				else if (npc.ai[0] == 1.1f)
				{
					npc.knockBackResist = 0f;
					if (npc.ai[2] == 0f && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[2] = 1f;
					}
					if (npc.Bottom.Y >= Main.player[npc.target].position.Y || npc.velocity.Y <= 0f)
					{
						npc.ai[1] += 1f;
						if (npc.ai[1] > 10f)
						{
							npc.ai[0] = 0f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							if (Collision.SolidCollision(npc.position, npc.width, npc.height))
							{
								npc.ai[0] = 2f;
							}
						}
					}
					else
					{
						if (npc.ai[2] == 0f)
						{
							npc.noTileCollide = true;
							npc.noGravity = true;
							npc.knockBackResist = 0f;
						}
					}
					float yAcceleration = 0.135f;
					npc.velocity.Y += yAcceleration;
					if (npc.velocity.Y > 18f)
					{
						npc.velocity.Y = 18f;
						return;
					}
				}
				// Lunge towards the player
				else if (npc.ai[0] == 2f)
				{
					npc.direction = (npc.velocity.X > 0).ToDirectionInt();
					npc.noTileCollide = true;
					npc.noGravity = true;
					npc.knockBackResist = 0f;
					Vector2 idealDistanceVector = Main.player[npc.target].Center - npc.Center;
					idealDistanceVector.Y -= 4f;
					if (idealDistanceVector.Length() < 120f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						if (Main.rand.NextBool(5))
						{
							Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 8f, ModContent.ProjectileType<TrilobiteSpike>(),
								41, 4f);
						}
					}
					if (idealDistanceVector.Length() > 3f)
					{
						idealDistanceVector.Normalize();
						idealDistanceVector *= 9f;
					}
					npc.velocity = (npc.velocity * 4f + idealDistanceVector) / 5f;
					return;
				}
				// Significant, ground-based lunges
				else if (npc.ai[0] == 3f)
				{
					npc.knockBackResist = 0f;
					if (npc.velocity.Y == 0f)
					{
						npc.TargetClosest(true);
						npc.velocity.X = npc.velocity.X * 0.8f;
						npc.ai[1] += 1f;
						if (npc.ai[1] > 5f)
						{
							npc.ai[1] = 0f;
							npc.velocity.Y = npc.velocity.Y - 4f;
							if (Main.player[npc.target].Bottom.Y < npc.Center.Y)
							{
								npc.velocity.Y -= 1.25f;
							}
							if (Main.player[npc.target].Bottom.Y < npc.Center.Y - 40f)
							{
								npc.velocity.Y -= 1.5f;
							}
							if (Main.player[npc.target].Bottom.Y < npc.Center.Y - 80f)
							{
								npc.velocity.Y -= 1.75f;
							}
							if (Main.player[npc.target].Bottom.Y < npc.Center.Y - 120f)
							{
								npc.velocity.Y -= 2f;
							}
							if (Main.player[npc.target].Bottom.Y < npc.Center.Y - 160f)
							{
								npc.velocity.Y -= 2.25f;
							}
							if (Main.player[npc.target].Bottom.Y < npc.Center.Y - 200f)
							{
								npc.velocity.Y -= 2.5f;
							}
							if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
							{
								npc.velocity.Y -= 2f;
							}
							npc.velocity.X = 10f * npc.direction;
							npc.ai[2] += 1f;
						}
					}
					if (npc.ai[2] >= 3f && npc.velocity.Y == 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						return;
					}
				}
			}
			else
			{
				npc.aiStyle = 1;
				aiType = 141;
			}
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/IrradiatedSlime"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/IrradiatedSlime2"), 1f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<LeadCore>(), 100);
        }
    }
}
