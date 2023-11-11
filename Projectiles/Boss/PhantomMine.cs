using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.World;

namespace CalamityMod.Projectiles.Boss
{
    public class PhantomMine : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, Projectile.velocity.Length() / Projectile.ai[0]);

            if (Projectile.velocity.Length() < Projectile.ai[0])
            {
                Projectile.velocity *= Projectile.ai[1];
                if (Projectile.velocity.Length() > Projectile.ai[0])
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Projectile.ai[0];
                }

                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                {
                    if (Projectile.velocity.Length() >= Projectile.ai[0])
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            int totalProjectiles = 8;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 vector = new Vector2(0f, -8f).RotatedBy(radians * i);
                                int type = Main.rand.NextBool() ? ModContent.ProjectileType<PhantomShot2>() : ModContent.ProjectileType<PhantomShot>();
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector, type, Projectile.damage, 0f, Main.myPlayer);
                            }
                        }

                        Projectile.Kill();
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color((byte)(200 * Projectile.Opacity), (byte)(200 * Projectile.Opacity), (byte)(200 * Projectile.Opacity), Projectile.alpha);
        }

        public override bool CanHitPlayer(Player target) => Projectile.velocity.Length() >= Projectile.ai[0];

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 12f, targetHitbox);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            for (int i = 0; i < 15; i++)
            {
                int phantomDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 180, 0f, 0f, 100, default, 1.2f);
                Main.dust[phantomDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[phantomDust].scale = 0.5f;
                    Main.dust[phantomDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 30; j++)
            {
                int phantomDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 180, 0f, 0f, 100, default, 1.7f);
                Main.dust[phantomDust2].noGravity = true;
                Main.dust[phantomDust2].velocity *= 5f;
                phantomDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1f);
                Main.dust[phantomDust2].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Projectile.velocity.Length() >= Projectile.ai[0])
            {
                target.AddBuff(ModContent.BuffType<Nightwither>(), 120);
                target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 180);
            }
        }
    }
}
