using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class CometQuasherMeteor : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            AIType = ProjectileID.Meteor1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.Calamity().lineColor == 1)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/CometQuasherMeteor2").Value;
            if (Projectile.Calamity().lineColor == 2)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/CometQuasherMeteor3").Value;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, tex);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.ExpandHitboxBy((int)(128f * Projectile.scale));
            for (int i = 0; i < 8; ++i)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int j = 0; j < 32; ++j)
            {
                int fieryDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Dust dust1 = Main.dust[fieryDust];
                dust1.noGravity = true;
                dust1.velocity *= 3f;
                int fieryDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[fieryDust2];
                dust2.velocity *= 2f;
                dust2.noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int j = 0; j < 2; ++j)
                {
                    int fieryDust = Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                    Gore gore = Main.gore[fieryDust];
                    gore.velocity *= 0.3f;
                    gore.velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                    gore.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int j = 0; j < 5; ++j)
            {
                int fieryDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Utils.SelectRandom<int>(Main.rand, new int[3]{ 6, 259, 158 }), 2.5f * (float) Projectile.direction, -2.5f, 0, new Color(), 1f);
                Dust dust1 = Main.dust[fieryDust];
                dust1.alpha = 200;
                dust1.velocity *= 2.4f;
                dust1.scale += Main.rand.NextFloat();
            }
        }
    }
}
