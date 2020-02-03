using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
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
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
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

            float velocity = CalamityWorld.bossRushActive ? 8f : 2f;
            float acceleration = CalamityWorld.bossRushActive ? 0.4f : 0.1f;

            if (npc.position.Y > Main.player[npc.target].position.Y - 350f)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y = npc.velocity.Y * 0.98f;

                npc.velocity.Y = npc.velocity.Y - acceleration;

                if (npc.velocity.Y > velocity)
                    npc.velocity.Y = velocity;
            }
            else if (npc.position.Y < Main.player[npc.target].position.Y - 400f)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y = npc.velocity.Y * 0.98f;

                npc.velocity.Y = npc.velocity.Y + acceleration;

                if (npc.velocity.Y < -velocity)
                    npc.velocity.Y = -velocity;
            }

            if (npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + 100f)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X = npc.velocity.X * 0.98f;

                npc.velocity.X = npc.velocity.X - acceleration;

                if (npc.velocity.X > 8f)
                    npc.velocity.X = 8f;
            }
            if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - 100f)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X = npc.velocity.X * 0.98f;

                npc.velocity.X = npc.velocity.X + acceleration;

                if (npc.velocity.X < -8f)
                    npc.velocity.X = -8f;
            }

            // Fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Fire bolt every 1.5 seconds
                npc.localAI[0] += CalamityWorld.bossRushActive ? 2f : 1f;
                if (npc.localAI[0] >= (CalamityWorld.death ? 60f : 75f))
                {
                    npc.localAI[0] = 0f;

                    Vector2 vector62 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num506 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector62.X;
                    float num507 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector62.Y;
                    float num508 = (float)Math.Sqrt((double)(num506 * num506 + num507 * num507));

                    float num509 = CalamityWorld.bossRushActive ? 18f : 9f;
                    int num510 = 11;
					if (CalamityWorld.death)
						num510 += 1;
					int num511 = ProjectileID.RubyBolt;

                    num508 = num509 / num508;
                    num506 *= num508;
                    num507 *= num508;
                    vector62.X += num506 * 2f;
                    vector62.Y += num507 * 2f;

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        Vector2 velocity2 = new Vector2(num506, num507);
                        velocity2.Normalize();
                        int num622 = Dust.NewDust(npc.Center, npc.width, npc.height, 90, velocity2.X, velocity2.Y, 100, default, 2f);
                        Main.dust[num622].velocity *= 1.5f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 8);

                    int proj = Projectile.NewProjectile(vector62.X, vector62.Y, num506, num507, num511, num510, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[proj].Calamity().forceHostile = true;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 50, 50, 0);
        }
    }
}
