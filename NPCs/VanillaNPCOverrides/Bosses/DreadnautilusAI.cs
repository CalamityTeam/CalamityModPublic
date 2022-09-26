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
                        int num15 = Math.Sign(nPCAimedTarget.Center.X - npc.Center.X);
                        if (num15 != 0)
                        {
                            npc.direction = num15;
                            npc.spriteDirection = -npc.direction;
                        }

                        if (npc.localAI[1] == 0f && npc.alpha < 100)
                        {
                            npc.localAI[1] = 1f;
                            int num16 = 36;
                            for (int l = 0; l < num16; l++)
                            {
                                npc.position += npc.netOffset;
                                Vector2 value3 = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((l - (num16 / 2 - 1)) * ((float)Math.PI * 2f) / num16) + npc.Center;
                                Vector2 value4 = value3 - npc.Center;
                                int num17 = Dust.NewDust(value3 + value4, 0, 0, 5, value4.X * 2f, value4.Y * 2f, 100, default, 1.4f);
                                Main.dust[num17].noGravity = true;
                                Main.dust[num17].velocity = Vector2.Normalize(value4) * 3f;
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
                        float num13 = npc.Center.DirectionTo(nPCAimedTarget.Center).ToRotation() - 213f / 452f * npc.spriteDirection;
                        if (npc.spriteDirection == -1)
                            num13 += (float)Math.PI;

                        if (npc.spriteDirection != npc.direction)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.rotation = 0f - npc.rotation;
                            num13 = 0f - num13;
                        }

                        npc.rotation = npc.rotation.AngleTowards(num13, 0.02f);
                        npc.ai[1] += 1f;
                        if (npc.ai[1] > phaseSwitchPhaseTime)
                        {
                            int num14 = (int)npc.ai[3];
                            if (num14 % 7 == 3 && NPC.CountNPCS(NPCID.BloodSquid) < maxBloodSquids)
                            {
                                attackType = 3;
                            }
                            else if (num14 % 2 == 0)
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
                        float num18 = npc.Center.DirectionFrom(nPCAimedTarget.Center).ToRotation() - 213f / 452f * npc.spriteDirection;
                        if (npc.spriteDirection == -1)
                            num18 += (float)Math.PI;

                        bool flag2 = npc.ai[1] < dashChargeUpPhaseTime;
                        if (npc.spriteDirection != npc.direction && flag2)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.rotation = 0f - npc.rotation;
                            num18 = 0f - num18;
                        }

                        if (npc.ai[1] < dashChargeUpPhaseTime)
                        {
                            if (npc.ai[1] == dashChargeUpPhaseTime - 1f)
                                SoundEngine.PlaySound(SoundID.Item172, npc.Center);

                            npc.velocity *= 0.95f;
                            npc.rotation = npc.rotation.AngleLerp(num18, 0.02f);
                            npc.position += npc.netOffset;
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition4, out Vector2 mouthDirection4);
                            Dust dust6 = Dust.NewDustDirect(mouthPosition4 + mouthDirection4 * 60f - new Vector2(40f), 80, 80, 16, 0f, 0f, 150, Color.Transparent, 0.6f);
                            dust6.fadeIn = 1f;
                            dust6.velocity = dust6.position.DirectionTo(mouthPosition4 + Main.rand.NextVector2Circular(15f, 15f)) * dust6.velocity.Length();
                            dust6.noGravity = true;
                            dust6 = Dust.NewDustDirect(mouthPosition4 + mouthDirection4 * 100f - new Vector2(30f), 60, 60, 16, 0f, 0f, 100, Color.Transparent, 0.9f);
                            dust6.fadeIn = 1.5f;
                            dust6.velocity = dust6.position.DirectionTo(mouthPosition4 + Main.rand.NextVector2Circular(15f, 15f)) * (dust6.velocity.Length() + 5f);
                            dust6.noGravity = true;
                            npc.position -= npc.netOffset;
                        }
                        else if (npc.ai[1] < dashChargeUpPhaseTime + dashPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            npc.rotation = npc.rotation.AngleLerp(num18, 0.07f);
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
                                Dust dust7 = Dust.NewDustDirect(mouthPosition5 + mouthDirection5 * 60f - new Vector2(15f), 30, 30, 5, 0f, 0f, 0, Color.Transparent, 1.5f);
                                dust7.velocity = dust7.position.DirectionFrom(mouthPosition5 + Main.rand.NextVector2Circular(5f, 5f)) * dust7.velocity.Length();
                                dust7.position -= mouthDirection5 * 60f;
                                dust7 = Dust.NewDustDirect(mouthPosition5 + mouthDirection5 * 100f - new Vector2(20f), 40, 40, 5, 0f, 0f, 100, Color.Transparent, 1.5f);
                                dust7.velocity = dust7.position.DirectionFrom(mouthPosition5 + Main.rand.NextVector2Circular(10f, 10f)) * (dust7.velocity.Length() + 5f);
                                dust7.position -= mouthDirection5 * 100f;
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
                        float num10 = npc.Center.DirectionTo(nPCAimedTarget.Center).ToRotation() - 213f / 452f * npc.spriteDirection;
                        if (npc.spriteDirection == -1)
                            num10 += (float)Math.PI;

                        if (npc.spriteDirection != npc.direction)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.rotation = 0f - npc.rotation;
                            num10 = 0f - num10;
                        }

                        npc.rotation = npc.rotation.AngleLerp(num10, 0.2f);
                        if (npc.ai[1] < bloodSpitChargeUpPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            npc.velocity *= 0.95f;
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition2, out Vector2 mouthDirection2);
                            if (Main.rand.Next(4) != 0)
                            {
                                Dust dust3 = Dust.NewDustDirect(mouthPosition2 + mouthDirection2 * 60f - new Vector2(60f), 120, 120, 16, 0f, 0f, 150, Color.Transparent, 0.6f);
                                dust3.fadeIn = 1f;
                                dust3.velocity = dust3.position.DirectionTo(mouthPosition2 + Main.rand.NextVector2Circular(15f, 15f)) * (dust3.velocity.Length() + 3f);
                                dust3.noGravity = true;
                                dust3 = Dust.NewDustDirect(mouthPosition2 + mouthDirection2 * 100f - new Vector2(80f), 160, 160, 16, 0f, 0f, 100, Color.Transparent, 0.9f);
                                dust3.fadeIn = 1.5f;
                                dust3.velocity = dust3.position.DirectionTo(mouthPosition2 + Main.rand.NextVector2Circular(15f, 15f)) * (dust3.velocity.Length() + 5f);
                                dust3.noGravity = true;
                            }

                            npc.position -= npc.netOffset;
                        }
                        else if (npc.ai[1] < bloodSpitChargeUpPhaseTime + bloodSpitPhaseTime)
                        {
                            npc.position += npc.netOffset;
                            npc.velocity *= 0.9f;
                            float num11 = (npc.ai[1] - bloodSpitChargeUpPhaseTime) % (bloodSpitPhaseTime / numBloodSpitVolleys);
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 mouthPosition3, out Vector2 mouthDirection3);
                            if (num11 < bloodSpitPhaseTime / numBloodSpitVolleys * 0.8f)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Dust dust4 = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 50f - new Vector2(15f), 30, 30, 5, 0f, 0f, 0, Color.Transparent, 1.5f);
                                    dust4.velocity = dust4.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(5f, 5f)) * dust4.velocity.Length();
                                    dust4.position -= mouthDirection3 * 60f;
                                    dust4 = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 90f - new Vector2(20f), 40, 40, 5, 0f, 0f, 100, Color.Transparent, 1.5f);
                                    dust4.velocity = dust4.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(10f, 10f)) * (dust4.velocity.Length() + 5f);
                                    dust4.position -= mouthDirection3 * 100f;
                                }
                            }

                            // Spit blood spread
                            if ((int)num11 == 0)
                            {
                                // Recoil away with each spit
                                npc.velocity += mouthDirection3 * -8f;

                                // Spawn dust with each spit
                                for (int j = 0; j < 20; j++)
                                {
                                    Dust dust5 = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 60f - new Vector2(15f), 30, 30, 5, 0f, 0f, 0, Color.Transparent, 1.5f);
                                    dust5.velocity = dust5.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(5f, 5f)) * dust5.velocity.Length();
                                    dust5.position -= mouthDirection3 * 60f;
                                    dust5 = Dust.NewDustDirect(mouthPosition3 + mouthDirection3 * 100f - new Vector2(20f), 40, 40, 5, 0f, 0f, 100, Color.Transparent, 1.5f);
                                    dust5.velocity = dust5.position.DirectionFrom(mouthPosition3 + Main.rand.NextVector2Circular(10f, 10f)) * (dust5.velocity.Length() + 5f);
                                    dust5.position -= mouthDirection3 * 100f;
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
                            float num9 = MathHelper.Clamp(1f - npc.ai[1] / bloodSquidPhaseTime * 1.5f, 0f, 1f);
                            npc.velocity = Vector2.Lerp(value2: new Vector2(0f, num9 * -1.5f), value1: npc.velocity, amount: 0.03f);
                            npc.velocity = Vector2.Zero;
                            npc.rotation = npc.rotation.AngleLerp(targetAngle, 0.02f);
                            npc.BloodNautilus_GetMouthPositionAndRotation(out Vector2 _, out Vector2 _);
                            float t = npc.ai[1] / bloodSquidPhaseTime;
                            float scaleFactor2 = Utils.GetLerpValue(0f, 0.5f, t) * Utils.GetLerpValue(1f, 0.5f, t);
                            Lighting.AddLight(npc.Center, new Vector3(1f, 0.5f, 0.5f) * scaleFactor2);
                            if (Main.rand.Next(3) != 0)
                            {
                                Dust dust2 = Dust.NewDustDirect(npc.Center - new Vector2(6f), 12, 12, 5, 0f, 0f, 60, Color.Transparent, 1.4f);
                                dust2.position += new Vector2(npc.spriteDirection * 12, 12f);
                                dust2.velocity *= 0.1f;
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

            Point point = npc.Center.ToTileCoordinates();
            Point point2 = point;
            int num = 20;
            int num2 = 3;
            int num3 = 8;
            int num4 = 2;
            int attempts = 0;
            int num6;
            int num7;
            while (true)
            {
                if (attempts >= 100)
                    return;

                attempts++;
                num6 = Main.rand.Next(point2.X - num, point2.X + num + 1);
                num7 = Main.rand.Next(point2.Y - num, point2.Y + num + 1);
                if ((num7 < point2.Y - num3 || num7 > point2.Y + num3 || num6 < point2.X - num3 || num6 > point2.X + num3) && (num7 < point.Y - num2 || num7 > point.Y + num2 || num6 < point.X - num2 || num6 > point.X + num2) && !Main.tile[num6, num7].HasUnactuatedTile)
                {
                    bool spawnBloodTear = true;
                    if (spawnBloodTear && Main.tile[num6, num7].LiquidType == LiquidID.Lava)
                        spawnBloodTear = false;

                    if (spawnBloodTear && Collision.SolidTiles(num6 - num4, num6 + num4, num7 - num4, num7 + num4))
                        spawnBloodTear = false;

                    if (spawnBloodTear && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        spawnBloodTear = false;

                    if (spawnBloodTear)
                        break;
                }
            }

            Projectile.NewProjectile(npc.GetSource_FromAI(), num6 * 16 + 8, num7 * 16 + 8, 0f, 0f, ProjectileID.BloodNautilusTears, 0, 0f, Main.myPlayer);
        }
    }
}
