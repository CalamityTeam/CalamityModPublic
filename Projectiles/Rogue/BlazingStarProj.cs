using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using System;

namespace CalamityMod.Projectiles.Rogue
{
    public class BlazingStarProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BlazingStar";

        public static int Lifetime = 1540;
        public static int ReboundTime = 60;
        public static float Speed = 13f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
                Projectile.penetrate = Projectile.maxPenetrate = -1;

            // Boomerang rotation
            Projectile.rotation += 0.4f * Projectile.direction;

            // Boomerang sound
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            // Returns after some number of frames in the air
            if (Projectile.timeLeft < Lifetime - ReboundTime)
                Projectile.ai[0] = 1f;

            if (Projectile.ai[0] != 0f)
            {
                Projectile.tileCollide = false;
                float returnSpeed = 32.5f;
                float acceleration = 2f;
                Player owner = Main.player[Projectile.owner];

                // Delete the projectile if it's excessively far away.
                // Relying on the length instead of a second square root call for Normalize is used here for general performance since both the distance
                // and normalized vector are used in this code.
                float distanceToPlayer = Projectile.Distance(owner.Center);
                Vector2 velocity = (owner.Center - Projectile.Center) / distanceToPlayer * returnSpeed;

                // Kill the projectile is if it's super far from the player.
                if (distanceToPlayer > 3000f)
                    Projectile.Kill();

                // Home back in on the player.

                // No, projectiles do not have a SimpleFlyMovement extension method like NPCs do.
                if (Projectile.velocity.X < velocity.X)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && velocity.X > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > velocity.X)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && velocity.X < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < velocity.Y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && velocity.Y > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > velocity.Y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && velocity.Y < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        // Make the star bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Impacts the terrain even though it bounces off.
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.ai[0] = 1f;
            return false;
        }
    }
}
