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
    public class DankCreeper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 70;
            NPC.height = 70;
            NPC.defense = 6;

            NPC.lifeMax = 90;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 2000;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            if ((CalamityWorld.LegendaryMode && CalamityWorld.revenge))
                NPC.reflectsProjectiles = true;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.3f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            bool revenge = CalamityWorld.revenge;
            float speed = revenge ? 12f : 11f;
            if (BossRushEvent.BossRushActive)
                speed = 18f;

            if (NPC.ai[1] < 90f)
                NPC.ai[1] += 1f;

            speed = MathHelper.Lerp(3f, speed, NPC.ai[1] / 90f);

            NPC.rotation = NPC.velocity.X * 0.05f;

            Vector2 targetDirection = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
            float playerXDist = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - targetDirection.X;
            float playerYDist = Main.player[NPC.target].Center.Y - targetDirection.Y;
            float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);
            float timeToReachTarget = speed / playerDistance;
            playerXDist *= timeToReachTarget;
            playerYDist *= timeToReachTarget;
            NPC.ai[0] -= 1f;
            if (playerDistance < 200f || NPC.ai[0] > 0f)
            {
                if (playerDistance < 200f)
                {
                    NPC.ai[0] = 20f;
                }
                if (NPC.velocity.X < 0f)
                {
                    NPC.direction = -1;
                }
                else
                {
                    NPC.direction = 1;
                }
                return;
            }
            NPC.velocity.X = (NPC.velocity.X * 50f + playerXDist) / 51f;
            NPC.velocity.Y = (NPC.velocity.Y * 50f + playerYDist) / 51f;
            if (playerDistance < 350f)
            {
                NPC.velocity.X = (NPC.velocity.X * 10f + playerXDist) / 11f;
                NPC.velocity.Y = (NPC.velocity.Y * 10f + playerYDist) / 11f;
            }
            if (playerDistance < 300f)
            {
                NPC.velocity.X = (NPC.velocity.X * 7f + playerXDist) / 8f;
                NPC.velocity.Y = (NPC.velocity.Y * 7f + playerYDist) / 8f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 13, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 13, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DankCreeperGore").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DankCreeperGore2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DankCreeperGore3").Type, 1f);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            if ((Main.expertMode || BossRushEvent.BossRushActive) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                int damage = NPC.GetProjectileDamage(type);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer);
            }
        }
    }
}
