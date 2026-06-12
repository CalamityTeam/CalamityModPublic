using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    [HeavyKnockbackWhitelisted]
    public class PlanterasFreeTentacle : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.PlanterasTentacle}";

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.damage = 60;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 24;
            NPC.height = 24;
            NPC.defense = 20;
            NPC.lifeMax = 500;
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
            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                NPC.frame.Y = 0;
        }

        public override void AI()
        {
            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, default);

            // Emit light
            Lighting.AddLight(NPC.Center, 0.2f, 0.4f, 0.1f);

            // Spore dust
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.JungleSpore, 0f, 0f, 250, default, 0.4f);
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

            // If vomited out during Master Mode, maintain velocity until a certain time has passed
            if (NPC.ai[0] > 0f)
            {
                NPC.knockBackResist = 0f;

                NPC.ai[0] -= 1f;

                if (NPC.velocity.Length() < NPC.ai[1])
                {
                    NPC.velocity *= 1.01f;
                    if (NPC.velocity.Length() > NPC.ai[1])
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= NPC.ai[1];
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

                return;
            }

            NPC.knockBackResist = 0.4f;

            // Velocity and acceleration
            Vector2 idealVelocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * (death ? 7f : 5.5f);
            float acceleration = death ? 0.14f : 0.105f;

            if (Main.getGoodWorld)
            {
                idealVelocity *= 1.2f;
                acceleration *= 1.4f;
            }

            NPC.SimpleFlyMovement(idealVelocity, acceleration);

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
            foreach (var n in Main.ActiveNPCs)
            {
                if (n.whoAmI != NPC.whoAmI && n.type == NPC.type)
                {
                    if (Vector2.Distance(NPC.Center, n.Center) < 40f * NPC.scale)
                    {
                        if (NPC.position.X < n.position.X)
                            NPC.velocity.X -= pushVelocity;
                        else
                            NPC.velocity.X += pushVelocity;

                        if (NPC.position.Y < n.position.Y)
                            NPC.velocity.Y -= pushVelocity;
                        else
                            NPC.velocity.Y += pushVelocity;
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
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
            NPC.damage = (int)(NPC.damage * 1.15f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; (double)i < hit.Damage / (double)NPC.lifeMax * 100.0; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Plantera_Green, hit.HitDirection, -1f);

                return;
            }

            for (int i = 0; i < 150; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Plantera_Green, 2 * hit.HitDirection, -2f);

            if (!Main.dedServ)
            {
                Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height)), NPC.velocity, 388, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height)), NPC.velocity, 389, NPC.scale);
            }
        }
    }
}
