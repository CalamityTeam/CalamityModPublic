using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class KingSlimeJewel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown Jewel");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 22;
            npc.height = 22;
			npc.defense = 10;
			npc.DR_NERD(0.1f);
			npc.lifeMax = 280;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit5;
			npc.DeathSound = SoundID.NPCDeath15;
			npc.Calamity().VulnerableToSickness = false;
		}

        public override void AI()
        {
            // Red light
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 1f, 0f, 0f);

            // Despawn
            if (!NPC.AnyNPCs(NPCID.KingSlime))
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            // Float around the player
            npc.rotation = npc.velocity.X / 15f;

            npc.TargetClosest(true);

            float velocity = 2f;
            float acceleration = 0.1f;

            if (npc.position.Y > Main.player[npc.target].position.Y - 350f)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.98f;

                npc.velocity.Y -= acceleration;

                if (npc.velocity.Y > velocity)
                    npc.velocity.Y = velocity;
            }
            else if (npc.position.Y < Main.player[npc.target].position.Y - 400f)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= 0.98f;

                npc.velocity.Y += acceleration;

                if (npc.velocity.Y < -velocity)
                    npc.velocity.Y = -velocity;
            }

            if (npc.Center.X > Main.player[npc.target].Center.X + 100f)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X *= 0.98f;

                npc.velocity.X -= acceleration;

                if (npc.velocity.X > 8f)
                    npc.velocity.X = 8f;
            }
            if (npc.Center.X < Main.player[npc.target].Center.X - 100f)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X *= 0.98f;

                npc.velocity.X += acceleration;

                if (npc.velocity.X < -8f)
                    npc.velocity.X = -8f;
            }

            // Fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Fire bolt every 1.5 seconds
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 45f : CalamityWorld.death ? 60f : 75f))
                {
                    npc.localAI[0] = 0f;

                    Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
                    float xDist = Main.player[npc.target].Center.X - npcPos.X;
                    float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
                    Vector2 projVector = new Vector2(xDist, yDist);
					float projLength = projVector.Length();

                    float speed = 9f;
					int type = ModContent.ProjectileType<JewelProjectile>();

                    projLength = speed / projLength;
                    projVector.X *= projLength;
                    projVector.Y *= projLength;
                    npcPos.X += projVector.X * 2f;
                    npcPos.Y += projVector.Y * 2f;

                    for (int dusty = 0; dusty < 10; dusty++)
                    {
                        Vector2 dustVel = projVector;
                        dustVel.Normalize();
                        int ruby = Dust.NewDust(npc.Center, npc.width, npc.height, 90, dustVel.X, dustVel.Y, 100, default, 2f);
                        Main.dust[ruby].velocity *= 1.5f;
                        Main.dust[ruby].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[ruby].scale = 0.5f;
                            Main.dust[ruby].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    Main.PlaySound(SoundID.Item8, npc.position);
					int damage = npc.GetProjectileDamage(type);
					if (CalamityWorld.death || BossRushEvent.BossRushActive)
					{
						int numProj = 2;
						float rotation = MathHelper.ToRadians(9);
						for (int i = 0; i < numProj + 1; i++)
						{
							Vector2 perturbedSpeed = projVector.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
							Projectile.NewProjectile(npcPos, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
					else
						Projectile.NewProjectile(npcPos, projVector, type, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 50, 50, 0);
        }

		public override bool CheckActive()
		{
			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(npc.position, npc.width, npc.height, 90, hitDirection, -1f, 0, default, 1f);
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (npc.width / 2);
				npc.position.Y = npc.position.Y + (npc.height / 2);
				npc.width = 45;
				npc.height = 45;
				npc.position.X = npc.position.X - (npc.width / 2);
				npc.position.Y = npc.position.Y - (npc.height / 2);
				for (int num621 = 0; num621 < 2; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 90, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 90, 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 90, 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}
