using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneMonster : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneMonsterSpawn");
        public static readonly SoundStyle DroneSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneMonsterDrone");

        private float speedAdd = 0f;
        private float speedLimit = 0f;

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 320;
            Projectile.height = 320;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 36000;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedAdd);
            writer.Write(Projectile.localAI[0]);
            writer.Write(speedLimit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedAdd = reader.ReadSingle();
            Projectile.localAI[0] = reader.ReadSingle();
            speedLimit = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
                return;
            }

            int choice = (int)Projectile.ai[1];
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.soundDelay = 1125 - (choice * 225);
                SoundEngine.PlaySound(SpawnSound, Projectile.Center);
                Projectile.localAI[0] += 1f;
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

            if (Projectile.soundDelay <= 0 && (choice == 0 || choice == 2))
            {
                Projectile.soundDelay = 420;
                SoundEngine.PlaySound(DroneSound, Projectile.Center);
            }

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            Lighting.AddLight(Projectile.Center, 3f * Projectile.Opacity, 0f, 0f);

            float inertia = (revenge ? 4.5f : 5f) + speedAdd;
            float speed = (revenge ? 1.5f : 1.35f) + (speedAdd * 0.25f);
            float minDist = 160f;

            if (NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
            {
                inertia *= 1.5f;
                speed *= 0.5f;
            }

            if (Projectile.timeLeft < 90)
                Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 90f, 0f, 1f);
            else
                Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - 35910) / 90f), 0f, 1f);

            int target = (int)Projectile.ai[0];
            if (target >= 0 && Main.player[target].active && !Main.player[target].dead)
            {
                if (Projectile.Distance(Main.player[target].Center) > minDist)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[target].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * speed) / inertia;
                }
            }
            else
            {
                Projectile.ai[0] = Player.FindClosest(Projectile.Center, 1, 1);
                Projectile.netUpdate = true;
            }

            if (death)
                return;

            // Fly away from other brimstone monsters.
            float pushForce = 0.05f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible.
                if (!otherProj.active || k == Projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == Projectile.type;
                float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                float distancegate = Main.zenithWorld ? 360f : 320f;
                if (sameProjType && taxicabDist < distancegate)
                {
                    if (Projectile.position.X < otherProj.position.X)
                        Projectile.velocity.X -= pushForce;
                    else
                        Projectile.velocity.X += pushForce;

                    if (Projectile.position.Y < otherProj.position.Y)
                        Projectile.velocity.Y -= pushForce;
                    else
                        Projectile.velocity.Y += pushForce;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 170f, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            lightColor.R = (byte)(255 * Projectile.Opacity);

            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                    {
                        tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneMonsterII").Value;
                        lightColor.B = (byte)(255 * Projectile.Opacity);
                    }
                }
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300, true);

            // Remove all positive buffs from the player if they're hit by HAGE while Cirrus is alive.
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                    {
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            int buffType = target.buffType[l];
                            if (target.buffTime[l] > 0 && CalamityLists.amalgamBuffList.Contains(buffType))
                            {
                                target.DelBuff(l);
                                l--;
                            }
                        }
                    }
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            behindNPCs.Add(index);
        }
    }
}
