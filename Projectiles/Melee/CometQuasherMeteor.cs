using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class CometQuasherMeteor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 2;
            aiType = ProjectileID.Meteor1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            if (projectile.Calamity().lineColor == 1)
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Melee/CometQuasherMeteor2");
            if (projectile.Calamity().lineColor == 2)
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Melee/CometQuasherMeteor3");

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1, tex);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item89, projectile.position);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(128f * projectile.scale));
            for (int index = 0; index < 8; ++index)
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 32; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Dust dust1 = Main.dust[index2];
                dust1.noGravity = true;
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
                dust2.noGravity = true;
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Gore.NewGore(projectile.position + new Vector2((float) (projectile.width * Main.rand.Next(100)) / 100f, (float) (projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[index2];
                gore.velocity *= 0.3f;
                gore.velocity.X += (float) Main.rand.Next(-10, 11) * 0.05f;
                gore.velocity.Y += (float) Main.rand.Next(-10, 11) * 0.05f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.localAI[1] = -1f;
                projectile.maxPenetrate = 0;
                projectile.Damage();
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Utils.SelectRandom<int>(Main.rand, new int[3]{ DustID.Fire, 259, 158 }), 2.5f * (float) projectile.direction, -2.5f, 0, new Color(), 1f);
                Dust dust1 = Main.dust[index2];
                dust1.alpha = 200;
                dust1.velocity *= 2.4f;
                dust1.scale += Main.rand.NextFloat();
            }
        }
    }
}
