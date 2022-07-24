using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerHoldout : ModProjectile
    {
        int shottimer = 0; // Solely exists so that the Platinum shots aren't instantaneous
        bool rolling = true; // If the slot machine is currently rolling
        int rolltimer = 60; // Cooldown for the slot machine so that it doesn't instantly role
        int soundtimer = 0; // Counts how long the slot machine has been spinning + the cooldown
        public static readonly SoundStyle Win = new("CalamityMod/Sounds/Custom/AbilitySounds/FullAdrenaline");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seven's Striker");
            Main.projFrames[Projectile.type] = 19;
        }

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotoffset = Projectile.velocity.X <= 0 ? MathHelper.Pi : 0;
            Vector2 playerpos = player.RotatedRelativePoint(player.MountedCenter, true);
            bool shouldBeHeld = player.channel && !player.noItems && !player.CCed;

            int shot = ProjectileID.CopperCoin;
            float scaleFactor = 14f;
            int weaponDamage = player.GetWeaponDamage(player.ActiveItem());
            float weaponKnockback = player.ActiveItem().knockBack;

            if (Projectile.ai[1] == 0)
            {
                player.PickAmmo(player.ActiveItem(), out shot, out scaleFactor, out weaponDamage, out weaponKnockback, out _);
                Projectile.ai[0] = shot;
                Projectile.ai[1] = CalculateOutcome();
            }

            rolltimer--;
            soundtimer++;

            if (rolling)
            {
                Projectile.frameCounter++;
            }
            else
            {
                Projectile.frame = 0;
            }
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                rolling = false;
                rolltimer = 16;
                Projectile.frame = 0;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (shouldBeHeld && Projectile.ai[1] != 0)
                {
                    float holdscale = player.ActiveItem().shootSpeed * Projectile.scale;
                    Vector2 playerpos2 = playerpos;
                    Vector2 going = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - playerpos2;
                    if (player.gravDir == -1f)
                    {
                        going.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - playerpos2.Y;
                    }
                    Vector2 normalizedgoing = Vector2.Normalize(going);
                    if (float.IsNaN(normalizedgoing.X) || float.IsNaN(normalizedgoing.Y))
                    {
                        normalizedgoing = -Vector2.UnitY;
                    }
                    normalizedgoing *= holdscale;
                    if (normalizedgoing.X != Projectile.velocity.X || normalizedgoing.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = normalizedgoing * 0.55f;

                    if (!rolling && rolltimer <= 0)
                    {
                        //Jackpot gets special benefits since it shoots multiple rounds
                        if (Projectile.ai[1] == 4)
                        {
                            shottimer++;
                            if (shottimer == 1)
                            {
                                SoundEngine.PlaySound(Win with { Pitch = Win.Pitch + 0.5f}, Projectile.Center);
                                CombatText.NewText(player.getRect(), Color.Gold, "Jackpot!!!", true);
                            }
                            if (shottimer % 5 == 0 && shottimer > 5)
                            {
                                Shoot(7, ProjectileID.PlatinumCoin, weaponDamage, weaponKnockback, (int)scaleFactor * 2f, 0.15f);
                                SoundEngine.PlaySound(CommonCalamitySounds.GaussWeaponFire with { Pitch = CommonCalamitySounds.GaussWeaponFire.Pitch + 0.4f, Volume = CommonCalamitySounds.GaussWeaponFire.Volume - 0.4f }, Projectile.Center);
                            }
                            if (shottimer > 40)
                            {
                                soundtimer = 0;
                                rolling = true;
                                Projectile.ai[1] = 0;
                                shottimer = 0;
                            }
                        }
                        switch (Projectile.ai[1])
                        {
                            case 1:
                                Shoot(1, ModContent.ProjectileType<SevensStrikerBrick>(), (int)(Projectile.damage * 0.1f), 0, 2f, 0);
                                CombatText.NewText(player.getRect(), Color.Gray, "Bust!", true);
                                SoundEngine.PlaySound(SoundID.Item16 with { Pitch = SoundID.Item16.Pitch - 0.2f }, Projectile.Center);
                                break;
                            case 2:
                                Shoot(7, ModContent.ProjectileType<SevensStrikerOrange>(), Projectile.damage, Projectile.knockBack, 2f, 0.1f);
                                CombatText.NewText(player.getRect(), Color.Orange, "Doubles!", true);
                                SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                                break;
                            case 3:
                                Shoot(7, ModContent.ProjectileType<SevensStrikerCherry>(), Projectile.damage, Projectile.knockBack, 1.5f, 0.05f);
                                Shoot(7, ModContent.ProjectileType<SevensStrikerGrape>(), Projectile.damage, Projectile.knockBack, 2f, 0.1f);
                                CombatText.NewText(player.getRect(), Color.Red, "Triples!", true);
                                SoundEngine.PlaySound(SoundID.Item4 with { Pitch = SoundID.Item4.Pitch + 0.3f }, Projectile.Center);
                                break;
                        }
                        if (Projectile.ai[1] != 4)
                        {
                            Projectile.ai[1] = 0;
                            soundtimer = 0;
                            rolling = true;
                        }
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }

            // Sounds
            if (Projectile.frameCounter == 0 && Projectile.frame == 2 * (Main.projFrames[Projectile.type] / 19))
            {
                SoundEngine.PlaySound(SoundID.Item108 with { Volume = SoundID.Item108.Volume - 0.5f }, Projectile.Center);
            }
            if (soundtimer == 96 || soundtimer == 108 || soundtimer == 120)
            {
                SoundEngine.PlaySound(SoundID.Item17 with { Volume = SoundID.Item108.Volume - 0.5f }, Projectile.Center);
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + rotoffset;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        // Calculates which attack will occur based on coin. Platinum guarantees the best outcome.
        public int CalculateOutcome()
        {
            int outcome = 1;
            switch (Projectile.ai[0])
            {
                case ProjectileID.CopperCoin:
                    {
                        int roll = Main.rand.Next(100);
                        if (roll <= 50)
                            return 1;
                        else if (roll > 50 && roll <= 80)
                            return 2;
                        else if (roll > 80 && roll <= 95)
                            return 3;
                        else
                            return 4;
                    }
                case ProjectileID.SilverCoin:
                    {
                        int roll = Main.rand.Next(100);
                        if (roll <= 20)
                            return 1;
                        else if (roll > 20 && roll <= 70)
                            return 2;
                        else if (roll > 70 && roll <= 90)
                            return 3;
                        else
                            return 4;
                    }
                case ProjectileID.GoldCoin:
                    {
                        int roll = Main.rand.Next(100);
                        if (roll <= 5)
                            return 1;
                        else if (roll > 5 && roll <= 35)
                            return 2;
                        else if (roll > 35 && roll <= 85)
                            return 3;
                        else
                            return 4;
                    }
                case ProjectileID.PlatinumCoin:
                    return 4;
            }
            return outcome;
        }

        // Where the shooting takes place (wow!)
        public void Shoot(int projcount, int type, int damage, float kb, float scaleFactor, float spreadfactor)
        {
            for (int i = 0; i < projcount; ++i)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-spreadfactor, spreadfactor, i / 3f)) * scaleFactor;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, type, damage, kb, Main.player[Projectile.owner].whoAmI);
            }
        }

        public override bool? CanDamage() => false;
    }
}
