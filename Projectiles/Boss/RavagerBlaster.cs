using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class RavagerBlaster : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/Ravager/RavagerHead";
        public static readonly SoundStyle SANS = new("CalamityMod/Sounds/Custom/Ravager/GasterBlaster");
        public Vector2 storedVelocity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager Blaster");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            //ai0 = timer, ai1 = laser size
            if (Projectile.ai[0] < 90f) //Before the laser
            {
                Projectile.ai[0]++;
                if (storedVelocity == Vector2.Zero)
                {
                    storedVelocity = Projectile.SafeDirectionTo(Projectile.velocity);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                    Projectile.rotation = (float)Math.Atan2(storedVelocity.Y, storedVelocity.X) - MathHelper.PiOver2;
                    
                    SoundEngine.PlaySound(SANS, Projectile.Center); //Funny Gaster Blaster sounds
                }
                else if (Projectile.ai[0] >= 55f)
                {
                    Projectile.ai[0] = 90f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, storedVelocity, ModContent.ProjectileType<RavagerBlast>(), Projectile.damage, 0f, Projectile.owner, Projectile.ai[1], Projectile.whoAmI);
                }
            }
            else //Move out and despawn
            {
                //Start moving
                if (Projectile.velocity == Vector2.Zero)
                    Projectile.velocity = storedVelocity * -1f;
                else
                    Projectile.velocity *= 1.01f;

                //Fade out and die
                Projectile.alpha += 3;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }

        // Does no contact damage
        public override bool CanHitPlayer(Player target) => false;
    }
}
