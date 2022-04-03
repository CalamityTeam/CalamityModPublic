using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class CometQuasherMeteor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            aiType = ProjectileID.Meteor1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.Calamity().lineColor == 1)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/CometQuasherMeteor2");
            if (Projectile.Calamity().lineColor == 2)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/CometQuasherMeteor3");

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, tex);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)(128f * Projectile.scale));
            for (int index = 0; index < 8; ++index)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 32; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Dust dust1 = Main.dust[index2];
                dust1.noGravity = true;
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                dust2.noGravity = true;
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Gore.NewGore(Projectile.position + new Vector2((float) (Projectile.width * Main.rand.Next(100)) / 100f, (float) (Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[index2];
                gore.velocity *= 0.3f;
                gore.velocity.X += (float) Main.rand.Next(-10, 11) * 0.05f;
                gore.velocity.Y += (float) Main.rand.Next(-10, 11) * 0.05f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom<int>(Main.rand, new int[3]{ DustID.Fire, 259, 158 }), 2.5f * (float) Projectile.direction, -2.5f, 0, new Color(), 1f);
                Dust dust1 = Main.dust[index2];
                dust1.alpha = 200;
                dust1.velocity *= 2.4f;
                dust1.scale += Main.rand.NextFloat();
            }
        }
    }
}
