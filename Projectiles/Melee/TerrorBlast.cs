using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class TerrorBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;

            // This projectile can only hit when it explodes, so these values aren't a problem.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        // Terror Blasts do nothing until they explode.
        public override bool? CanDamage() => Projectile.localAI[0] > 0f;

        public override void OnKill(int timeLeft)
        {
            // Explode on death, becoming an enormous hitbox and spawning a ton of dust.
            SoundEngine.PlaySound(SoundID.Item60, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 400;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);

            for (int i = 0; i < 6; i++)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);

            for (int i = 0; i < 60; i++)
            {
                int scaryDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[scaryDust].noGravity = true;
                Main.dust[scaryDust].velocity *= 3f;
                scaryDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[scaryDust].velocity *= 2f;
                Main.dust[scaryDust].noGravity = true;
            }

            // Guarantee a hit on all nearby enemies when the projectile explodes. Changing localAI[0] enables it to hit.
            Projectile.localAI[0] = 1f;
            Projectile.Damage();
        }
    }
}
