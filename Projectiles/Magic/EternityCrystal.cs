using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityCrystal : ModProjectile
    {
        public bool Collapsing = false;
        public float TargetOffsetRadius = 480f;
        public float DegreesToSpin = 2f;
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float SpinAngle
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public const int InwardCollapseTime = 70;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = EternityHex.Lifetime;
            Projectile.alpha = 0;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Collapsing);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Collapsing = reader.ReadBoolean();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Delete the crystal if any necessary components are incorrect/would cause errors.
            if (Projectile.localAI[1] >= Main.projectile.Length || Projectile.localAI[0] < 0)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            Projectile book = Main.projectile[(int)Projectile.localAI[1]];

            if (!book.active)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            if (TargetIndex >= Main.npc.Length || TargetIndex < 0)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            NPC target = Main.npc[TargetIndex];

            if (!target.active)
            {
                DeathDust();
                Projectile.Kill();
                return;
            }

            Projectile.localAI[0] += 1f;
            if (Projectile.timeLeft == 1 && !Collapsing)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * 2f;
                Projectile.timeLeft = InwardCollapseTime;
                Collapsing = true;

                Projectile.netUpdate = true;
            }
            SpinAngle -= MathHelper.ToRadians(DegreesToSpin);
            Projectile.rotation = Projectile.AngleTo(target.Center) - MathHelper.PiOver2;
            Projectile.position = target.Center + SpinAngle.ToRotationVector2() * TargetOffsetRadius;
            if (!Collapsing)
            {
                Projectile.damage = 0;
            }
            else
            {
                DegreesToSpin *= 1.0425f;
                TargetOffsetRadius *= 0.95f;

                if (Projectile.Hitbox.Intersects(target.Hitbox) && !target.dontTakeDamage)
                    ExplosionEffect(target, player);
                if (Projectile.alpha < 255)
                    Projectile.alpha += 3;
            }
        }
        public void DeathDust()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Eternity.DustID, newColor: new Color(245, 112, 218));
                dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(2f, 6f);
                dust.noGravity = true;
            }
        }
        public void ExplosionEffect(NPC target, Player player)
        {
            // Apply damage to the target and register it to the owner's DPS meter.
            int damage = (int)(Eternity.ExplosionDamage * player.MagicDamage() * Main.rand.NextFloat(0.9f, 1.1f));
            player.addDPS(damage);
            target.StrikeNPC(damage, 0f, 0, false);

            Vector2 randomCirclePointVector = Vector2.UnitY.RotatedBy(Projectile.rotation);

            // pointsPerStarStrip is basically how many times dust should be drawn to make half of a star point.
            // The amount of dust from the explosion = pointsPerStarStrip * starPoints * 2.
            int pointsPerStarStrip = 20;
            int starPoints = 9;

            float minStarOutwardness = Main.rand.NextFloat(12f, 28f);
            float maxStarOutwardness = Main.rand.NextFloat(48f, 68f);
            for (float i = 0; i < starPoints; i++)
            {
                for (int rotationDirection = -1; rotationDirection <= 1; rotationDirection += 2)
                {
                    Vector2 randomCirclePointRotated = randomCirclePointVector.RotatedBy(rotationDirection * MathHelper.TwoPi / (starPoints * 2));
                    for (float k = 0f; k < pointsPerStarStrip; k++)
                    {
                        Vector2 randomCirclePointLerped = Vector2.Lerp(randomCirclePointVector, randomCirclePointRotated, k / pointsPerStarStrip);
                        float outwardness = MathHelper.Lerp(minStarOutwardness, maxStarOutwardness, k / pointsPerStarStrip) * 2f;
                        Dust dust = Dust.NewDustDirect(new Vector2(target.Center.X, target.Center.Y), 0, 0, Main.rand.Next(132, 133 + 1), 0f, 0f, 100, default, 1.3f);
                        dust.velocity *= 0.1f;
                        dust.velocity += randomCirclePointLerped * outwardness;
                        dust.noGravity = true;
                        dust.color = Utils.SelectRandom(Main.rand, new Color(61, 141, 235), new Color(229, 52, 220));
                    }
                }

                randomCirclePointVector = randomCirclePointVector.RotatedBy(MathHelper.TwoPi / starPoints);
            }
            SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), (int)target.Center.X, (int)target.Center.Y);
            Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D myTexture = ModContent.Request<Texture2D>(Texture);
            Rectangle frame = myTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Color trasparentCrystalColor = Projectile.GetAlpha(lightColor) * 0.6f;
            Vector2 origin = frame.Size() / 2f;

            // Determine the offset factor of the crystals via a universal time-based sinusoid incorporated into a linear interpolation.
            float outwardness = MathHelper.Lerp(2f, 5f, (float)Math.Cos(Main.GlobalTime) * 0.5f + 0.5f);

            for (float i = 0f; i < 5; i++)
            {
                float angle = MathHelper.TwoPi / 5f * i + MathHelper.PiOver2;
                Vector2 offset = Vector2.UnitY.RotatedBy(angle).RotatedBy(Projectile.rotation);
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + offset * outwardness + Vector2.UnitY * Projectile.gfxOffY;
                spriteBatch.Draw(myTexture, drawPosition, frame, trasparentCrystalColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
