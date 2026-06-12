using System;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{

    public class EvilSmasherProjectile : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Player Owner => Main.player[Projectile.owner];
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<EvilSmasher>()).Item;

        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 10;
        public override int OffsetDistance => 50;
        public override int CooldownTime { get; set; }
        public override bool AlternateSwings => false;
        public override bool useAttackSpeed => false;
        public override SoundStyle? UseSound => SoundID.DD2_MonkStaffSwing with {Volume = 1f};

        public ref float CurrentChargeMult => ref Projectile.ai[0];

        bool hasSmashedTile = false;
        bool playedChargeSound = false;
        bool firstEnemyHit = true;

        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
            swingWidth = 200;
            RotateInCooldown = 0;
            RotateInStartup = 0;
            Projectile.width = 64;
            Projectile.height = 66;
        }

        public override void Spawn()
        {
            angle = new Vector2(angle.X.DirectionalSign(), 0);
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 80;
            CooldownTime = 30;
            swingTime = 10;
            modplayer.swingNum = 0;
            Projectile.timeLeft = 600;
            Projectile.scale *= 1.25f;
        }

        public override void AdditionalAI()
        {
            if (inStartup)
            {
                CurrentChargeMult = timer / (float)(StartupTime-1);
                Owner.velocity.X *= 0.97f;
            }
            if (inStartup && !Owner.channel && timer > 30)
            {
                timer = StartupTime - 1;
            }
            if (Owner.channel && timer == StartupTime - 1)
            {
                Projectile.timeLeft++;
                timer--;
                if (!playedChargeSound)
                {
                    SoundEngine.PlaySound(SoundID.DeerclopsStep with { Volume = 2f, Pitch = 0.5f }, Projectile.Center);
                    SoundEngine.PlaySound(Murasama.InorganicHit with { Pitch = -0.5f }, Projectile.Center);
                    playedChargeSound = true;
                    for (int i = 0; i < 5; i++)
                    {
                        int sparkLifetime = Main.rand.Next(8, 12);
                        float sparkScale = Main.rand.NextFloat(0.5f, 1f);
                        var sparkColor = Main.rand.NextBool() ? Color.Purple : Color.Red;

                        if (Main.rand.NextBool(5))
                            sparkScale *= 1.4f;

                        Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                        SparkParticle spark = new SparkParticle(Projectile.Center + angle * 30, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            if (!hasSmashedTile && inSwing && SwingCompletion > 0.275f)
            {
                var adjustedAngle = angle.RotatedBy(Projectile.spriteDirection * SwingFunction());
                Vector2 HammerFrontPos = Projectile.Center + adjustedAngle * -16 * Projectile.scale + (adjustedAngle.RotatedBy(MathHelper.PiOver2) * 20 * Projectile.scale * angle.X);
                if (Collision.SolidCollision(HammerFrontPos, 1, 1))
                {

                    Owner.velocity *= 0.15f;
                    Owner.velocity -= adjustedAngle.RotatedBy(MathHelper.PiOver2) * angle.X * MathHelper.Lerp(7.5f,16f, CurrentChargeMult);
                    float ringRot = SwingCompletion < 0.5f ? 0 : MathHelper.PiOver2;
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(HammerFrontPos, Vector2.Zero, Color.Red, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloatDirection(), 0.03f, 0.04f * (2 + CurrentChargeMult), 15));
                    GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(HammerFrontPos, Vector2.Zero, Color.Purple, new Vector2(0.2f, 1f), ringRot, 0.2f, 0.75f * (2 + CurrentChargeMult), 30));
                    GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(HammerFrontPos, Vector2.Zero, Color.Purple, new Vector2(0.2f, 1f), ringRot, 0.1f, 0.5f * (2 + CurrentChargeMult), 30));
                    int radius = (int)(4 * CurrentChargeMult);
                    Point scanAreaStart = HammerFrontPos.ToTileCoordinates() + new Point(-radius, -radius);
                    Point scanAreaEnd = HammerFrontPos.ToTileCoordinates() + new Point(radius, radius);
                    Projectile.CreateImpactExplosion((int)(10 * CurrentChargeMult), Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);

                    hasSmashedTile = true;
                    timer = StartupTime + swingTime;
                    angle = adjustedAngle;
                    var pos = Projectile.Center;
                    Projectile.Size *= 2 + CurrentChargeMult;
                    Projectile.Center = HammerFrontPos;
                    Projectile.Damage();
                    Projectile.Size /= 2 + CurrentChargeMult;
                    Projectile.Center = pos;

                    if (CurrentChargeMult >= 1)
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with {VariantsWeights = new ReadOnlySpan<float>(new float[] { 1, 0, 0 })});
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact);
                }
            }
            Owner.heldProj = Projectile.whoAmI;
        }

        public override float SwingFunction()
        {
            if (hasSmashedTile)
                return MathHelper.ToRadians(MathHelper.Lerp(0, -swingWidth * 0.4f, MathF.Pow(CooldownCompletion, 0.5f)));
            if (inStartup)
                return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * 0.8f, -swingWidth * 0.66f, 1 - MathF.Pow(StartupCompletion, 0.5f)));
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.33f, swingWidth * 0.45f, MathF.Pow(CooldownCompletion, 0.5f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth * .66f, (swingWidth * 0.33f), SwingCompletion));
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= CurrentChargeMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (firstEnemyHit && Main.rand.NextFloat() < CurrentChargeMult)
            {
                string text = "";
                if (target.life > 0)
                    text = CalamityUtils.GetTextValue("Items.Weapons.Melee.EvilSmasher.DamageDialogue" + Main.rand.Next(4));
                else
                    text = CalamityUtils.GetTextValue("Items.Weapons.Melee.EvilSmasher.KillDialogue" + Main.rand.Next(2));
                char[] textToFormat = text.ToCharArray();
                for (var i = 0; i < textToFormat.Length; i++)
                {
                    if (Main.rand.NextBool())
                        textToFormat[i] = char.ToUpper(textToFormat[i]);
                }
                text = new string(textToFormat);
                bool intense = target.life <= 0;
                CombatText.NewText(Projectile.Hitbox, Color.Lerp(Color.DarkGray,Color.Gold, intense ? 0.25f: 0.1f), text, intense);
                firstEnemyHit = false;
            }
            if (!hasSmashedTile && CurrentChargeMult >= 1)
            {
                for (int i = 0; i < 16; i++)
                {
                    int sparkLifetime = Main.rand.Next(10, 15);
                    float sparkScale = Main.rand.NextFloat(1f, 2f);
                    var sparkColor = Main.rand.NextBool() ? Color.Purple : Color.Red;

                    if (Main.rand.NextBool(5))
                        sparkScale *= 1.4f;

                    Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * MathHelper.Lerp(10, 30, Main.rand.NextFloat());
                    SparkParticle spark = new SparkParticle(target.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);

                }
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack with { Volume = 0.5f, LimitsArePerVariant = false, MaxInstances = 1 });
            }
            else if (!hasSmashedTile)
            {

                SoundEngine.PlaySound(SoundID.Item69 with { Volume = 1f, LimitsArePerVariant = false, MaxInstances = 1 });
            }

            target.AddBuff(ModContent.BuffType<SmashedEvil>(), (int)MathHelper.Lerp(60, 900, CurrentChargeMult));
            Owner.AddBuff(ModContent.BuffType<SmashedEvil>(), (int)MathHelper.Lerp(60, 900, CurrentChargeMult));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!inCooldown)
            {
                var tex = ModContent.Request<Texture2D>(Texture + "Glow").Value;
                float outlineWidth = (int)(4 * CurrentChargeMult) * 0.5f;
                if (inSwing)
                {
                    outlineWidth *= 1 - SwingCompletion;
                }
                for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.25f)
                {
                    Main.spriteBatch.Draw(
                        tex,
                        Projectile.Center + new Vector2(0, Projectile.gfxOffY) + Vector2.UnitX.RotatedBy(i + Projectile.rotation) * outlineWidth * Projectile.scale - Main.screenPosition,
                        null,
                        Color.Lerp(Color.Purple,Color.Red,MathF.Sin(Main.GlobalTimeWrappedHourly * 6)* 0.5f + 0.5f),
                        Projectile.rotation,
                        tex.Size() * 0.5f,
                        Projectile.scale,
                        Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        0
                    );
                }
            }
            if (inSwing)
                return base.PreDraw(ref lightColor);
            return true;
        }
    }
}
