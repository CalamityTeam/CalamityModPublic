using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 50;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 40;
            npc.DR_NERD(0.15f);
            npc.lifeMax = 19182;
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.netAlways = true;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = null;
            if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
            {
                npc.defense *= 2;
                npc.lifeMax *= 4;
            }
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 45000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            npc.damage = 0;

            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            npc.Center = Main.npc[CalamityGlobalNPC.scavenger].Center + new Vector2(1f, -20f);

            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            npc.ai[1] += 1f;
            if (npc.ai[1] >= (death ? 420f : 480f))
            {
                Main.PlaySound(SoundID.Item62, npc.position);

                // Get a target
                if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                    npc.TargetClosest();

                // Despawn safety, make sure to target another player if the current player target is too far away
                if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                    npc.TargetClosest();

                npc.ai[1] = 0f;
                int type = ModContent.ProjectileType<ScavengerNuke>();
                int damage = npc.GetProjectileDamage(type);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 shootFromVector = new Vector2(npc.Center.X, npc.Center.Y - 20f);
                    Vector2 velocity = new Vector2(0f, -15f);
                    int nuke = Projectile.NewProjectile(shootFromVector, velocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, npc.target, 0f);
                    Main.projectile[nuke].velocity.Y = -15f;
                }
            }
        }

        public override bool CheckActive() => false;

        public override bool PreNPCLoot() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                int num285 = 0;
                while ((double)num285 < damage / (double)npc.lifeMax * 100.0)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, (float)hitDirection, -1f, 0, default, 1f);
                    num285++;
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<RavagerHead2>(), npc.whoAmI);
            }
        }
    }
}
