using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailPurple : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation += Math.Abs(Projectile.velocity.X) * 0.04f * (float)Projectile.direction;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 90f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);

            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 704, 1f);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 705, 1f);
            }

            int numShrapnel = 4;
            int shrapnelDamage = Projectile.damage / 3;
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < numShrapnel; i++)
                {
                    Vector2 randProjSpawn = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    randProjSpawn.Normalize();
                    randProjSpawn *= (float)Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, randProjSpawn.X, randProjSpawn.Y, ModContent.ProjectileType<MadAlchemistsCocktailShrapnel>(), shrapnelDamage, 0f, Projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
                }
            }
        }
    }
}
