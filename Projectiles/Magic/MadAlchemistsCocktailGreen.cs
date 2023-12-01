using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailGreen : ModProjectile, ILocalizedModType
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
            SoundEngine.PlaySound(SoundID.Item88, Projectile.position);

            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 704, 1f);
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity * 0.2f, 705, 1f);
            }

            for (int i = 0; i < 3; i++)
            {
                float x = Projectile.position.X + (float)Main.rand.Next(-100, 100);
                float y = Projectile.position.Y - (float)Main.rand.Next(500, 600);
                Vector2 vector = new Vector2(x, y);
                float projSpawnX = Projectile.position.X + (float)(Projectile.width / 2) - vector.X;
                float projSpawnY = Projectile.position.Y + (float)(Projectile.height / 2) - vector.Y;
                projSpawnX += (float)Main.rand.Next(-100, 101);
                float projSpawnDist = (float)Math.Sqrt((double)(projSpawnX * projSpawnX + projSpawnY * projSpawnY));
                projSpawnDist = 25f / projSpawnDist;
                projSpawnX *= projSpawnDist;
                projSpawnY *= projSpawnDist;

                float flareAI = projSpawnY + Projectile.position.Y;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, projSpawnX, projSpawnY, ProjectileID.LunarFlare, Projectile.damage / 2, 5f, Projectile.owner, 0f, flareAI);
            }
        }
    }
}
