using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class Nebudust : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0f);
            float projTimer = 25f;
            if (Projectile.ai[0] > 60f)
            {
                projTimer -= (Projectile.ai[0] - 60f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;
            if (Projectile.ai[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item105, Projectile.position);
            }
            Projectile.ai[0] += 4f;
            int timerCounter = 0;
            while ((float)timerCounter < projTimer)
            {
                float rando1 = (float)Main.rand.Next(-6, 7);
                float rando2 = (float)Main.rand.Next(-6, 7);
                float rando5 = (float)Main.rand.Next(2, 5);
                float randoAdjuster = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                randoAdjuster = rando5 / randoAdjuster;
                rando1 *= randoAdjuster;
                rando2 *= randoAdjuster;
                int astra = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 269, 0f, 0f, 100, default, 1f);
                Main.dust[astra].noGravity = true;
                Main.dust[astra].position.X = Projectile.Center.X;
                Main.dust[astra].position.Y = Projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[astra];
                expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[astra];
                expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
                Main.dust[astra].velocity.X = rando1;
                Main.dust[astra].velocity.Y = rando2;
                timerCounter++;
            }
        }
    }
}
