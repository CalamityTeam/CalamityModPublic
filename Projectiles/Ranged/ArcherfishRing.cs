using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Audio;


namespace CalamityMod.Projectiles.Ranged
{
    public class ArcherfishRing : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.alpha = 115;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            //The ring expanding slowing while increasing opacity until it disappears at max size 
            
            if (Projectile.ai[0] == 0)
            {
            Projectile.scale = 0.2f;
            Projectile.ai[0]++;
            }

            if (Projectile.scale <= 1.5f)
            {
                Projectile.scale *= 1.035f;
            }
            if (Projectile.velocity.X > -0.8f && Projectile.velocity.X < 0.8f &
                Projectile.velocity.Y > -0.8f && Projectile.velocity.Y < 0.8f)
            {
                Projectile.alpha += 10;
            }
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Audio feedback for hitting the ring
            Player user = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);

            target.AddBuff(BuffID.Wet, 240);
        }
    }
}
