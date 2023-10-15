using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheSevensStriker>();
        public bool rolling = true; // If the slot machine is currently rolling
        public bool shotonce = false; // So that the first shot doesn't consume two ammo
        public int shottimer = 0; // Solely exists so that the Platinum shots aren't instantaneous
        public int rolltimer = 60; // Cooldown for the slot machine so that it doesn't instantly role
        public int soundtimer = 0; // Counts how long the slot machine has been spinning + the cooldown

        public SlotId RouletteSoundSlot;

        public override void SetStaticDefaults()
        {
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
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerpos = player.RotatedRelativePoint(player.MountedCenter, true);
            bool shouldBeHeld = player.channel && !player.noItems && !player.CCed;

            int shot;
            float scaleFactor = 14f;
            int weaponDamage = player.GetWeaponDamage(player.ActiveItem());
            float weaponKnockback = player.ActiveItem().knockBack;

            // Consumes a coin, stores it, then calculates what effect will be executed
            if (Projectile.ai[1] == 0)
            {
                // These checks are here so that the weapon doesn't consume two coins when first used
                if (shotonce)
                {
                    if (player.PickAmmo(player.ActiveItem(), out shot, out scaleFactor, out weaponDamage, out weaponKnockback, out _))
                    {
                        Projectile.ai[0] = shot;
                        Projectile.ai[1] = CalculateOutcome();
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.ai[1] = CalculateOutcome();
                    shotonce = true;
                }
            }

            rolltimer--;
            soundtimer++;

            // While the slot machine is rolling, play the animation
            if (rolling)
            {
                Projectile.frameCounter++;
            }
            // Make sure that it defaults at frame 1
            else
            {
                Projectile.frame = 0;
            }
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            // Once the animation is finished, stop rolling, set the extra timer to 16 frames, and reset the sprite to frame 0
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
                    // Holdout stuff
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

                    // If the animation isn't playing and the cooldown timer is at or below 0
                    if (!rolling && rolltimer <= 0)
                    {
                        // Jackpot gets special benefits since it shoots multiple rounds
                        if (Projectile.ai[1] == 4)
                        {
                            shottimer++;
                            // Play a sound and display the jackpot text
                            if (shottimer == 1)
                            {
                                SoundEngine.PlaySound(TheSevensStriker.JackpotSound, Projectile.Center);
                                CombatText.NewText(player.getRect(), Color.Gold, CalamityUtils.GetTextValue("Misc.SevensJackpot"), true);
                            }
                            // Every 7 frames, shoot 7 coins. The first 7 frames are excluded for timing purposes
                            if (shottimer % 7 == 0 && shottimer > 7)
                            {
                                int jackpotDamage = (int)(weaponDamage * TheSevensStriker.JackpotMultiplier);
                                Shoot(7, ModContent.ProjectileType<SevensStrikerPlatinumCoin>(), jackpotDamage, weaponKnockback, (int)scaleFactor * 2f, 0.2f);
                                SoundEngine.PlaySound(TheSevensStriker.CoinSound, Projectile.Center);
                            }
                            // After 7 waves have been shot, reset the gun and roll again
                            if (shottimer > 56)
                            {
                                soundtimer = 0;
                                rolling = true;
                                Projectile.ai[1] = 0;
                                shottimer = 0;
                            }
                        }
                        else
                        {
                            shottimer++;

                            if (shottimer == 1)
                            {
                                // The other three outcomes
                                switch (Projectile.ai[1])
                                {
                                    // A single brick with 100% damage
                                    case 1:
                                        Shoot(1, ModContent.ProjectileType<SevensStrikerBrick>(), weaponDamage, 0, 2f, 0);
                                        CombatText.NewText(player.getRect(), Color.Gray, CalamityUtils.GetTextValue("Misc.SevensBust"), true);
                                        SoundEngine.PlaySound(TheSevensStriker.BustSound, Projectile.Center);
                                        break;
                                    // 7 exploding oranges with 100% damage
                                    case 2:
                                        int doublesDamage = (int)(weaponDamage * TheSevensStriker.DoublesMultiplier);
                                        Shoot(7, ModContent.ProjectileType<SevensStrikerOrange>(), doublesDamage, weaponKnockback, 2f, 0.1f);
                                        CombatText.NewText(player.getRect(), Color.Orange, CalamityUtils.GetTextValue("Misc.SevensDoubles"), true);
                                        SoundEngine.PlaySound(TheSevensStriker.DoublesSound, Projectile.Center);
                                        break;
                                    // 7 piercing grapes with X% damage
                                    // Also fires 7 splitting cherries in a tighter spread with Y% damage
                                    case 3:
                                        int cherryDamage = (int)(weaponDamage * TheSevensStriker.TriplesCherryMultiplier);
                                        int grapeDamage = (int)(weaponDamage * TheSevensStriker.TriplesGrapeMultiplier);
                                        Shoot(7, ModContent.ProjectileType<SevensStrikerCherry>(), cherryDamage, weaponKnockback, 1.5f, 0.1f);
                                        Shoot(7, ModContent.ProjectileType<SevensStrikerGrape>(), grapeDamage, weaponKnockback, 2f, 0.2f);
                                        CombatText.NewText(player.getRect(), Color.Red, CalamityUtils.GetTextValue("Misc.SevensTriples"), true);
                                        SoundEngine.PlaySound(TheSevensStriker.TriplesSound, Projectile.Center);
                                        break;
                                }
                            }

                            // Reset the gun and roll again
                            if (shottimer > 16)
                            {
                                soundtimer = 0;
                                rolling = true;
                                Projectile.ai[1] = 0;
                                shottimer = 0;
                            }
                        }
                    }
                    // Update the roll sound position
                    if (SoundEngine.TryGetActiveSound(RouletteSoundSlot, out var rouletteSound) && rouletteSound.IsPlaying)
                        rouletteSound.Position = Projectile.Center;
                }
                // If the player can't use the gun, KILL it
                else
                {
                    Projectile.Kill();
                }
            }

            // Sounds
            // Crank & new casino
            if (Projectile.frameCounter == 0 && Projectile.frame == 2 * (Main.projFrames[Projectile.type] / 19))
            {
                SoundEngine.PlaySound(SoundID.Item108 with { Volume = SoundID.Item108.Volume * 0.9f }, Projectile.Center);
                RouletteSoundSlot = SoundEngine.PlaySound(TheSevensStriker.RouletteSound, Projectile.Center);
            }
            // Clicks for when each slot is finished
            if (soundtimer == 92 || soundtimer == 108 || soundtimer == 124)
            {
                SoundEngine.PlaySound(TheSevensStriker.RouletteTickSound, Projectile.Center);
            }

            // Holdout stuff
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        // Calculates which attack will occur based on coin.
        public int CalculateOutcome()
        {
            switch (Projectile.ai[0])
            {
                // Copper Coins have:
                // 50% chance for a brick
                // 30% chance for oranges
                // 15% chance for grapes and cherries
                // 5% chance for a jackpot
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
                // Silver Coins have:
                // 20% chance for a brick
                // 50% chance for oranges
                // 20% chance for grapes and cherries
                // 10% chance for a jackpot
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
                // Gold Coins have:
                // 5% chance for a brick
                // 30% chance for oranges
                // 50% chance for grapes and cherries
                // 15% chance for a jackpot
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
                // Platinum Coins are a guaranteed jackpot
                case ProjectileID.PlatinumCoin:
                    return 4;
            }
            // This should never be returned
            return 1;
        }

        // Where the shooting takes place (wow!)
        public void Shoot(int projcount, int type, int damage, float kb, float scaleFactor, float spreadfactor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 armPosition = player.RotatedRelativePoint(player.MountedCenter, true);
            armPosition += Projectile.velocity.SafeNormalize(player.direction * Vector2.UnitX) * 32f;
            armPosition.Y -= 20f;
            Vector2 shootDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(-Vector2.UnitY);
            Vector2 gunTip = armPosition + shootDirection * player.ActiveItem().scale * 90f;
            for (int i = 0; i < projcount; ++i)
            {
                Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-spreadfactor, spreadfactor, i / 7f)) * scaleFactor;
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), gunTip, perturbedSpeed, type, damage, kb, Main.player[Projectile.owner].whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = damage;

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player Owner = Main.player[Projectile.owner];
            Texture2D gun = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SevensStrikerHoldout").Value;

            SpriteEffects flip = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float drawAngle = Projectile.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0);
            Vector2 drawOrigin = new Vector2(Owner.direction < 0 ? gun.Width - 33f : 33f, 33f);
            Vector2 drawOffset = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() - Main.screenPosition;
            drawOffset.Y -= 10;
            int indframeheight = gun.Height / Main.projFrames[Projectile.type];
            int currentframe = indframeheight * Projectile.frame;
            Rectangle frame = new Rectangle(0, currentframe, gun.Width, indframeheight);

            Main.EntitySpriteDraw(gun, drawOffset, frame, lightColor, drawAngle, drawOrigin, Projectile.scale, flip, 0);

            return false;
        }

        // When the gun disappears, stop any in-progress slots sounds and set a cooldown of 12 frames.
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(RouletteSoundSlot, out var dringdring))
                dringdring.Stop();

            Main.player[Projectile.owner].SetDummyItemTime(12);
        }

        // This gun does not deal melee damage, thanks.
        public override bool? CanDamage() => false;
    }
}
