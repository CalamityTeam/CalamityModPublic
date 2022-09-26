using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using CalamityMod.Particles;
using System;

namespace CalamityMod.Projectiles.Ranged
{
    public class PumplerGrenade : ModProjectile
    {
        public ref float State => ref Projectile.ai[0];
        public static int MaxTime => 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squash Shell");
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.timeLeft = MaxTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PumplerGrenade";

        private void Explode(bool NPCHit = false)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            int size = NPCHit ? 50 : 70; //Bigger explosion if no npc is hit
            Projectile.scale = (size / (float)Projectile.height) * Projectile.scale;

            Projectile.position -= Vector2.One * (size - Projectile.width) * 0.5f;
            Projectile.height = Projectile.width = size;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 6;
            Projectile.hide = true;
            Projectile.velocity = Vector2.Zero;
            State = 1;
        }

        public override void AI()
        {
            //Behave like a grenade
            if (State == 0)
            {
                if (Projectile.timeLeft == 1)
                {
                    Explode();
                    return;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Point tileCoords = Projectile.Bottom.ToTileCoordinates();
                if (Main.tile[tileCoords.X, tileCoords.Y + 1].HasUnactuatedTile &&
                    WorldGen.SolidTile(Main.tile[tileCoords.X, tileCoords.Y + 1]) && Projectile.timeLeft < 165)
                {
                    Explode();
                    return;
                }

                else if (Projectile.timeLeft < MaxTime - 8)
                {
                    Projectile.velocity.Y += 0.4f;
                    if (Projectile.velocity.Y > 16f)
                        Projectile.velocity.Y = 16f;
                }
            }

            else
            {
                if (Projectile.timeLeft == 5)
                    SmokeBoom();
            }
        }

        private void SmokeBoom()
        {
            for (int i = 0; i < 15; i++)
            {
                Particle smoke = new SmallSmokeParticle(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Vector2.Zero, Color.Orange, new Color(40, 40, 40), Main.rand.NextFloat(0.8f, 1.6f), 145 - Main.rand.Next(30));
                smoke.Velocity = (smoke.Position - Projectile.Center) * 0.2f + Projectile.velocity;
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= -1f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (State == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Explode(true);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (State == 0)
                return null;

            float blastRadius = Projectile.height / 2f;

            //Get the absolute value of the distance between the target's hitbox center & the explosion's center
            float distanceX = Math.Abs(Projectile.Center.X - target.Hitbox.Center.X);
            float distanceY = Math.Abs(Projectile.Center.Y - target.Hitbox.Center.Y);

            //If the distance is just too big for the two to intersect, return false
            if (distanceX > (target.Hitbox.Width / 2f + blastRadius) || distanceY > (target.Hitbox.Height / 2f + blastRadius))
                return false;

            //If either distance is too small (aka if the projectile's center is inside of the target's hitbox, return true
            if (distanceX <= (target.Hitbox.Width / 2f) || distanceY <= (target.Hitbox.Height / 2f))
                return null;

            //Pythagorean theorem stuff to determine the litteral edge cases
            float squaredCornerDisance = (float)(Math.Pow(distanceX - target.Hitbox.Width / 2f, 2) + Math.Pow(distanceY - target.Hitbox.Height / 2f, 2));
            if (squaredCornerDisance <= Math.Pow(blastRadius, 2))
                return null;

            return false;
        }
    }
}
