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
            // Belching smoke and flames copied from Handheld Tank with a few edits (lasts 8 frames, but all happens at once due to extra updates)
            for (int i = 0; i < 30; ++i)
            {
                // Pick a random type of fire (there's a little water mixed in)
                int dustID;
                switch (Main.rand.Next(6))
                {
                    case 0:
                        dustID = 170;
                        break;
                    case 1:
                    case 2:
                        dustID = 174;
                        break;
                    default:
                        dustID = 33;
                        break;
                }

                // Choose a random speed and angle to belch out the fire
                float dustSpeed = Main.rand.NextFloat(3.0f, 13.0f);
                float angleRandom = 0.07f;
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                // Pick a size for the fire particle
                float scale = Main.rand.NextFloat(0.7f, 1.8f);

                // Actually spawn the fire
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
