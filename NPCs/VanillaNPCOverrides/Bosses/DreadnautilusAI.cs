using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class DreadnautilusAI
    {
        public static bool BuffedDreadnautilusAI(NPC npc, Mod mod)
        {
            // Death Mode bool
            bool death = CalamityWorld.death;

            // Attack variables
            float goToAttackPositionAcceleration = death ? 0.2f : 0.15f;
            float goToAttackPositionVelocity = death ? 10f : 7.5f;
            float phaseSwitchPhaseTime = death ? 30f : 60f;
            float dashChargeUpPhaseTime = 120f;
            float dashPhaseTime = death ? 150f : 180f;
            float bloodSpitChargeUpPhaseTime = 90f;
            float bloodSpitPhaseTime = death ? 120f : 90f;
            int numBloodSpitVolleys = death ? 3 : 2;
            float bloodSquidPhaseTime = 180f;
            int maxBloodSquids = death ? 3 : 2;

            // Spawn effect
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                npc.alpha = 255;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = -1f;
                    npc.netUpdate = true;
                }
            }

            // Create dust
            if (npc.ai[0] != -1f && Main.rand.NextBool(4))
            {
                npc.position += npc.netOffset;
                Dust dust = Dust.NewDustDirect(npc.position + new Vector2(5f), npc.width - 10, npc.height - 10, 5);
                dust.velocity *= 0.5f;
                if (dust.velocity.Y < 0f)
                    dust.velocity.Y *= -1f;

                dust.alpha = 120;
                dust.scale = 1f + Main.rand.NextFloat() * 0.4f;
                dust.velocity += npc.velocity * 0.3f;
                npc.position -= npc.netOffset;
            }

            // Get a target
            if (npc.target == Main.maxPlayers)
            {
                npc.TargetClosest();
                npc.ai[2] = npc.direction;
            }

            // Get a new target if the current target is dead or too far away
            if (Main.player[npc.target].dead || Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 2000f)
                npc.TargetClosest();

            // Set to despawn
            NPCAimedTarget nPCAimedTarget = npc.GetTargetData();
            if (Main.dayTime || !Main.bloodMoon)
                nPCAimedTarget = default(NPCAimedTarget);

            // Attacks and shit
            int attackType = -1;
            switch ((int)npc.ai[0])
            {
                // Spawn effects
                case -1:
                    {
                        npc.velocity *= 0.98f;
                        int spawnFaceDirection = Math.Sign(nPCAimedTarget.Center.X - npc.Center.X);
                        if (spawnFaceDirection != 0)
                        {
                            npc.direction = spawnFaceDirection;
                            npc.spriteDirection = -npc.direction;
                        }

                        if (npc.localAI[1] == 0f && npc.alpha < 100)
                        {
                            npc.localAI[1] = 1f;
                            int dustAmt = 36;
                            for (int l = 0; l < dustAmt; l++)
                            {
                                npc.position += npc.netOffset;
                                Vector2 dustRotation = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((l - (dustAmt / 2 - 1)) * ((float)Math.PI * 2f) / dustAmt) + npc.Center;
                                Vector2 dustVelocity = dustRotation - npc.Center;
                                int spawnDustBlood = Dust.NewDust(dustRotation + dustVelocity, 0, 0, 5, dustVelocity.X * 2f, dustVelocity.Y * 2f, 100, default, 1.4f);
                                Main.dust[spawnDustBlood].noGravity = true;
                                Main.dust[spawnDustBlood].velocity = Vector2.Normalize(dustVelocity) * 3f;
                                npc.position -= npc.netOffset;
                            }
                        }

                        if (npc.ai[2] > 5f)
                        {
                            npc.velocity.Y = -2.5f;
                            npc.alpha -= 10;
                            if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                            {
                                npc.alpha += 15;
                                if (npc.alpha > 150)
                                    npc.alpha = 150;
                            }

                            if (npc.alpha < 0)
                                npc.alpha = 0;
                        }

                        npc.ai[2] += 1f;
                        if (npc.ai[2] >= 50f)
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.netUpdate = true;
                        }

                        break;
                    }

                // Get in position for an attack and then choose an attack type
                case 0:
                    {
                        Vector2 destination = nPCAimedTarget.Center + new Vector2((0f - npc.ai[2]) * 500f, -300f);
                        if (npc.Center.Distance(destination) > 50f)
                        {
                            Vector2 desiredVelocity = npc.DirectionTo(destination) * goToAttackPositionVelocity;
                            npc.SimpleFlyMovement(desiredVelocity, goToAttackPositionAcceleration);
                        }

                        npc.direction = (npc.Center.X < nPCAimedTarget.Center.X) ? 1 : (-1);
                        float faceTargetDirection = npc.Center.DirectionTo(nPCAimedTarget.Center).ToRotation() - 213f / 452f * npc.spriteDirection;
                        if (npc.spriteDirection == -1)
                            faceTargetDirection += (float)Math.PI;

                        if (npc.spriteDirection != npc.direction)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.rotation = 0f - npc.rotation;
                            faceTargetDirection = 0f - faceTargetDirection;
                        }

                        npc.rotation = npc.rotation.AngleTowards(faceTargetDirection, 0.02f);
                        npc.ai[1] += 1f;
                        if (npc.ai[1] > phaseSwitchPhaseTime)
                        {
                            int attackPicker = (int)npc.ai[3];
                            if (attackPicker % 7 == 3 && NPC.CountNPCS(NPCID.BloodSquid) < maxBloodSquids)
                            {
                                attackType = 3;
                            }
                            else if (attackPicker % 2 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item170, npc.Center);
                                attackType = 2;
                            }
                            else
                            {
                                SoundEngine.PlaySound(SoundID.Item170, npc.Center);
                                attackType = 1;
                            }
                        }

                        break;
                    }

                // Dash
                case 1:
                    {
                        npc.direction = (!(npc.Center.X < nPCAimedTarget.Center.X)) ? 1 : (-1);
                        float chargeFaceDirection = npc.Center.DirectionFrom(nPCAimedTarget.Center).ToRotation() - 213f / 452f * npc.spriteDirection;
                        if (npc.spriteDirection == -1)
                            chargeFaceDirection += (float)Math.PI;

                        bool shouldStartCharge = npc.ai[1] < dashChargeUpPhaseTime;
                        if (npc.spriteDirection != npc.direction && shouldStartCharge)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.rotation = 0f - npc.rotation;
                            chargeFaceDirection = 0f - chargeFaceDirection;
                        }

                        if (npc.ai[1] < dashChargeUpPhaseTime)
                        {
                            if (npc.ai[1] == dashChargeUpPhaseTime - 1f)
                                SoundEngine.PlaySound(SoundID.Item172, npc.Center);

                            npc.velocity *= 0.95f;
                            npc.rotation = npc.rotation.AngleLerp(chargeFaceDirection, 0.02f);
                            npc.position += npc.netOffset;
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition4, out Vector2 mouthDirection4);
                            Dust chargeUpDust = Dust.NewDustDirect(mouthPosition4 + mouthDirection4 * 60f - new Vector2(40f), 80, 80, 16, 0f, 0f, 150, Color.Transparent, 0.6f);
                            chargeUpDust.fadeIn = 1f;
                            chargeUpDust.velocity = chargeUpDust.position.DirectionTo(mouthPosition4 + Main.rand.NextVector2Circular(15f, 15f)) * chargeUpDust.velocity.Length();
                            chargeUpDust.noGravity = true;
                            chargeUpDust = Dust.NewDustDirect(mouthPosition4 + mouthDirection4 * 100f - new Vector2(30f), 60, 60, 16, 0f, 0f, 100, Color.Transparent, 0.9f);
                            chargeUpDust.fadeIn = 1.5f;
                            chargeUpDust.velocity = chargeUpDust.position.DirectionTo(mouthPosition4 + Main.rand.NextVector2Circular(15f, 15f)) * (chargeUpDust.velocity.Length() + 5f);
                            chargeUpDust.noGravity = true;
                            npc.position -= npc.netOffset;
                        }
                        else if (npc.ai[1] < dashChargeUpPhaseTime + dashPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            npc.rotation = npc.rotation.AngleLerp(chargeFaceDirection, 0.07f);
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition5, out Vector2 mouthDirection5);

                            // Dash directly towards the target until within 15 tiles of the target, and then continue in the same direction for 18 frames (15 frames in Death Mode)
                            if (npc.ai[1] < dashChargeUpPhaseTime + dashPhaseTime * 0.9f)
                            {
                                if (npc.Center.Distance(nPCAimedTarget.Center) > 240f || npc.ai[1] == dashChargeUpPhaseTime)
                                    npc.velocity = mouthDirection5 * -(death ? 20f : 16f) + npc.Center.DirectionTo(nPCAimedTarget.Center) * 2f;
                                else
                                    npc.ai[1] = dashChargeUpPhaseTime + dashPhaseTime * 0.9f;
                            }

                            for (int m = 0; m < 4; m++)
                            {
                                Dust chargeBloodDust = Dust.NewDustDirect(mouthPosition5 + mouthDirection5 * 60f - new Vector2(15f), 30, 30, 5, 0f, 0f, 0, Color.Transparent, 1.5f);
                                chargeBloodDust.velocity = chargeBloodDust.position.DirectionFrom(mouthPosition5 + Main.rand.NextVector2Circular(5f, 5f)) * chargeBloodDust.velocity.Length();
                                chargeBloodDust.position -= mouthDirection5 * 60f;
                                chargeBloodDust = Dust.NewDustDirect(mouthPosition5 + mouthDirection5 * 100f - new Vector2(20f), 40, 40, 5, 0f, 0f, 100, Color.Transparent, 1.5f);
                                chargeBloodDust.velocity = chargeBloodDust.position.DirectionFrom(mouthPosition5 + Main.rand.NextVector2Circular(10f, 10f)) * (chargeBloodDust.velocity.Length() + 5f);
                                chargeBloodDust.position -= mouthDirection5 * 100f;
                            }

                            npc.position -= npc.netOffset;
                        }

                        npc.ai[1] += 1f;
                        if (npc.ai[1] >= dashChargeUpPhaseTime + dashPhaseTime)
                            attackType = 0;

                        break;
                    }

                // Spit 3 spreads of blood projectiles
                case 2:
                    {
                        npc.direction = (npc.Center.X < nPCAimedTarget.Center.X) ? 1 : (-1);
                        float bloodProjFaceDirection = npc.Center.DirectionTo(nPCAimedTarget.Center).ToRotation() - 213f / 452f * npc.spriteDirection;
                        if (npc.spriteDirection == -1)
                            bloodProjFaceDirection += (float)Math.PI;

                        if (npc.spriteDirection != npc.direction)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.rotation = 0f - npc.rotation;
                            bloodProjFaceDirection = 0f - bloodProjFaceDirection;
                        }

                        npc.rotation = npc.rotation.AngleLerp(bloodProjFaceDirection, 0.2f);
                        if (npc.ai[1] < bloodSpitChargeUpPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            npc.velocity *= 0.95f;
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition2, out Vector2 mouthDirection2);
                            if (Main.rand.Next(4) != 0)
                            {
                                Dust bloodProjChargeUpDust = Dust.NewDustDirect(mouthPosition2 + mouthDirection2 * 60f - new Vector2(60f), 120, 120, 16, 0f, 0f, 150, Color.Transparent, 0.6f);
                                bloodProjChargeUpDust.fadeIn = 1f;
                                bloodProjChargeUpDust.velocity = bloodProjChargeUpDust.position.DirectionTo(mouthPosition2 + Main.rand.NextVector2Circular(15f, 15f)) * (bloodProjChargeUpDust.velocity.Length() + 3f);
                                bloodProjChargeUpDust.noGravity = true;
                                bloodProjChargeUpDust = Dust.NewDustDirect(mouthPosition2 + mouthDirection2 * 100f - new Vector2(80f), 160, 160, 16, 0f, 0f, 100, Color.Transparent, 0.9f);
                                bloodProjChargeUpDust.fadeIn = 1.5f;
                                bloodProjChargeUpDust.velocity = bloodProjChargeUpDust.position.DirectionTo(mouthPosition2 + Main.rand.NextVector2Circular(15f, 15f)) * (bloodProjChargeUpDust.velocity.Length() + 5f);
                                bloodProjChargeUpDust.noGravity = true;
                            }

                            npc.position -= npc.netOffset;
                        }
                        else if (npc.ai[1] < bloodSpitChargeUpPhaseTime + bloodSpitPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            npc.velocity *= 0.9f;
                            float bloodProjShootTimer = (npc.ai[1] - bloodSpitChargeUpPhaseTime) % (bloodSpitPhaseTime / numBloodSpitVolleys);
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition3, out Vector2 mouthDirection3);
                            if (bloodProjShootTimer < bloodSpitPhaseTime / numBloodSpitVolleys * 0.8f)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Dust bloodProjShootDust = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 50f - new Vector2(15f), 30, 30, 5, 0f, 0f, 0, Color.Transparent, 1.5f);
                                    bloodProjShootDust.velocity = bloodProjShootDust.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(5f, 5f)) * bloodProjShootDust.velocity.Length();
                                    bloodProjShootDust.position -= mouthDirection3 * 60f;
                                    bloodProjShootDust = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 90f - new Vector2(20f), 40, 40, 5, 0f, 0f, 100, Color.Transparent, 1.5f);
                                    bloodProjShootDust.velocity = bloodProjShootDust.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(10f, 10f)) * (bloodProjShootDust.velocity.Length() + 5f);
                                    bloodProjShootDust.position -= mouthDirection3 * 100f;
                                }
                            }

                            // Spit blood spread
                            if ((int)bloodProjShootTimer == 0)
                            {
                                // Recoil away with each spit
                                npc.velocity += mouthDirection3 * -8f;

                                // Spawn dust with each spit
                                for (int j = 0; j < 20; j++)
                                {
                                    Dust bloodProjShootDust2 = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 60f - new Vector2(15f), 30, 30, 5, 0f, 0f, 0, Color.Transparent, 1.5f);
                                    bloodProjShootDust2.velocity = bloodProjShootDust2.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(5f, 5f)) * bloodProjShootDust2.velocity.Length();
                                    bloodProjShootDust2.position -= mouthDirection3 * 60f;
                                    bloodProjShootDust2 = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 100f - new Vector2(20f), 40, 40, 5, 0f, 0f, 100, Color.Transparent, 1.5f);
                                    bloodProjShootDust2.velocity = bloodProjShootDust2.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(10f, 10f)) * (bloodProjShootDust2.velocity.Length() + 5f);
                                    bloodProjShootDust2.position -= mouthDirection3 * 100f;
                                }

                                // Spawn projectiles
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int projectileAmt = death ? 6 : 5;
                                    int spread = death ? 35 : 30;
                                    float rotation = MathHelper.ToRadians(spread);
                                    Vector2 initialProjectileVelocity = mouthDirection3 * 10f;
                                    int damage = npc.GetAttackDamage_ForProjectiles(30f, 25f);
                                    for (int k = 0; k < projectileAmt + 1; k++)
                                    {
                                        Vector2 perturbedSpeed = initialProjectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, k / (float)(projectileAmt - 1)));
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), mouthPosition3 - mouthDirection3 * 5f, initialProjectileVelocity + perturbedSpeed, ProjectileID.BloodNautilusShot, damage, 0f, Main.myPlayer);
                                    }
                                }
                            }

                            npc.position -= npc.netOffset;
                        }

                        npc.ai[1] += 1f;
                        if (npc.ai[1] >= bloodSpitChargeUpPhaseTime + bloodSpitPhaseTime)
                            attackType = 0;

                        break;
                    }

                // Spawn Blood Squids
                case 3:
                    {
                        npc.direction = (npc.Center.X < nPCAimedTarget.Center.X) ? 1 : (-1);
                        float targetAngle = 0f;
                        npc.spriteDirection = npc.direction;
                        if (npc.ai[1] < bloodSquidPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            float bloodSquidVelClamp = MathHelper.Clamp(1f - npc.ai[1] / bloodSquidPhaseTime * 1.5f, 0f, 1f);
                            npc.velocity = Vector2.Lerp(value2: new Vector2(0f, bloodSquidVelClamp * -1.5f), value1: npc.velocity, amount: 0.03f);
                            npc.velocity = Vector2.Zero;
                            npc.rotation = npc.rotation.AngleLerp(targetAngle, 0.02f);
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 _, out Vector2 _);
                            float t = npc.ai[1] / bloodSquidPhaseTime;
                            float scaleFactor2 = Utils.GetLerpValue(0f, 0.5f, t) * Utils.GetLerpValue(1f, 0.5f, t);
                            Lighting.AddLight(npc.Center, new Vector3(1f, 0.5f, 0.5f) * scaleFactor2);
                            if (Main.rand.Next(3) != 0)
                            {
                                Dust bloodSquidSpawnDust = Dust.NewDustDirect(npc.Center - new Vector2(6f), 12, 12, 5, 0f, 0f, 60, Color.Transparent, 1.4f);
                                bloodSquidSpawnDust.position += new Vector2(npc.spriteDirection * 12, 12f);
                                bloodSquidSpawnDust.velocity *= 0.1f;
                            }

                            npc.position -= npc.netOffset;
                        }

                        if (npc.ai[1] == 10f || (death && npc.ai[1] == 20f) || npc.ai[1] == 30f)
                            BloodNautilus_CallForHelp(npc);

                        npc.ai[1] += 1f;
                        if (npc.ai[1] >= bloodSquidPhaseTime)
                            attackType = 0;

                        break;
                    }
            }

            // Set AI arrays for the next attack
            if (attackType != -1)
            {
                npc.ai[0] = attackType;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.netUpdate = true;
                npc.TargetClosest();
                if (attackType == 0)
                    npc.ai[2] = npc.direction;
                else
                    npc.ai[3] += 1f;
            }

            // Always set this to false because it's fucking stupid
            npc.reflectsProjectiles = false;

            return false;
        }

        private static void BloodNautilus_CallForHelp(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !Main.player[npc.target].active || Main.player[npc.target].dead || npc.Distance(Main.player[npc.target].Center) > 2000f)
                return;

            Point npcCenterTileCoords = npc.Center.ToTileCoordinates();
            Point npcCenterTileCoordsCopy = npcCenterTileCoords;
            int bloodTearRandSpawnOffset = 20;
            int npcCenterTileRadius = 3;
            int npcCenterCopyTileRadius = 8;
            int bloodTearSpawnTileRadius = 2;
            int attempts = 0;
            int bloodTearTileX;
            int bloodTearTileY;
            while (true)
            {
                if (attempts >= 100)
                    return;

                attempts++;
                bloodTearTileX = Main.rand.Next(npcCenterTileCoordsCopy.X - bloodTearRandSpawnOffset, npcCenterTileCoordsCopy.X + bloodTearRandSpawnOffset + 1);
                bloodTearTileY = Main.rand.Next(npcCenterTileCoordsCopy.Y - bloodTearRandSpawnOffset, npcCenterTileCoordsCopy.Y + bloodTearRandSpawnOffset + 1);
                if ((bloodTearTileY < npcCenterTileCoordsCopy.Y - npcCenterCopyTileRadius || bloodTearTileY > npcCenterTileCoordsCopy.Y + npcCenterCopyTileRadius || bloodTearTileX < npcCenterTileCoordsCopy.X - npcCenterCopyTileRadius || bloodTearTileX > npcCenterTileCoordsCopy.X + npcCenterCopyTileRadius) && (bloodTearTileY < npcCenterTileCoords.Y - npcCenterTileRadius || bloodTearTileY > npcCenterTileCoords.Y + npcCenterTileRadius || bloodTearTileX < npcCenterTileCoords.X - npcCenterTileRadius || bloodTearTileX > npcCenterTileCoords.X + npcCenterTileRadius) && !Main.tile[bloodTearTileX, bloodTearTileY].HasUnactuatedTile)
                {
                    bool spawnBloodTear = true;
                    if (spawnBloodTear && Main.tile[bloodTearTileX, bloodTearTileY].LiquidType == LiquidID.Lava)
                        spawnBloodTear = false;

                    if (spawnBloodTear && Collision.SolidTiles(bloodTearTileX - bloodTearSpawnTileRadius, bloodTearTileX + bloodTearSpawnTileRadius, bloodTearTileY - bloodTearSpawnTileRadius, bloodTearTileY + bloodTearSpawnTileRadius))
                        spawnBloodTear = false;

                    if (spawnBloodTear && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        spawnBloodTear = false;

                    if (spawnBloodTear)
                        break;
                }
            }

            Projectile.NewProjectile(npc.GetSource_FromAI(), bloodTearTileX * 16 + 8, bloodTearTileY * 16 + 8, 0f, 0f, ProjectileID.BloodNautilusTears, 0, 0f, Main.myPlayer);
        }
    }
}
