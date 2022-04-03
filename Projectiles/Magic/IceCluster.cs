using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class IceCluster : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.5f;

            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[1] == 1f)
            {
                if (Projectile.ai[0] % 30f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vector80 = Projectile.rotation.ToRotationVector2();
                    Projectile.NewProjectile(Projectile.Center, vector80, ModContent.ProjectileType<IceCluster>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                Lighting.AddLight(Projectile.Center, 0.3f, 0.75f, 0.9f);
            }

            if (Projectile.ai[0] >= 90f)
                Projectile.alpha += 25;
            else
                Projectile.alpha -= 15;

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;

            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

            Vector2 vector80 = Projectile.rotation.ToRotationVector2();
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.Center, vector80, ModContent.ProjectileType<IceCluster>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
