using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items;

namespace CalamityMod.Projectiles.Ranged
{
    public class FreedomStarHoldout : ModProjectile
    {
        private const float OrbLargeGateValue = 80f;
        private const float LaserGateValue = 180f;
        private const float LaserLargeGateValue = 660f;

        public override void SetStaticDefaults() => DisplayName.SetDefault("Freedom Star");

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.ai[0] += 1f;
            int chargeAmt = 0;
            if (Projectile.ai[0] >= OrbLargeGateValue)
                chargeAmt++;

            if (Projectile.ai[0] >= LaserGateValue)
                chargeAmt++;

            float lerpAmtForCharge = (Projectile.ai[0] - LaserGateValue) / (LaserLargeGateValue - LaserGateValue);
            if (lerpAmtForCharge > 1f)
                lerpAmtForCharge = 1f;

            bool shootLaser = Projectile.ai[0] >= LaserGateValue;
            int shootGateValue = 5;
            if (!shootLaser)
                Projectile.ai[1] += 1f;

            bool shootThisFrame = false;
            if (Projectile.ai[0] == 1f)
                shootThisFrame = true;

            if (shootLaser && Projectile.ai[0] % 20f == 0f)
                shootThisFrame = true;

            // Update position.
            if ((!shootLaser && Projectile.ai[1] >= shootGateValue) || shootLaser)
            {
                if (!shootLaser)
                    Projectile.ai[1] = 0f;

                shootThisFrame = true;
                float num21 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                Vector2 value11 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - Projectile.Center;
                if (player.gravDir == -1f)
                    value11.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y;

                Vector2 velocity2 = Vector2.Normalize(value11);
                if (float.IsNaN(velocity2.X) || float.IsNaN(velocity2.Y))
                    velocity2 = -Vector2.UnitY;

                velocity2 *= num21;
                if (velocity2.X != Projectile.velocity.X || velocity2.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;

                Projectile.velocity = velocity2;
            }

            // Emit sounds.
            if (Projectile.soundDelay <= 0 && !shootLaser)
            {
                Projectile.soundDelay = shootGateValue - chargeAmt;
                Projectile.soundDelay *= 2;
                if (Projectile.ai[0] != 1f)
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
            }

            // Dust spawning for orbs.
            if (Projectile.ai[0] > 5f && !shootLaser)
            {
                Vector2 spinningpoint3 = Vector2.UnitX * 32f;
                spinningpoint3 = spinningpoint3.RotatedBy(Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f));
                Vector2 value12 = Projectile.Center + spinningpoint3;

                for (int k = 0; k < chargeAmt + 1; k++)
                {
                    int num22 = 226;
                    float num23 = 0.4f;
                    if (k % 2 == 1)
                    {
                        num22 = 226;
                        num23 = 0.65f;
                    }

                    Vector2 vector3 = value12 + ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2() * (12f - chargeAmt * 2);
                    int num24 = Dust.NewDust(vector3 - Vector2.One * 8f, 16, 16, num22, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                    Main.dust[num24].velocity = Vector2.Normalize(value12 - vector3) * 1.5f * (10f - chargeAmt * 2f) / 10f;
                    Main.dust[num24].noGravity = true;
                    Main.dust[num24].scale = num23;
                    Main.dust[num24].customData = player;
                }
            }

            // Dust spawning for laser.
            if (shootLaser)
            {
                Vector2 spinningpoint4 = Vector2.UnitX * 32f;
                spinningpoint4 = spinningpoint4.RotatedBy(Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f));
                Vector2 vector4 = Projectile.Center + spinningpoint4;

                for (int l = 0; l < 2; l++)
                {
                    int num25 = 226;
                    float num26 = 0.35f;
                    if (l % 2 == 1)
                    {
                        num25 = 226;
                        num26 = 0.45f;
                    }
                    num26 *= MathHelper.Lerp(1f, 3f, lerpAmtForCharge);

                    float num27 = Main.rand.NextFloatDirection();
                    Vector2 value13 = vector4 + (Projectile.rotation + (Projectile.spriteDirection == -1 ? -MathHelper.PiOver2 : MathHelper.PiOver2) + num27 * ((float)Math.PI / 4f) * 0.8f - (float)Math.PI / 2f).ToRotationVector2() * 6f;
                    int num28 = 24;
                    int num29 = Dust.NewDust(value13 - Vector2.One * (num28 / 2), num28, num28, num25, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                    Main.dust[num29].velocity = (value13 - vector4).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(1.5f, 9f, Utils.GetLerpValue(1f, 0f, Math.Abs(num27), clamped: true));
                    Main.dust[num29].noGravity = true;
                    Main.dust[num29].scale = num26;
                    Main.dust[num29].customData = player;
                    Main.dust[num29].fadeIn = 0.5f;
                }
            }

            // Projectile shooting.
            if (shootThisFrame && Main.myPlayer == Projectile.owner)
            {
                Item freedomStar = player.ActiveItem();

                // This grabs the weapon's current damage, taking current charge level into account.
                int currentDamage = player.GetWeaponDamage(freedomStar);

                bool stillInUse = player.channel && !player.noItems && !player.CCed;

                // Checks if the Freedom Star has sufficient charge to fire. If this is false, it stops functioning.
                CalamityGlobalItem modItem = freedomStar.Calamity();
                float chargeConsumed = FreedomStar.HoldoutChargeUse_Orb;
                switch (chargeAmt)
                {
                    case 1:
                        chargeConsumed = FreedomStar.HoldoutChargeUse_OrbLarge;
                        break;

                    case 2:
                        chargeConsumed = MathHelper.Lerp(FreedomStar.HoldoutChargeUse_Laser, FreedomStar.HoldoutChargeUse_LaserLarge, lerpAmtForCharge);
                        break;

                    default:
                        break;
                }
                bool hasCharge = modItem.Charge >= chargeConsumed;

                if (stillInUse && hasCharge)
                {
                    // Use charge when firing anything.
                    modItem.Charge -= chargeConsumed;

                    if (Projectile.ai[0] == LaserGateValue)
                    {
                        SoundEngine.PlaySound(SoundID.Item124, Projectile.position);

                        Vector2 actualVelocity = Vector2.Normalize(Projectile.velocity);
                        if (float.IsNaN(actualVelocity.X) || float.IsNaN(actualVelocity.Y))
                            actualVelocity = -Vector2.UnitY;

                        int projectileDamage = (int)(currentDamage * 1.5f);
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, actualVelocity, ModContent.ProjectileType<FreedomStarBeam>(), projectileDamage, Projectile.knockBack, Projectile.owner, 0f, Projectile.whoAmI);
                        Projectile.ai[1] = proj;
                        Projectile.netUpdate = true;
                    }
                    else if (shootLaser)
                    {
                        Projectile projectile = Main.projectile[(int)Projectile.ai[1]];
                        if (!projectile.active || projectile.type != ModContent.ProjectileType<FreedomStarBeam>())
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        bool shootOrb = false;
                        if (Projectile.ai[0] == 1f)
                            shootOrb = true;
                        if (Projectile.ai[0] <= 50f && Projectile.ai[0] % 5f == 0f)
                            shootOrb = true;
                        if (Projectile.ai[0] >= OrbLargeGateValue && Projectile.ai[0] < LaserGateValue && Projectile.ai[0] % 10f == 0f)
                            shootOrb = true;

                        if (shootOrb)
                        {
                            SoundEngine.PlaySound(SoundID.Item75, Projectile.position);

                            int projectileType = ProjectileID.ChargedBlasterOrb;
                            float projectileVelocity = 10f;
                            Vector2 actualVelocity = Vector2.Normalize(Projectile.velocity) * projectileVelocity;
                            if (float.IsNaN(actualVelocity.X) || float.IsNaN(actualVelocity.Y))
                                actualVelocity = -Vector2.UnitY;

                            float orbPower = 0.7f + chargeAmt * 0.3f;
                            int projectileDamage = (orbPower < 1f) ? currentDamage : ((int)(currentDamage * 1.5f));
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, actualVelocity, projectileType, projectileDamage, Projectile.knockBack, Projectile.owner, 0f, orbPower);
                            Main.projectile[proj].DamageType = DamageClass.Ranged;
                            Main.projectile[proj].extraUpdates += 2;
                        }
                    }
                }
                else
                    Projectile.Kill();
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage() => false;
    }
}
