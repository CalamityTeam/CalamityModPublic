using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class DarkHeart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.damage = 20;
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 2;

            NPC.lifeMax = 75;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 1800;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.4f;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath21;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            NPC.TargetClosest();
            float npcSpeed = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 8f : revenge ? 4.5f : 4f;
            float velocityMult = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 1f : revenge ? 0.8f : 0.75f;
            if (BossRushEvent.BossRushActive)
            {
                npcSpeed *= 2f;
                velocityMult *= 2f;
            }

            Vector2 npcCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
            float playerXDist = Main.player[NPC.target].Center.X - npcCenter.X;
            float playerYDist = Main.player[NPC.target].Center.Y - npcCenter.Y - ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 500f : 400f);
            float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);
            if (playerDistance < ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 10f : 20f))
            {
                playerXDist = NPC.velocity.X;
                playerYDist = NPC.velocity.Y;
            }
            else
            {
                playerDistance = npcSpeed / playerDistance;
                playerXDist *= playerDistance;
                playerYDist *= playerDistance;
            }
            if (NPC.velocity.X < playerXDist)
            {
                NPC.velocity.X = NPC.velocity.X + velocityMult;
                if (NPC.velocity.X < 0f && playerXDist > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + velocityMult * 2f;
                }
            }
            else if (NPC.velocity.X > playerXDist)
            {
                NPC.velocity.X = NPC.velocity.X - velocityMult;
                if (NPC.velocity.X > 0f && playerXDist < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - velocityMult * 2f;
                }
            }
            if (NPC.velocity.Y < playerYDist)
            {
                NPC.velocity.Y = NPC.velocity.Y + velocityMult;
                if (NPC.velocity.Y < 0f && playerYDist > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + velocityMult * 2f;
                }
            }
            else if (NPC.velocity.Y > playerYDist)
            {
                NPC.velocity.Y = NPC.velocity.Y - velocityMult;
                if (NPC.velocity.Y > 0f && playerYDist < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - velocityMult * 2f;
                }
            }
            if (NPC.position.X + NPC.width > Main.player[NPC.target].position.X && NPC.position.X < Main.player[NPC.target].position.X + Main.player[NPC.target].width && NPC.position.Y + NPC.height < Main.player[NPC.target].position.Y && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] >= (Main.getGoodWorld ? 12f : 24f))
                {
                    NPC.ai[0] = 0f;
                    int shaderainXPos = (int)(NPC.position.X + 10f + Main.rand.Next(NPC.width - 20));
                    int shaderainYos = (int)(NPC.position.Y + NPC.height + 4f);
                    int type = ModContent.ProjectileType<ShaderainHostile>();
                    int damage = NPC.GetProjectileDamage(type);
                    float randomXVelocity = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? Main.rand.NextFloat() * 5f : 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shaderainXPos, shaderainYos, randomXVelocity, 4f, type, damage, 0f, Main.myPlayer);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
