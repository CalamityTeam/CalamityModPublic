using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SludgeSplotchProj1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Splotch");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y >= 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 191, 0f, 0f, 225, new Color(255, 255, 255), 3);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = Main.dust[dust].velocity * 0.25f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    if (oldVelocity.X < 0)
                    {
                        Projectile.ai[0] = 1;
                    }
                    if (oldVelocity.X > 0)
                    {
                        Projectile.ai[0] = -1;
                    }
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    if (oldVelocity.Y < 0)
                    {
                        Projectile.ai[1] = 1;
                    }
                    if (oldVelocity.Y > 0)
                    {
                        Projectile.ai[1] = -1;
                    }
                }
            }

            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            SoundEngine.PlaySound(SoundID.NPCDeath92 with { Volume = SoundID.NPCDeath92.Volume * 0}, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slow, 120);
            SoundEngine.PlaySound(SoundID.NPCDeath9 with { Volume = SoundID.NPCDeath9.Volume * 2}, Projectile.position, 0);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slow, 120);
            SoundEngine.PlaySound(SoundID.NPCDeath9 with { Volume = SoundID.NPCDeath9.Volume * 2}, Projectile.position, 0);
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 6);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter - 2, sparkScatter + 2));

                    sparkVelocity.Normalize();
                    sparkVelocity *= 7;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, sparkVelocity, ModContent.ProjectileType<SludgeSplotchProj2>(), 7, 0, Projectile.owner, 0, 0);
                }
            }

            int numDust = 20;
            int dustType = 191;
            float spread = 3f;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 velocity = Projectile.velocity + new Vector2(Main.rand.NextFloat(-spread, spread), Main.rand.NextFloat(-spread, spread));

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, velocity.X, velocity.Y, 175, default, 3f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
