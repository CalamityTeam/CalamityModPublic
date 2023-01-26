using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Turret
{
    public class OnyxShotBuffer : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Shot");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 1;
        }


        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item36 with { Volume = 0.65f }, Projectile.Center);

                Projectile.localAI[0] = 1f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = Main.player[Main.myPlayer].GetSource_FromThis();
                for (int i = -1; i < 2; i++)
                    Projectile.NewProjectile(source, Projectile.Center + new Vector2(3f, 0f), Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.035f, 0.11f) * i), ModContent.ProjectileType<OnyxShot>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.Kill();
            }
        }

        public override bool? CanDamage() => false;
    }
}
