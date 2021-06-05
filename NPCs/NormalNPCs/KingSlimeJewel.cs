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
            npc.lifeMax = 999;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
            npc.DeathSound = SoundID.NPCDeath39;
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

            float velocity = BossRushEvent.BossRushActive ? 8f : 2f;
            float acceleration = BossRushEvent.BossRushActive ? 0.4f : 0.1f;

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
                npc.localAI[0] += BossRushEvent.BossRushActive ? 2f : 1f;
                if (npc.localAI[0] >= (CalamityWorld.malice ? 45f : CalamityWorld.death ? 60f : 75f))
                {
                    npc.localAI[0] = 0f;

                    Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
                    float xDist = Main.player[npc.target].Center.X - npcPos.X;
                    float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
                    Vector2 projVector = new Vector2(xDist, yDist);
					float projLength = projVector.Length();

                    float speed = BossRushEvent.BossRushActive ? 18f : 9f;
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
					if (CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice)
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
    }
}
