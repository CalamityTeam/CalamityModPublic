using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicBunny : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
			if (Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = 3000;
				SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			}

			if (Projectile.velocity.X > 0f)
				Projectile.direction = 1;
			else if (Projectile.velocity.X < 0f)
				Projectile.direction = -1;

			Projectile.spriteDirection = Projectile.direction;
			Projectile.ai[0] += 1f;
			Projectile.rotation += Projectile.velocity.X * 0.05f + (float)Projectile.direction * 0.05f;
			if (Projectile.ai[0] >= 18f)
			{
				Projectile.velocity.Y += 0.28f;
				Projectile.velocity.X *= 0.99f;
			}

			if (Projectile.velocity.Y > 15.9f)
				Projectile.velocity.Y = 15.9f;

            //Fade in
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

			if (Math.Abs(Projectile.velocity.X) < 0.2f && Math.Abs(Projectile.velocity.Y) < 0.2f)
				Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (Projectile.velocity.X != oldVelocity.X)
				Projectile.velocity.X = -oldVelocity.X * 0.5f;

			if (Projectile.velocity.Y != oldVelocity.Y)
				Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
			int idx = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f)), 76);
			Gore gore = Main.gore[idx];
			gore.velocity -= Projectile.velocity * 0.5f;
			idx = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f)), 77);
			gore.velocity -= Projectile.velocity * 0.5f;
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			for (int i = 0; i < 20; i++)
			{
				int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
				Dust dust = Main.dust[index];
				dust.velocity *= 1.4f;
			}

			for (int i = 0; i < 10; i++)
			{
				int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2.5f);
				Dust dust = Main.dust[index];
				dust.noGravity = true;
				dust.velocity *= 5f;
				index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1.5f);
				dust.velocity *= 3f;
			}

			idx = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, default(Vector2), Main.rand.Next(61, 64));
			gore.velocity *= 0.4f;
			gore.velocity.X += 1f;
			gore.velocity.Y += 1f;
			idx = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, default(Vector2), Main.rand.Next(61, 64));
			gore.velocity *= 0.4f;
			gore.velocity.X -= 1f;
			gore.velocity.Y += 1f;
			idx = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, default(Vector2), Main.rand.Next(61, 64));
			gore.velocity *= 0.4f;
			gore.velocity.X += 1f;
			gore.velocity.Y -= 1f;
			idx = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, default(Vector2), Main.rand.Next(61, 64));
			gore.velocity *= 0.4f;
			gore.velocity.X -= 1f;
			gore.velocity.Y -= 1f;

			Projectile.ExpandHitboxBy(128);
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			Projectile.Damage();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White; // new Color(255, 239, 0, Projectile.alpha);
    }
}
