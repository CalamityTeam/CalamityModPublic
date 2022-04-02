using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseDragonProjectile : ModProjectile
    {
        public bool ReelingBack
        {
            get => projectile.timeLeft <= ReelbackTime;
            set
            {
                if (value)
                    projectile.timeLeft = ReelbackTime;
            }
        }

        public Player Owner => Main.player[projectile.owner];
        public float Outwardness => OutwardnessMax * (float)Math.Sin(projectile.timeLeft / (float)Lifetime * MathHelper.Pi);
        public ref float InitialRotation => ref projectile.ai[0];
        public ref float OutwardnessMax => ref projectile.ai[1];
        public ref float SwingDirection => ref projectile.localAI[0];
        public const int ChargeTime = 25;
        public const int ReelbackTime = 25;
        public const int Lifetime = ChargeTime + ReelbackTime;
        public const float MaximumPossibleOutwardness = 72f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Dragon");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.timeLeft = Lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SwingDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingDirection = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.rotation = projectile.AngleTo(Owner.Center) - MathHelper.PiOver2;
            if (Owner.dead)
            {
                projectile.Kill();
                return;
            }

            projectile.direction = (Owner.Center.X - projectile.Center.X > 0).ToDirectionInt();
            projectile.spriteDirection = projectile.direction;

            ManipulateOwnerFields();

            if (projectile.timeLeft >= Lifetime - ChargeTime)
            {
                float time = Utils.InverseLerp(Lifetime, Lifetime - ChargeTime, projectile.timeLeft, true);
                float offsetAngle = MathHelper.Lerp(-1.1f, 1.5f, time); // Set the range of the offset to -1.8 and 1.8 based off of the time.
                offsetAngle *= projectile.localAI[0]; // Incorporate direction into the offset angle.

                projectile.velocity = InitialRotation.ToRotationVector2().RotatedBy(offsetAngle) * 29f;

                // Adjust the velocity to go along with the player's if the velocity directions are similar so that the player's movement doesn't hinder the projectile's movement.
                if (Vector2.Dot(projectile.velocity.SafeNormalize(Vector2.Zero), Owner.velocity.SafeNormalize(Vector2.Zero)) > 0.45f)
                {
                    projectile.velocity += Owner.velocity;
                }
            }
            else
            {
                projectile.velocity = projectile.SafeDirectionTo(Owner.Center, Vector2.UnitX * Owner.direction) * 43f;
                projectile.timeLeft = ReelbackTime;
                if (projectile.WithinRange(Owner.Center, 45f))
                    projectile.Kill();
            }

            GenerateIdleDust();
            if (projectile.timeLeft % 3 == 2 && projectile.Distance(Owner.Center) > 40f)
                SpawnElectricFields();
        }

        public void ManipulateOwnerFields()
        {
            Owner.itemAnimation = 4;
            Owner.itemTime = 4;
            Owner.ChangeDir(-projectile.direction);
            Owner.itemRotation = projectile.rotation + (projectile.spriteDirection == -1).ToInt() * MathHelper.TwoPi;
        }

        public void GenerateIdleDust()
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(52f, 52f), 261);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
            }
        }

        public void SpawnElectricFields()
        {
            if (Main.myPlayer != projectile.owner)
                return;
            Projectile field = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ProjectileID.Electrosphere, projectile.damage, projectile.knockBack, projectile.owner);
            if (field.whoAmI.WithinBounds(Main.maxProjectiles))
            {
                field.Calamity().forceMelee = true;
                field.usesLocalNPCImmunity = true;
                field.localNPCHitCooldown = 3;
                field.timeLeft = 12;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            Texture2D chainTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/PulseDragonChain");
            Texture2D pulseTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PulseAura");
            Texture2D dragonHeadTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PulseDragonProjectile");
            if (ReelingBack)
                dragonHeadTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PulseDragonHeadClosed");
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;

            // Chain drawing.
            List<Vector2> bezierPoints = new List<Vector2>()
            {
                mountedCenter
            };
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = Vector2.UnitX * -SwingDirection;
                offset *= Outwardness * (float)Math.Sin(i / 20f * MathHelper.Pi);
                offset *= Utils.InverseLerp(0f, 300f, Owner.Distance(Vector2.Lerp(mountedCenter, projectile.Center, i / 20f) + offset), true);
                bezierPoints.Add(Vector2.Lerp(mountedCenter, projectile.Center, i / 20f) + offset);
            }
            bezierPoints.Add(projectile.Center);

            BezierCurve bezierCurve = new BezierCurve(bezierPoints.ToArray());
            int totalChains = (int)(projectile.Distance(mountedCenter) / chainTexture.Height);
            totalChains = (int)MathHelper.Clamp(totalChains, 40f, 1000f);
            for (int i = 0; i < totalChains - 1; i++)
            {
                Vector2 drawPosition = bezierCurve.Evaluate(i / (float)totalChains);
                float angle = (bezierCurve.Evaluate(i / (float)totalChains + 1f / totalChains) - drawPosition).ToRotation();
                angle -= MathHelper.PiOver2;
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, lightColor, angle, chainTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            }

            // Aura drawing.
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = (i / 5f * MathHelper.TwoPi).ToRotationVector2() * 24f;
                float time = (float)Math.Sin(Main.GlobalTime * 1.8f);
                float angle = time * MathHelper.Pi + Main.GlobalTime * 2.1f;
                float scale = 1.1f + time * 0.2f;
                spriteBatch.Draw(pulseTexture, projectile.Center + offset - Main.screenPosition, null, Color.Cyan * 0.3f, angle, pulseTexture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }

            // Head drawing.
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(dragonHeadTexture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, dragonHeadTexture.Size() * 0.5f, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
