using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class KylieBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Kylie";

        //This variable will be used for the stealth strike
        public float ReboundTime = 0f;
        public float timer = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kylie");
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {

            //Constant rotation
            Projectile.rotation += 0.2f;

            timer++;
            //Dust trail
            if (Main.rand.Next(15) == 0)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 7, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, 0f);
                Main.dust[d].position = Projectile.Center;
            }
            //Constant sound effects
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 15;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            //Slopes REEEEEEEEEEEE
            if (timer == 3f)
                Projectile.tileCollide = true;
            //Decide the range of the boomerang depending on stealth
            if (Projectile.Calamity().stealthStrike)
                ReboundTime = 27f;
            else
                ReboundTime = 55f;

            // ai[0] stores whether the boomerang is returning. If 0, it isn't. If 1, it is.
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= ReboundTime)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.tileCollide = false;
                float returnSpeed = Kylie.Speed * 1.5f;
                float acceleration = 3.2f;
                Player owner = Main.player[Projectile.owner];

                // Delete the boomerang if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    Projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }


                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //Start homing at player if you hit an enemy
            Projectile.ai[0] = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce off tiles and start homing on player if it hits a tile
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.ai[0] = 1;
            return false;

        }
    }
}
