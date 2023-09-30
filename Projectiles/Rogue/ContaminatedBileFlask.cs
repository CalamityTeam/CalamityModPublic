using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ContaminatedBileFlask : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ContaminatedBile";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.alpha = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.ai[0]++ > 45f)
            {
                if (Projectile.velocity.Y < 10f)
                    Projectile.velocity.Y += 0.15f;
            }

            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length());
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.Bottom);

            Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BileExplosion>(), (int)(Projectile.damage * 0.75), Projectile.knockBack, Projectile.owner);
            if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
            {
                explosion.Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
                explosion.timeLeft = explosion.Calamity().stealthStrike ? 60 : 20;
            }
        }
    }
}
