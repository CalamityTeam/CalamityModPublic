using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class IncineratingFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];
        public bool Released = false;
        public bool TriggeredBurnOut = false;
        public const float StartScale = 0.0004f;
        public const float EndScale = 10.25f;

        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Timer++;
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * (Projectile.scale * 0.5f));

            // This CalamityPlayer variable causes the weapon to be unusable while it's greater than 0.
            // Yes, it's getting set every frame as soon as the fireball starts despawning. That's intentional.
            // It's the delay for after the fireball despawns, and the burnout visual effects should appear immediately.
            if (TriggeredBurnOut)
                Owner.Calamity().burningSeaBurnOut = BurningSea.BurnOutReuseDelay;

            bool canUseMana = Owner.CheckMana(Owner.HeldItem);
            // Use different behavior depending on if the player is channeling or not.
            if (Owner.CantUseHoldout() || !canUseMana || Projectile.timeLeft <= (int)BurningSea.FizzleOutTime)
            {
                Released = true;

                // Time spent fizzling out scales with how long the fireball was charged up.
                if (Projectile.timeLeft > BurningSea.FizzleOutTime)
                    Projectile.timeLeft = (int)MathHelper.Clamp(MathHelper.Lerp(0f, BurningSea.FizzleOutTime, Timer / BurningSea.ChargeTime), 0f, BurningSea.FizzleOutTime);

                // Lose velocity sharply.
                Projectile.velocity *= 0.965f;

                // Fizzle out. Once it's small enough, it dies.
                Projectile.scale = Utils.Remap(Projectile.timeLeft, 0f, BurningSea.FizzleOutTime, StartScale, EndScale);
                Projectile.ExpandHitboxBy((int)(Projectile.scale * 50f));
            }
            else if (!Released)
            {
                // Follow the player's position.
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 projLocation = Owner.Center;
                    Vector2 mouse = Owner.ClampedMouseWorld();
                    float mouseDist = Vector2.Distance(mouse, projLocation);
                    Vector2 mouseDiff = mouse - projLocation;
                    if (mouseDist > 128f)
                    {
                        mouseDiff.Normalize();
                        mouseDiff *= 128f;
                    }
                    projLocation += mouseDiff;

                    Vector2 orbAttemptedVelocity = Vector2.Zero.MoveTowards(projLocation - Projectile.Center, 25f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, orbAttemptedVelocity, 0.08f);

                    Projectile.netUpdate = true;
                }

                // Slowly increase in size as the fireball is charged up.
                Projectile.scale = Utils.Remap(Timer, 0f, BurningSea.ChargeTime, StartScale, EndScale);
                Projectile.ExpandHitboxBy((int)(Projectile.scale * 50f));

                // Consume mana periodically.
                if (Timer % 15 == 0f)
                    Owner.CheckMana(Owner.HeldItem, -1, true);

                // Spawn little spark effects around the fireball when fully charged.
                if (Timer > BurningSea.ChargeTime && Timer < BurningSea.BurnOutTime)
                {
                    for (int s = 0; s < 6; s++)
                    {
                        float sparkRotation = Main.GlobalTimeWrappedHourly * -5.75f + (MathHelper.TwoPi / 6f * s);
                        Vector2 sparkLocation = Projectile.Center + Vector2.UnitX.RotatedBy(sparkRotation) * 220f;
                        Vector2 sparkVelocity = Vector2.Normalize(sparkLocation - Projectile.Center).RotatedBy(MathHelper.ToRadians(70)) * 2f;
                        AltLineParticle spark = new(sparkLocation, sparkVelocity, false, 8, 0.8f, Color.Lerp(Color.Red, Color.Orange, Main.rand.NextFloat(0.3f)));
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                // If channeled for a while, turns red (handled in PreDraw) and starts emitting smoke as a warning.
                if (Timer > BurningSea.BurnOutTime - 90f)
                {
                    for (int s = 0; s < 3; s++)
                    {
                        Vector2 smokeLocation = Projectile.Center + Main.rand.NextVector2Circular(220f, 220f);
                        Vector2 smokeVel = -Vector2.UnitY * Main.rand.NextFloat(7f, 13f);
                        HeavySmokeParticle burningUp = new(smokeLocation, smokeVel, new Color(192, 192, 192), 10, 0.7f, 0.6f);
                        GeneralParticleHandler.SpawnParticle(burningUp);
                    }
                }
                // Holding it to this point triggers its burn-out mechanic. This has several effects:
                // Spawn text, play a sound, immediately cause the fizzle out, inflict Brimstone Flames on the player, and set a long delay to the weapon.
                if (Timer > BurningSea.BurnOutTime)
                {
                    Projectile.timeLeft = (int)BurningSea.FizzleOutTime;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/WeaponEnchant"), Owner.Center);
                    CombatText.NewText(Owner.Hitbox, new Color(192, 0, 0), CalamityUtils.GetTextValue("Misc.BurningSeaBurn"), true);

                    Owner.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
                    TriggeredBurnOut = true;
                }
            }

            AdjustPlayerValues();
        }

        public void AdjustPlayerValues()
        {
            Projectile.spriteDirection = Projectile.direction = Owner.direction;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        // Can only deal damage while not fizzling out.
        public override bool? CanDamage() => !Released && Timer % Projectile.localNPCHitCooldown == 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle Burn = new SoundStyle("CalamityMod/Sounds/Item/WeldingBurn") with { Volume = 0.25f };
            SoundEngine.PlaySound(Burn, target.Center);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);

            if (damageDone > 2 && !target.Calamity().IsArmored())
                Projectile.damage = (int)(Projectile.damage * 0.7f);

            // Whoa buddy no smoking allowed
            for (int i = 0; i < 4; i++)
            {
                Vector2 smokeVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2.5f, 5f);
                HeavySmokeParticle smoke = new(target.Center, smokeVel, Color.Gray, 36, 0.7f, 0.625f, 0f, true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        // We don't want this massive thing obstructing nearby projectiles.
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            Texture2D TheodoreJNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoise").Value;
            Vector2 drawScale = Projectile.Size / TheodoreJNoise.Size() * 1.25f;
            float rotation = Main.GlobalTimeWrappedHourly * 4.2f;

            GameShaders.Misc["CalamityMod:ExoVortex"].UseOpacity(0.6f);
            GameShaders.Misc["CalamityMod:ExoVortex"].Apply();

            // Big fucking fireball
            for (int i = 0; i < 6; i++)
            {
                float direction = (i % 2 == 0).ToDirectionInt();
                float offsetDist = 0f;
                Color fireballColor = Color.Lerp(new Color(255, 200, 200), new Color(255, 30, 30), MathHelper.Clamp(Timer / BurningSea.ChargeTime, 0f, 1f));
                if (Timer > BurningSea.BurnOutTime - 120f)
                {
                    fireballColor = Color.Lerp(new Color(255, 30, 30), Color.Red, MathHelper.Clamp((Timer - BurningSea.BurnOutTime + 120f) / 60f, 0f, 1f));
                    offsetDist = MathHelper.Lerp(5f, 40f, (Timer - BurningSea.BurnOutTime + 120f) / 120f);
                }

                Main.spriteBatch.Draw(TheodoreJNoise, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(offsetDist, offsetDist), null, fireballColor, direction * rotation, TheodoreJNoise.Size() / 2f, drawScale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
