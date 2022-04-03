using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AndromedasStrideBoltSpawner : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Size => ref Projectile.ai[1]; //Yes

        public float WaitTimer; //How long until the monoliths appears
        public Vector2 OriginDirection; //The direction of the original strike
        public float Facing; //The direction of the original strike

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda Shock");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.hide = true;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
            {
                SurfaceUp();
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity = Vector2.Zero;
            }

            if (WaitTimer > 0)
            {
                Projectile.timeLeft = 30;
                WaitTimer--;
            }

            if (Projectile.timeLeft == 29)
            {
                if (Size * 0.8 > 0.4 && Facing != 0)
                    SideSprouts(Facing, 150f, Size * 0.8f);
            }

            if (Projectile.timeLeft < 29)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Vector2 particleDirection = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 2f, MathHelper.PiOver4 / 2f)) * Main.rand.NextFloat(15f, 35f);

                    Particle smoke = new HeavySmokeParticle(Projectile.Center, flyDirection, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.4f, 1.3f) * Projectile.scale, 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.Next(3) == 0)
                    {
                        Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, flyDirection, Color.Red, 20, Main.rand.NextFloat(0.1f, 0.7f) * Projectile.scale, 0.8f, 0, true, 0.01f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }

                }
            }

            if (Projectile.timeLeft == 2)
            {
                //New bolt
                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.position, Projectile.rotation.ToRotationVector2() * 18f, ProjectileType<GalaxiaBolt>(), Projectile.damage, 10f, Owner.whoAmI, 0.75f, MathHelper.Pi / 25f);
                    proj.scale = Size * 3f;
                    proj.timeLeft = 50;
                }

                Vector2 particleDirection = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, Projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 10f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(5f, 15f);

                    Particle smoke = new HeavySmokeParticle(Projectile.Center + hitPositionDisplace, flyDirection, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(1.6f, 2.2f) * Projectile.scale, 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.Next(3) == 0)
                    {
                        Particle smokeGlow = new HeavySmokeParticle(Projectile.Center + hitPositionDisplace, flyDirection, Color.Red, 20, Main.rand.NextFloat(1.4f, 1.7f) * Projectile.scale, 0.8f, 0, true, 0.005f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }
                }
            }
        }

        //Go up to the "surface" so you're not stuck in the middle of the ground like a complete moron.
        public void SurfaceUp()
        {
            for (float i = 0; i < 40; i += 0.5f)
            {
                Vector2 positionToCheck = Projectile.Center + Projectile.velocity * i;
                if (!Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolid())
                {
                    Projectile.Center = Projectile.Center + Projectile.velocity * i;
                    return;
                }
            }
            Projectile.Center = Projectile.Center + Projectile.velocity * 40f;
        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = Projectile.Center + OriginDirection.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

                if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolid())
                    widestAngle = i;

                else if (widestAngle != 0)
                {
                    validPositionFound = true;
                    widestSurfaceAngle = widestAngle;
                }
            }

            if (validPositionFound)
            {
                Vector2 projPosition = Projectile.Center + OriginDirection.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = OriginDirection.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<AndromedasStrideBoltSpawner>(), Projectile.damage, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is AndromedasStrideBoltSpawner spawner)
                {
                    spawner.WaitTimer = (float)Math.Sqrt(1.0 - Math.Pow(projSize - 1.0, 2)) * 3f;
                    spawner.OriginDirection = OriginDirection;
                    spawner.Facing = facing;
                }
            }

            return validPositionFound;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(WaitTimer);
            writer.Write(Facing);
            writer.WriteVector2(OriginDirection);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            WaitTimer = reader.ReadSingle();
            Facing = reader.ReadSingle();
            OriginDirection = reader.ReadVector2();
        }
    }
}
