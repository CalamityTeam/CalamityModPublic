using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class CraniumSmasherStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/CraniumSmasherExplosive";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stealthy Cranium Smasher");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.tileCollide = false;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 5f)
            {
                Projectile.tileCollide = true;
            }
            Projectile.rotation += Projectile.velocity.X * 0.02f;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.085f;
            Projectile.velocity.X = Projectile.velocity.X * 0.99f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int smash = Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CraniumSMASH>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0f, 0f);
                Main.projectile[smash].Center = Projectile.Center;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int smash = Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CraniumSMASH>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner, 0f, 0f);
                Main.projectile[smash].Center = Projectile.Center;
            }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 192;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 14);
            CalamityUtils.ExplosionGores(Projectile.Center, 3);
            for (int num194 = 0; num194 < 25; num194++)
            {
                int num195 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, 0f, 0f, 100, default, 2f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 0f;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/CraniumSmasherGlow");
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
