using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityCrystal : ModProjectile
    {
        public bool Collapsing = false;
        public float TargetOffsetRadius = 480f;
        public float DegreesToSpin = 2f;
        public int TargetIndex
        {
            get => (int)projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float SpinAngle
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public const int InwardCollapseTime = 70;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 36;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = EternityHex.Lifetime;
            projectile.alpha = 0;
            projectile.magic = true;
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
            Player player = Main.player[projectile.owner];

            // Delete the crystal if any necessary components are incorrect/would cause errors.
            if (projectile.localAI[1] >= Main.projectile.Length || projectile.localAI[0] < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            Projectile book = Main.projectile[(int)projectile.localAI[1]];

            if (!book.active)
            {
                DeathDust();
                projectile.Kill();
                return;
            }
            
            if (TargetIndex >= Main.npc.Length || TargetIndex < 0)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            NPC target = Main.npc[TargetIndex];

            if (!target.active)
            {
                DeathDust();
                projectile.Kill();
                return;
            }

            projectile.localAI[0] += 1f;
            if (projectile.timeLeft == 1 && !Collapsing)
            {
                projectile.velocity = projectile.SafeDirectionTo(target.Center) * 2f;
                projectile.timeLeft = InwardCollapseTime;
                Collapsing = true;

                projectile.netUpdate = true;
            }
            SpinAngle -= MathHelper.ToRadians(DegreesToSpin);
            projectile.rotation = projectile.AngleTo(target.Center) - MathHelper.PiOver2;
            projectile.position = target.Center + SpinAngle.ToRotationVector2() * TargetOffsetRadius;
            if (!Collapsing)
            {
                projectile.damage = 0;
            }
            else
            {
                DegreesToSpin *= 1.0425f;
                TargetOffsetRadius *= 0.95f;

                if (projectile.Hitbox.Intersects(target.Hitbox) && !target.dontTakeDamage)
                    ExplosionEffect(target, player);
                if (projectile.alpha < 255)
                    projectile.alpha += 3;
            }
        }
        public void DeathDust()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, Eternity.DustID, newColor: new Color(245, 112, 218));
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

            Vector2 randomCirclePointVector = Vector2.UnitY.RotatedBy(projectile.rotation);

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
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), (int)target.Center.X, (int)target.Center.Y);
            projectile.Kill();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D myTexture = ModContent.GetTexture(Texture);
            Rectangle frame = myTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Color trasparentCrystalColor = projectile.GetAlpha(lightColor) * 0.6f;
            Vector2 origin = frame.Size() / 2f;

            // Determine the offset factor of the crystals via a universal time-based sinusoid incorporated into a linear interpolation.
            float outwardness = MathHelper.Lerp(2f, 5f, (float)Math.Cos(Main.GlobalTime) * 0.5f + 0.5f);

            for (float i = 0f; i < 5; i++)
            {
                float angle = MathHelper.TwoPi / 5f * i + MathHelper.PiOver2;
                Vector2 offset = Vector2.UnitY.RotatedBy(angle).RotatedBy(projectile.rotation);
                Vector2 drawPosition = projectile.Center - Main.screenPosition + offset * outwardness + Vector2.UnitY * projectile.gfxOffY;
                spriteBatch.Draw(myTexture, drawPosition, frame, trasparentCrystalColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}