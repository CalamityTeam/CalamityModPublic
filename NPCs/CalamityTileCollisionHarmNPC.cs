using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityTileCollisionHarmNPC : GlobalNPC
    {
        /// <summary>
        /// How much "gravity damage" is this npc slated to recieve once it hits the ground.
        /// </summary>
        private int potentialEnergyDamage = 0;
        /// <summary>
        /// This keeps track of the last recorded velocity, so that we can dispel the potential fall damage if the enemy slows their fall down by any means
        /// </summary>
        private Vector2 OldVelocity = Vector2.Zero;
        /// <summary>
        /// This keeps track of the last recorded velocity, because terraria is dumb and we need two of them
        /// </summary>
        private Vector2 OlderVelocity = Vector2.Zero;
        private float terminalVelocityForFullFallDamage = 0;
        private bool CheckTiles = false;
        private Vector2 ForcedVel = Vector2.Zero;
        /// <summary>
        /// Set to true if you only want the npc to receive forced velocity and not receive damage.
        /// </summary>
        private bool ForceVelocityOnly = false;
        private bool hitVoid = false;
        private Player attacker;
        private int packetTimer = 0;
        private bool GrandDad = false;

        public int PotentialEnergyDamage
        {
            get
            {
                return potentialEnergyDamage;
            }

            private set
            {
                //You can't stack gravity damage. Higher "gravity" simply replaces the gravity damage.
                potentialEnergyDamage = value == 0 ? 0 : Math.Max(potentialEnergyDamage, value);
            }
        }

        public bool FallDamageSusceptible => PotentialEnergyDamage > 0 || ForceVelocityOnly;

        public override bool InstancePerEntity => true;

        public override GlobalNPC Clone(NPC npc, NPC npcClone)
        {
            CalamityTileCollisionHarmNPC myClone = (CalamityTileCollisionHarmNPC)base.Clone(npc, npcClone);

            myClone.potentialEnergyDamage = potentialEnergyDamage;
            myClone.OldVelocity = OldVelocity;
            myClone.OlderVelocity = OlderVelocity;
            myClone.terminalVelocityForFullFallDamage = terminalVelocityForFullFallDamage;

            return myClone;
        }

        public override void SetDefaults(NPC npc)
        {
            PotentialEnergyDamage = 0;
            OldVelocity = Vector2.Zero;
            OlderVelocity = Vector2.Zero;
        }

        /// <summary>
        /// Sets up a NPC to recieve forced velocity in a direction and receive fall damage when they hit the ground.
        /// </summary>
        /// <param name="npc">The npc to apply fall damage to</param>
        /// <param name="player">The player applying fall damage to the npc</param>
        /// <param name="potentialDamage">The maximum fall damage taken by the npc once they hit the ground</param>
        /// <param name="terminalVelocityForFullDamage">The downwards velocity necessary to recieve the full fall damage. By default 10, the npc max fall speed</param>
        /// <param name="checkTiles">Will check for tiles in the collision instead of velocity</param>
        public void ApplyCollisionDamage(NPC npc, Player player, int potentialDamage, Vector2 forcedVel, float terminalVelocityForFullDamage = 10f, bool checkTiles = false, bool grandDad = false)
        {
            PotentialEnergyDamage = potentialDamage;
            OldVelocity = npc.velocity;
            OlderVelocity = npc.velocity;
            terminalVelocityForFullFallDamage = terminalVelocityForFullDamage;
            CheckTiles = checkTiles;
            ForcedVel = forcedVel;
            attacker = player;
            GrandDad = grandDad;
        }

        /// <summary>
        /// Sets up a NPC to recieve forced velocity in a direction.
        /// </summary>
        /// <param name="npc">The npc to apply velocity to</param>
        /// <param name="player">The player applying velocity to the npc</param>
        /// <param name="forcedVel">The velocity being applied to the npc</param>
        /// <param name="checkTiles">Will check for tiles in the collision instead of velocity</param>
        public void ApplyForcedVelocity(NPC npc, Player player, Vector2 forcedVel, bool checkTiles = false)
        {
            OldVelocity = npc.velocity;
            OlderVelocity = npc.velocity;
            CheckTiles = checkTiles;
            ForcedVel = forcedVel;
            attacker = player;
            ForceVelocityOnly = true;
        }

        public override void PostAI(NPC npc)
        {
            if (FallDamageSusceptible)
            {
                if (CheckTiles)
                {
                    npc.Center += ForcedVel;
                    ForcedVel *= 0.995f;
                    Vector2 safeVel = ForcedVel.SafeNormalize(Vector2.UnitX);

                    if (npc.velocity == Vector2.Zero || ForcedVel.Length() < 2 || Collision.SolidCollision(npc.Center, (int)(npc.width * 0.5f), (int)(npc.height * 0.5f)) || !WorldGen.InWorld(npc.Center.ToTileCoordinates().X, npc.Center.ToTileCoordinates().Y, 40))
                    {
                        if (!ForceVelocityOnly && potentialEnergyDamage > 0)
                        {
                            float oldVelocity = OldVelocity.Length();
                            if (GrandDad)
                            {
                                float scale = (npc.boss ? 1.35f : 1) + Utils.GetLerpValue(20, 800, Math.Max(npc.width, npc.height));

                                attacker.SetScreenshake(7f * scale);
                                if (!CalamityClientConfig.Instance.Photosensitivity)
                                {
                                    Vector2 pulseVel = -safeVel;
                                    Particle pulse1 = new CustomSpark(npc.Center, pulseVel * 2, "CalamityMod/Particles/BloomCircle", false, (int)(23 * MathF.Pow(scale, 2)), 0.6f * MathF.Pow(scale, 2), Color.DodgerBlue, new Vector2(2f, 1f), shrinkSpeed: -0.1f / scale, noShrink: true, glowCenter: true, glowOpacity: 0.8f);
                                    GeneralParticleHandler.SpawnParticle(pulse1, true);
                                    Particle pulse2 = new CustomSpark(npc.Center, pulseVel, "CalamityMod/Particles/BloomRing", false, (int)(18 * MathF.Pow(scale, 2)), 0.5f * MathF.Pow(scale, 2), Color.Blue, new Vector2(4f, 0.7f), shrinkSpeed: -0.3f / scale, noShrink: true);
                                    GeneralParticleHandler.SpawnParticle(pulse2, true);
                                }


                                for (int i = 0; i < 2; i++)
                                {
                                    SoundStyle die = new("CalamityMod/Sounds/Item/HolyFireBulletExplosion");
                                    SoundEngine.PlaySound(die with { Pitch = 0.5f, Volume = Math.Min(0.6f * scale, 1f), MaxInstances = 2 }, npc.Center);
                                    if (npc.boss)
                                    {
                                        SoundStyle dieHard = new("CalamityMod/Sounds/Item/HellkiteSmallHit3");
                                        SoundEngine.PlaySound(dieHard with { Pitch = -1f, Volume = Math.Min(0.5f * scale, 1f), MaxInstances = 2 }, npc.Center);
                                    }
                                    SoundStyle dieHarder = new("CalamityMod/Sounds/Item/HeliumFlashCoreImpact");
                                    SoundEngine.PlaySound(dieHarder with { Pitch = -1.8f, Volume = Math.Min(0.6f * scale, 1f), MaxInstances = 2 }, npc.Center);
                                }
                                for (int i = 0; i < 30; i++)
                                {
                                    float variance = Main.rand.NextFloat(-0.8f, 0.8f);
                                    Dust dust = Dust.NewDustPerfect(npc.Center, ModContent.DustType<VoidDustPixelated>(),
                                        -safeVel.RotatedBy(variance * 2).RotatedByRandom(MathF.Abs(variance) / 3) * scale * Main.rand.NextFloat(30.0f, 33.0f) * MathF.Pow((1 - MathF.Abs(variance)), 2), 0, default, scale * Main.rand.NextFloat(2.3f, 3.2f) - MathF.Abs(variance));
                                    dust.noGravity = true;
                                    dust.color = Main.rand.NextBool() ? Color.DodgerBlue : Color.Blue;
                                    dust.customData = 2;
                                    dust.fadeIn = 0.5f * scale;
                                }
                                for (int i = 0; i < 15; i++)
                                {
                                    float variance = Main.rand.NextFloat(-0.8f, 0.8f);
                                    Vector2 vel = -safeVel.RotatedBy(variance / 2).RotatedByRandom(MathF.Abs(variance) / 3) * Main.rand.NextFloat(20.0f, 23.0f) * MathF.Pow((1 - MathF.Abs(variance)), 2);
                                    Dust dust2 = Dust.NewDustPerfect(npc.Center + vel * 13, ModContent.DustType<SquashDustPixelated>(),
                                        vel, 0, default, 2 * Main.rand.NextFloat(2.3f, 3.2f) - MathF.Abs(variance));
                                    dust2.noGravity = true;
                                    dust2.color = Main.rand.NextBool() ? Color.Gold : Color.Goldenrod;
                                    dust2.customData = new Vector2(0.2f, 1.6f);
                                    dust2.fadeIn = 2.5f;

                                }
                            }

                            bool instakill = (GrandDad && potentialEnergyDamage != 1);
                            int wallImpactDamage = (int)(PotentialEnergyDamage * Math.Clamp(oldVelocity / terminalVelocityForFullFallDamage, (ForcedVel.Length() > 0 ? 1f : 0f), 1f));
                            Projectile wallImpact = Projectile.NewProjectileDirect(attacker.GetSource_FromThis(), npc.Center, Vector2.Zero, instakill ? ModContent.ProjectileType<TileSlamDamage>() : ModContent.ProjectileType<DirectStrike>(), wallImpactDamage, 0f, attacker.whoAmI, npc.whoAmI);
                            wallImpact.DamageType = DamageClass.Melee;
                            wallImpact.CritChance = (int)attacker.GetCritChance(DamageClass.Melee) + attacker.HeldItem.crit;
                        }

                        PotentialEnergyDamage = 0;
                        ForceVelocityOnly = false;
                        hitVoid = false;
                    }

                    if (packetTimer % 30 == 0)
                        npc.SyncMotionToServer();
                    if (!WorldGen.InWorld(npc.Center.ToTileCoordinates().X, npc.Center.ToTileCoordinates().Y, 40) && !hitVoid)
                    {
                        npc.velocity = -npc.velocity * 0.5f;
                        ForcedVel = -ForcedVel * 0.5f;
                        hitVoid = true;
                    }
                    packetTimer++;
                }
                else
                {
                    float newVerticalVelocity = npc.velocity.Y;
                    float oldVerticalVelocity = OldVelocity.Y;
                    float olderVerticalVelocity = OlderVelocity.Y;

                    //If the thing flies back up, cancel the fall dmg
                    if (newVerticalVelocity < 0 && oldVerticalVelocity >= 0)
                        PotentialEnergyDamage = 0;

                    //Same goes for if it slows its fall. 
                    //We use 3 variables cuz there is a frame inbetween the fall and the landing where the velocity is still set to something.
                    if (newVerticalVelocity > 0 && olderVerticalVelocity > 0 && oldVerticalVelocity < olderVerticalVelocity)
                        PotentialEnergyDamage = 0;

                    //If the npc hit a tile/Came to a stop after falling
                    if (newVerticalVelocity == 0 && oldVerticalVelocity > 0) //Collision.SolidCollision(npc.Center, npc.width, npc.height))
                    {
                        if (!ForceVelocityOnly)
                        {
                            int impactDamage = (int)(PotentialEnergyDamage * Math.Clamp(olderVerticalVelocity / terminalVelocityForFullFallDamage, 0f, 1f));
                            Projectile impact = Projectile.NewProjectileDirect(attacker.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), impactDamage, 0f, attacker.whoAmI, npc.whoAmI);
                            impact.DamageType = DamageClass.Melee;
                        }

                        PotentialEnergyDamage = 0;
                        ForceVelocityOnly = false;
                    }

                    OlderVelocity = OldVelocity;
                    OldVelocity = npc.velocity;
                }
            }
        }
    }
}
