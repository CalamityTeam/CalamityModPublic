using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class EssenceScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 78;
            Projectile.aiStyle = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.alpha = 55;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            aiType = ProjectileID.DeathSickle;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.65f / 255f);

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 173, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 300f, 12f, 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(50f, 39f);
            Color color = Color.White;
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                color = new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, origin, 1f, spriteEffects, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 0, 200, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 2;
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
            if (target.life <= 0)
            {
                if (Projectile.owner == Main.myPlayer)
                    CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], 8, ModContent.ProjectileType<EssenceFlame>(), 1200f, 0f);
            }
        }
    }
}
