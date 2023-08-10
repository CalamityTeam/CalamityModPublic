using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShockGrenadeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShockGrenade";

        public static int spriteWidth = 14;
        public static int spriteHeight = 30;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y >= 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Projectile.rotation += Projectile.direction * 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);

            Projectile.localAI[0] = 0;
            Projectile.localAI[1] = 0;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X < 0)
                {
                    Projectile.localAI[0] = 1;
                }
                if (oldVelocity.X > 0)
                {
                    Projectile.localAI[0] = -1;
                }
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y < 0)
                {
                    Projectile.localAI[1] = 1;
                }
                if (oldVelocity.Y > 0)
                {
                    Projectile.localAI[1] = -1;
                }
            }
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(spriteWidth / 2, spriteHeight / 2);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/ShockGrenadeGlow").Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Volume = SoundID.Item94.Volume * 0.75f }, Projectile.position);

            int boltCount = Main.rand.Next(5, 11);
            for (int i = 0; i < boltCount; i++)
            {
                int boltScatter = 1;
                Vector2 boltVelocity = new Vector2(Main.rand.NextFloat(-boltScatter, boltScatter), Main.rand.NextFloat(-boltScatter * 2, boltScatter * 2));
                if (Projectile.localAI[0] != 0)
                {
                    boltVelocity.X *= -1;
                }
                if (Projectile.localAI[1] != 0)
                {
                    boltVelocity.Y *= -1;
                }
                boltVelocity.X += Projectile.localAI[0];
                boltVelocity.Y += Projectile.localAI[1] * 2;
                boltVelocity.Normalize();
                boltVelocity *= 10;

                int boltType = Main.rand.Next(0, 2);
                int boltDamage = Projectile.damage / 2;

                int boltAI = 0;
                if (Projectile.Calamity().stealthStrike)
                {
                    boltAI = 1;
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, boltVelocity, ModContent.ProjectileType<ShockGrenadeBolt>(), boltDamage, 0, Projectile.owner, boltType, boltAI);
            }

            if (Projectile.Calamity().stealthStrike)
            {
                int auraDamage = Projectile.damage / 4;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockTeslaAura>(), auraDamage, 1, Projectile.owner, 0, 0);

                SoundEngine.PlaySound(SoundID.Item93 with { Volume = SoundID.Item93.Volume * 0.5f }, Projectile.position);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockGrenadeExplosion>(), Projectile.damage, 3, Projectile.owner, 0, 0);
        }
    }
}
