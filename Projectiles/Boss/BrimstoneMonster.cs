using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneMonster : ModProjectile
    {
        private float speedAdd = 0f;
        private float speedLimit = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Monster");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 320;
            projectile.height = 320;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 36000;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedAdd);
            writer.Write(projectile.localAI[0]);
            writer.Write(speedLimit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedAdd = reader.ReadSingle();
            projectile.localAI[0] = reader.ReadSingle();
            speedLimit = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                projectile.active = false;
                projectile.netUpdate = true;
                return;
            }

            int choice = (int)projectile.ai[1];
            if (projectile.localAI[0] == 0f)
            {
                projectile.soundDelay = 1125 - (choice * 225);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BrimstoneMonsterSpawn"), projectile.Center);
                projectile.localAI[0] += 1f;
                switch (choice)
                {
                    case 0:
                        speedLimit = 10f;
                        break;
                    case 1:
                        speedLimit = 20f;
                        break;
                    case 2:
                        speedLimit = 30f;
                        break;
                    case 3:
                        speedLimit = 40f;
                        break;
                    default:
                        break;
                }
            }

            if (speedAdd < speedLimit)
                speedAdd += 0.04f;

            if (projectile.soundDelay <= 0 && (choice == 0 || choice == 2))
            {
                projectile.soundDelay = 420;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BrimstoneMonsterDrone"), projectile.Center);
            }

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            Lighting.AddLight(projectile.Center, 3f * projectile.Opacity, 0f, 0f);

            float inertia = (revenge ? 4.5f : 5f) + speedAdd;
            float speed = (revenge ? 1.5f : 1.35f) + (speedAdd * 0.25f);
            float minDist = 160f;

            if (NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
            {
                inertia *= 1.5f;
                speed *= 0.5f;
            }

            if (projectile.timeLeft < 90)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 90f, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - 35910) / 90f), 0f, 1f);

            int target = (int)projectile.ai[0];
            if (target >= 0 && Main.player[target].active && !Main.player[target].dead)
            {
                if (projectile.Distance(Main.player[target].Center) > minDist)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(Main.player[target].Center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * speed) / inertia;
                }
            }
            else
            {
                projectile.ai[0] = Player.FindClosest(projectile.Center, 1, 1);
                projectile.netUpdate = true;
            }

            if (death)
                return;

            // Fly away from other brimstone monsters
            float pushForce = 0.05f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || k == projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == projectile.type;
                float taxicabDist = Vector2.Distance(projectile.Center, otherProj.Center);
                if (sameProjType && taxicabDist < 320f)
                {
                    if (projectile.position.X < otherProj.position.X)
                        projectile.velocity.X -= pushForce;
                    else
                        projectile.velocity.X += pushForce;

                    if (projectile.position.Y < otherProj.position.Y)
                        projectile.velocity.Y -= pushForce;
                    else
                        projectile.velocity.Y += pushForce;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 170f, targetHitbox);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            lightColor.R = (byte)(255 * projectile.Opacity);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 900);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300, true);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
            drawCacheProjsBehindNPCs.Add(index);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
