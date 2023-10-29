using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerHead2 : ModNPC
    {
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
            NPC.lifeMax = 10000;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.HitSound = RavagerBody.HitSound;
            NPC.DeathSound = RavagerBody.LimbLossSound;
            if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
            {
                NPC.defense *= 2;
                NPC.lifeMax *= 4;
            }
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 22500;
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

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            Player player = Main.player[Main.npc[CalamityGlobalNPC.scavenger].target];

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool provy = DownedBossSystem.downedProvidence && !bossRush;

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Rotation
            float playerXDist = NPC.position.X + (NPC.width / 2) - player.position.X - (player.width / 2);
            float playerYDist = NPC.position.Y + NPC.height - 59f - player.position.Y - (player.height / 2);
            float headRotation = (float)Math.Atan2(playerYDist, playerXDist) + MathHelper.PiOver2;
            if (headRotation < 0f)
                headRotation += MathHelper.TwoPi;
            else if (headRotation > MathHelper.TwoPi)
                headRotation -= MathHelper.TwoPi;

            float headRotateIncrement = 0.1f;
            if (NPC.rotation < headRotation)
            {
                if ((headRotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= headRotateIncrement;
                else
                    NPC.rotation += headRotateIncrement;
            }
            else if (NPC.rotation > headRotation)
            {
                if ((NPC.rotation - headRotation) > MathHelper.Pi)
                    NPC.rotation += headRotateIncrement;
                else
                    NPC.rotation -= headRotateIncrement;
            }

            if (NPC.rotation > headRotation - headRotateIncrement && NPC.rotation < headRotation + headRotateIncrement)
                NPC.rotation = headRotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > headRotation - headRotateIncrement && NPC.rotation < headRotation + headRotateIncrement)
                NPC.rotation = headRotation;

            NPC.ai[1] += 1f;
            bool fireProjectiles = NPC.ai[1] >= (bossRush ? 240f : 480f);
            if (fireProjectiles && Vector2.Distance(NPC.Center, player.Center) > 80f)
            {
                int type = ModContent.ProjectileType<ScavengerLaser>();
                int damage = NPC.GetProjectileDamage(type);
                float projectileVelocity = death ? 8f : 6f;

                if (NPC.ai[1] >= 600f)
                {
                    NPC.ai[0] += 1f;
                    NPC.ai[1] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(RavagerHead.MissileSound, NPC.Center);
                        type = ModContent.ProjectileType<ScavengerNuke>();
                        damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity * 0.25f, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, Main.npc[CalamityGlobalNPC.scavenger].target, 0f);
                    }
                }
                else
                {
                    if (NPC.ai[1] % 40f == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, -1f);
                        }
                    }
                }
            }

            float attackMovementVel = 22f;
            float attackMovementAccel = 0.3f;
            if (death)
            {
                attackMovementVel += 4f;
                attackMovementAccel += 0.05f;
            }
            if (provy)
            {
                attackMovementVel *= 1.25f;
                attackMovementAccel *= 1.25f;
            }

            Vector2 npcCenter = NPC.Center;
            float distanceX = NPC.ai[0] % 2f == 0f ? 480f : -480f;
            float distanceY = fireProjectiles ? -320f : 320f;
            float playerXDistAttack = player.Center.X + (fireProjectiles ? distanceX : 0f) - npcCenter.X;
            float playerYDistAttack = player.Center.Y + distanceY - npcCenter.Y;
            float playerDistanceAttack = (float)Math.Sqrt(playerXDistAttack * playerXDistAttack + playerYDistAttack * playerYDistAttack);
            playerDistanceAttack = attackMovementVel / playerDistanceAttack;
            playerXDistAttack *= playerDistanceAttack;
            playerYDistAttack *= playerDistanceAttack;

            if (NPC.velocity.X < playerXDistAttack)
            {
                NPC.velocity.X += attackMovementAccel;
                if (NPC.velocity.X < 0f && playerXDistAttack > 0f)
                    NPC.velocity.X += attackMovementAccel;
            }
            else if (NPC.velocity.X > playerXDistAttack)
            {
                NPC.velocity.X -= attackMovementAccel;
                if (NPC.velocity.X > 0f && playerXDistAttack < 0f)
                    NPC.velocity.X -= attackMovementAccel;
            }
            if (NPC.velocity.Y < playerYDistAttack)
            {
                NPC.velocity.Y += attackMovementAccel;
                if (NPC.velocity.Y < 0f && playerYDistAttack > 0f)
                    NPC.velocity.Y += attackMovementAccel;
            }
            else if (NPC.velocity.Y > playerYDistAttack)
            {
                NPC.velocity.Y -= attackMovementAccel;
                if (NPC.velocity.Y > 0f && playerYDistAttack < 0f)
                    NPC.velocity.Y -= attackMovementAccel;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerHead").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerHead2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScavengerHead3").Type, 1f);
                }
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool CheckActive() => false;
    }
}
