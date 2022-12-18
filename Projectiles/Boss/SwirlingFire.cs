using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SwirlingFire : ModProjectile
    {
        public ref float AngularTurnSpeed => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            //TODO -- Later replace this new bool with Main.zenithWorld
            bool getFucked = Main.getGoodWorld && Main.masterMode;

            Vector2 fireVelocity = (Time / 6f).ToRotationVector2() * Main.rand.NextFloat(1.7f, 2.2f);
            Dust fire = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(3f, 3f), getFucked || !Main.dayTime ? 267 : 6);
            fire.velocity = fireVelocity.RotatedBy(MathHelper.PiOver2);
            fire.scale = Main.rand.NextFloat(1.3f, 1.45f);
            fire.noGravity = true;
            if (getFucked)
                fire.color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
            else if (!Main.dayTime)
                fire.color = Color.Lerp(Color.Cyan, Color.BlueViolet, Main.rand.NextFloat());

            Dust.CloneDust(fire).velocity = fireVelocity.RotatedBy(-MathHelper.PiOver2);

            Projectile.velocity = Projectile.velocity.RotatedBy(AngularTurnSpeed);
            Time++;
        }
    }
}
