using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Whiterain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item125 with { Volume = 0.7f }, Projectile.Center);
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);

            for (int i = 0; i < 2; i++)
            {
                int shiny = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, 0f, 100, default, 1.25f);
                Main.dust[shiny].noGravity = true;
                Main.dust[shiny].velocity *= 0.5f;
                Main.dust[shiny].velocity += Projectile.velocity * 0.1f;
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 320f, 14f, 20f);
        }
    }
}
