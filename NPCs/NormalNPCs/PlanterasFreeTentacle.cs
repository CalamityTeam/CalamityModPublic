using System;
using System.IO;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PlanterasFreeTentacle : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.PlanterasTentacle}";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.DR_NERD(0.1f);
            NPC.width = 24;
            NPC.height = 24;
            NPC.defense = 20;
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter >= 6.0)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override void AI()
        {
            NPC.TargetClosest();

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.2f, 0.4f, 0.1f);

            // Spore dust
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 44, 0f, 0f, 250, default, 0.4f);
                dust.fadeIn = 0.7f;
            }

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (Main.getGoodWorld)
            {
                if (Main.rand.NextBool(5))
                    NPC.reflectsProjectiles = true;
                else
                    NPC.reflectsProjectiles = false;
            }

            // Die if Plantera is gone
            if (NPC.plantBoss < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }

            // Velocity and acceleration
            Vector2 idealVelocity = new Vector2(death ? 12f : 9f, death ? 5f : 3.5f);
            float accelerationX = death ? 0.2f : 0.15f;
            float accelerationY = death ? 0.15f : 0.12f;

            if (Main.getGoodWorld)
            {
                idealVelocity *= 1.2f;
                accelerationX *= 1.4f;
                accelerationY *= 1.4f;
            }

            if (NPC.direction == -1 && NPC.velocity.X > -idealVelocity.X)
            {
                NPC.velocity.X -= accelerationX;
                if (NPC.velocity.X > idealVelocity.X)
                    NPC.velocity.X -= accelerationX;
                else if (NPC.velocity.X > 0f)
                    NPC.velocity.X -= accelerationX * 2f;

                if (NPC.velocity.X < -idealVelocity.X)
                    NPC.velocity.X = -idealVelocity.X;
            }
            else if (NPC.direction == 1 && NPC.velocity.X < idealVelocity.X)
            {
                NPC.velocity.X += accelerationX;
                if (NPC.velocity.X < -idealVelocity.X)
                    NPC.velocity.X += accelerationX;
                else if (NPC.velocity.X < 0f)
                    NPC.velocity.X += accelerationX * 2f;

                if (NPC.velocity.X > idealVelocity.X)
                    NPC.velocity.X = idealVelocity.X;
            }

            if (NPC.directionY == -1 && (double)NPC.velocity.Y > -idealVelocity.Y)
            {
                NPC.velocity.Y -= accelerationY - 0.02f;
                if ((double)NPC.velocity.Y > idealVelocity.Y)
                    NPC.velocity.Y -= accelerationY;
                else if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y -= accelerationY * 2.5f;

                if ((double)NPC.velocity.Y < -idealVelocity.Y)
                    NPC.velocity.Y = -idealVelocity.Y;
            }
            else if (NPC.directionY == 1 && (double)NPC.velocity.Y < idealVelocity.Y * 0.6f)
            {
                NPC.velocity.Y += accelerationY - 0.02f;
                if ((double)NPC.velocity.Y < -idealVelocity.Y)
                    NPC.velocity.Y += accelerationY;
                else if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y += accelerationY * 2.5f;

                if ((double)NPC.velocity.Y > idealVelocity.Y)
                    NPC.velocity.Y = idealVelocity.Y;
            }

            if (NPC.wet)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.95f;

                NPC.velocity.Y -= 0.5f;
                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;

                NPC.TargetClosest();
            }

            float pushVelocity = 0.5f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                    {
                        if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 40f * NPC.scale)
                        {
                            if (NPC.position.X < Main.npc[i].position.X)
                                NPC.velocity.X -= pushVelocity;
                            else
                                NPC.velocity.X += pushVelocity;

                            if (NPC.position.Y < Main.npc[i].position.Y)
                                NPC.velocity.Y -= pushVelocity;
                            else
                                NPC.velocity.Y += pushVelocity;
                        }
                    }
                }
            }

            if (NPC.velocity.X > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
            }
            if (NPC.velocity.X < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.Pi;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; (double)i < hit.Damage / (double)NPC.lifeMax * 100.0; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 167, hit.HitDirection, -1f);

                return;
            }

            for (int i = 0; i < 150; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 167, 2 * hit.HitDirection, -2f);

            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height)), NPC.velocity, 388, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height)), NPC.velocity, 389, NPC.scale);
            }
        }
    }
}
