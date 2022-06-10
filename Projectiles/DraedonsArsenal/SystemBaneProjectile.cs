using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class SystemBaneProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/SystemBane";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public const int LightningFireRate = 60;
        public const int FieldLightningFireRate = 45;
        public const float FieldRadius = 360f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("System Bane");
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 480;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.StickToTiles(false, true);

            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Y < 15f)
            {
                Projectile.velocity.Y += 0.5f;
            }
            // Generate idle sparks.
            if (Time % 15f == 0f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 229);
                dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
                dust.fadeIn = 1.05f;
                dust.noGravity = true;
            }
            // Every so often, generate some lightning at a nearby enemy, if one exists.
            if (Time % LightningFireRate == 0f && Main.myPlayer == Projectile.owner)
            {
                int lightningDamage = Projectile.damage;
                int totalSystemBanes = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type];

                // Make the damage of the lightning have diminishing returns depending on how many systems are present.
                lightningDamage = (int)Math.Ceiling(lightningDamage / Math.Pow(totalSystemBanes, 1D / 3D));

                NPC potentialTarget = Projectile.Center.ClosestNPCAt(900f);
                if (potentialTarget != null)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(potentialTarget.Center) * 15f, ModContent.ProjectileType<SystemBaneLightning>(), lightningDamage, Projectile.knockBack, Projectile.owner);
            }

            // Sometimes generate lightning from the outside of the energy field if the projectile was spawned by a stealth strike.
            if (Projectile.Calamity().stealthStrike)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(FieldRadius);
                if (Time % FieldLightningFireRate == 0f && potentialTarget != null && Main.myPlayer == Projectile.owner)
                {
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(FieldRadius, FieldRadius);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, potentialTarget.DirectionFrom(spawnPosition) * 14f, ModContent.ProjectileType<SystemBaneLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            if (!Projectile.Calamity().stealthStrike)
                return;
            int totalCirclePoints = 55;
            float generalOpacity = Utils.GetLerpValue(0f, 30f, Projectile.timeLeft, true) * Utils.GetLerpValue(480f, 450f, Projectile.timeLeft, true);
            Texture2D lightningTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/LightningProj").Value;
            for (int i = 0; i < totalCirclePoints; i++)
            {
                float angle = MathHelper.TwoPi * i / totalCirclePoints;
                float nextAngle = angle + MathHelper.TwoPi / totalCirclePoints;
                float radiusOffset = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 65f);
                Vector2 start = Projectile.Center + angle.ToRotationVector2() * (FieldRadius + radiusOffset) - Main.screenPosition;
                Vector2 end = Projectile.Center + nextAngle.ToRotationVector2() * (FieldRadius + radiusOffset) - Main.screenPosition;

                DelegateMethods.f_1 = SystemBaneLightning.InnerLightningOpacity * generalOpacity;
                DelegateMethods.c_1 = SystemBaneLightning.InnerLightningColor;
                Utils.DrawLaser(Main.spriteBatch, lightningTexture, start, end, new Vector2(SystemBaneLightning.InnerLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));

                DelegateMethods.f_1 = SystemBaneLightning.OuterLightningOpacity * generalOpacity;
                DelegateMethods.c_1 = SystemBaneLightning.OuterLightningColor;
                Utils.DrawLaser(Main.spriteBatch, lightningTexture, start, end, new Vector2(SystemBaneLightning.OuterLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X *= 0.8f;
            return false;
        }
    }
}

