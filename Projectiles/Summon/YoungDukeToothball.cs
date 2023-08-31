using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YoungDukeToothball : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
        }

        public override void AI() => Projectile.rotation += Projectile.velocity.X * .04f;

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 3;  i++)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi).SafeNormalize(Vector2.One) * 20f, ModContent.ProjectileType<YoungDukeToothballSpike>(), (int)(Projectile.damage / 3), Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
        }
    }
}
