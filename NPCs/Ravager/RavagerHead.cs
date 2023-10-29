using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerHead : ModNPC
    {
        public static readonly SoundStyle MissileSound = new("CalamityMod/Sounds/Custom/Ravager/RavagerMissileLaunch");
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.RavagerBody.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 50;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 40;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.HitSound = RavagerBody.HitSound;
            NPC.DeathSound = null;
            if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
            {
                NPC.defense *= 2;
                NPC.lifeMax *= 4;
            }
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 45000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }

            bool provy = DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            NPC.Center = Main.npc[CalamityGlobalNPC.scavenger].Center + new Vector2(1f, -20f);

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            NPC.ai[1] += 1f;
            if (NPC.ai[1] >= (death ? 420f : 480f))
            {
                SoundEngine.PlaySound(MissileSound, NPC.Center);

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.TargetClosest();

                // Despawn safety, make sure to target another player if the current player target is too far away
                if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                    NPC.TargetClosest();

                NPC.ai[1] = 0f;
                int type = ModContent.ProjectileType<ScavengerNuke>();
                int damage = NPC.GetProjectileDamage(type);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 shootFromVector = new Vector2(NPC.Center.X, NPC.Center.Y - 20f);
                    Vector2 velocity = new Vector2(0f, -15f);
                    int nuke = Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFromVector, velocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, NPC.target, 0f);
                    Main.projectile[nuke].velocity.Y = -15f;
                }
            }
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                int dustCounter = 0;
                while ((double)dustCounter < hit.Damage / (double)NPC.lifeMax * 100.0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, (float)hit.HitDirection, -1f, 0, default, 1f);
                    dustCounter++;
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient && !Main.zenithWorld) //GFB does something else
            {
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.position.Y + NPC.height, ModContent.NPCType<RavagerHead2>(), NPC.whoAmI);
            }
        }
    }
}
