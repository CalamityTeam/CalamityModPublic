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
    public class SeaDragonMuzzleBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 8;
            Projectile.MaxUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            // This projectile is already point blank. It cannot be boosted by point blank.
        }

        public override void AI()
        {
            // Belching smoke and flames copied from Handheld Tank (lasts 8 frames, but all happens at once due to extra updates)
            for (int i = 0; i < 30; ++i)
            {
                // Pick a random type of smoke (there's a little fire mixed in)
                int dustID;
                switch (Main.rand.Next(6))
                {
                    case 0:
                        dustID = 55;
                        break;
                    case 1:
                    case 2:
                        dustID = 54;
                        break;
                    default:
                        dustID = 53;
                        break;
                }

                // Choose a random speed and angle to belch out the smoke
                float dustSpeed = Main.rand.NextFloat(3.0f, 13.0f);
                float angleRandom = 0.06f;
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                // Pick a size for the smoke particle
                float scale = Main.rand.NextFloat(0.5f, 1.6f);

                // Actually spawn the smoke
                int idx = Dust.NewDust(Projectile.Center, 1, 1, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].position = Projectile.Center;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 240);
        }
    }
}
