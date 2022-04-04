using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 27;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.35f / 255f);
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, 0f, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 50, 255, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

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
            int dmg = Projectile.damage / 2;
            float kback = Projectile.knockBack * 0.5f;

            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, hourVector, projType, dmg, kback, Projectile.owner, 0f, 0f);
            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, minuteVector, projType, dmg, kback, Projectile.owner, 0f, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
