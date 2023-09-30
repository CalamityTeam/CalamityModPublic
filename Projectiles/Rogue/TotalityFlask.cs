using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class TotalityFlask : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TotalityBreakers";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.MolotovCocktail;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 20 == 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TotalityTar>(), (int)(Projectile.damage * 0.6), Projectile.knockBack, Projectile.owner, 0f, 0f);
                    }
                }
            }
            Vector2 spinningpoint = new Vector2(4f, -8f);
            float rotation = Projectile.rotation;
            if (Projectile.direction == -1)
                spinningpoint.X = -4f;
            Vector2 vector2 = spinningpoint.RotatedBy((double)rotation, new Vector2());
            for (int index1 = 0; index1 < 1; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.Center + vector2 - Vector2.One * 5f, 4, 4, 6, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].scale = 1.5f;
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity = Main.dust[index2].velocity * 0.25f + Vector2.Normalize(vector2) * 1f;
                Main.dust[index2].velocity = Main.dust[index2].velocity.RotatedBy(-MathHelper.PiOver2 * (double)Projectile.direction, new Vector2());
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            //glass-pot break sound
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);

            int meltdown = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TotalMeltdown>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            Main.projectile[meltdown].Center = Projectile.Center; //makes it centered because it's not without this

            Vector2 vector2 = new Vector2(20f, 20f);
            for (int d = 0; d < 5; ++d)
                Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 191, 0.0f, 0.0f, 0, Color.Red, 1f);
            for (int d = 0; d < 10; ++d)
            {
                int index2 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 1.4f;
            }
            for (int d = 0; d < 20; ++d)
            {
                int index2 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 6, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Dust dust1 = Main.dust[index2];
                dust1.noGravity = true;
                dust1.velocity *= 5f;
                int index3 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 6, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 3f;
            }
            int tarAmt = Main.rand.Next(2, 4);
            for (int t = 0; t < tarAmt; t++)
            {
                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TotalityTar>(), (int)(Projectile.damage * 0.3), 0f, Main.myPlayer, 0f, 0f);
            }
        }
    }
}
