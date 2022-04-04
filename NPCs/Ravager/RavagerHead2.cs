using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Ravager
{
    public class RavagerHead2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 50;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 40;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 9591;
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath14;
            if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
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
                NPC.life = 0;
                HitEffect(NPC.direction, 9999);
                NPC.netUpdate = true;
                return;
            }

            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            Player player = Main.player[Main.npc[CalamityGlobalNPC.scavenger].target];

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Rotation
            float num801 = NPC.position.X + (NPC.width / 2) - player.position.X - (player.width / 2);
            float num802 = NPC.position.Y + NPC.height - 59f - player.position.Y - (player.height / 2);
            float num803 = (float)Math.Atan2(num802, num801) + MathHelper.PiOver2;
            if (num803 < 0f)
                num803 += MathHelper.TwoPi;
            else if (num803 > MathHelper.TwoPi)
                num803 -= MathHelper.TwoPi;

            float num804 = 0.1f;
            if (NPC.rotation < num803)
            {
                if ((num803 - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= num804;
                else
                    NPC.rotation += num804;
            }
            else if (NPC.rotation > num803)
            {
                if ((NPC.rotation - num803) > MathHelper.Pi)
                    NPC.rotation += num804;
                else
                    NPC.rotation -= num804;
            }

            if (NPC.rotation > num803 - num804 && NPC.rotation < num803 + num804)
                NPC.rotation = num803;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > num803 - num804 && NPC.rotation < num803 + num804)
                NPC.rotation = num803;

            NPC.ai[1] += 1f;
            bool fireProjectiles = NPC.ai[1] >= (malice ? 240f : 480f);
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
                        SoundEngine.PlaySound(SoundID.Item62, NPC.position);
                        type = ModContent.ProjectileType<ScavengerNuke>();
                        damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity * 0.25f, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, Main.npc[CalamityGlobalNPC.scavenger].target, 0f);
                    }
                }
                else
                {
                    if (NPC.ai[1] % 40f == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SoundEngine.PlaySound(SoundID.Item33, NPC.position);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(player.Center - NPC.Center) * projectileVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, -1f);
                        }
                    }
                }
            }

            float num823 = 22f;
            float num824 = 0.3f;
            if (death)
            {
                num823 += 4f;
                num824 += 0.05f;
            }
            if (provy)
            {
                num823 *= 1.25f;
                num824 *= 1.25f;
            }

            Vector2 vector82 = NPC.Center;
            float distanceX = NPC.ai[0] % 2f == 0f ? 480f : -480f;
            float distanceY = fireProjectiles ? -320f : 320f;
            float num825 = player.Center.X + (fireProjectiles ? distanceX : 0f) - vector82.X;
            float num826 = player.Center.Y + distanceY - vector82.Y;
            float num827 = (float)Math.Sqrt(num825 * num825 + num826 * num826);
            num827 = num823 / num827;
            num825 *= num827;
            num826 *= num827;

            if (NPC.velocity.X < num825)
            {
                NPC.velocity.X += num824;
                if (NPC.velocity.X < 0f && num825 > 0f)
                    NPC.velocity.X += num824;
            }
            else if (NPC.velocity.X > num825)
            {
                NPC.velocity.X -= num824;
                if (NPC.velocity.X > 0f && num825 < 0f)
                    NPC.velocity.X -= num824;
            }
            if (NPC.velocity.Y < num826)
            {
                NPC.velocity.Y += num824;
                if (NPC.velocity.Y < 0f && num826 > 0f)
                    NPC.velocity.Y += num824;
            }
            else if (NPC.velocity.Y > num826)
            {
                NPC.velocity.Y -= num824;
                if (NPC.velocity.Y > 0f && num826 < 0f)
                    NPC.velocity.Y -= num824;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/ScavengerGores/ScavengerHead3"), 1f);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool CheckActive() => false;

        public override bool PreNPCLoot() => false;
    }
}
