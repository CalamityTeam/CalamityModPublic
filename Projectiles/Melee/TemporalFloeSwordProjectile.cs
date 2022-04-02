using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class TemporalFloeSwordProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Floe");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.35f / 255f);
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, 0f, 0f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 595)
                return false;

            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 50, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);

            // Get Terraria's current strange time variable
            double time = Main.time;

            // Correct for night time (which for some reason isn't just a different number) by adding 54000.
            if (!Main.dayTime)
                time += 54000D;

            // Divide by seconds in an hour
            time /= 3600D;

            // Terraria night starts at 7:30 PM, so offset accordingly
            time -= 19.5;

            // Offset time to ensure it is not negative
            if (time < 0D)
                time += 24D;

            // Get the decimal (smaller than hours, so minutes) component of time.
            int intTime = (int)time;
            double deltaTime = time - intTime;

            // Convert decimal time into an exact number of minutes.
            deltaTime = (int)(deltaTime * 60D);

            // Convert from 24 to 12 hour time
            if (intTime > 12)
                intTime -= 12;

            float projSpeed = 8f;

            float hour = (float)intTime;
            float hourAngle = MathHelper.PiOver2 - (MathHelper.TwoPi / 12f) * hour;
            hourAngle *= -1f; //correction because lol
            Vector2 hourVector = hourAngle.ToRotationVector2() * projSpeed;

            float minute = (float)deltaTime;
            float minuteAngle = MathHelper.PiOver2 - (MathHelper.TwoPi / 60f) * minute;
            minuteAngle *= -1f; //correction because pain
            Vector2 minuteVector = minuteAngle.ToRotationVector2() * projSpeed;

            int projType = ModContent.ProjectileType<TemporalFloeNumberTwo>();
            int dmg = projectile.damage / 2;
            float kback = projectile.knockBack * 0.5f;

            Projectile.NewProjectile(projectile.Center, hourVector, projType, dmg, kback, projectile.owner, 0f, 0f);
            Projectile.NewProjectile(projectile.Center, minuteVector, projType, dmg, kback, projectile.owner, 0f, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
