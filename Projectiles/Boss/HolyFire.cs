using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Fire");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 1;
            Projectile.tileCollide = false;
            cooldownSlot = 1;
            Projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }
            Projectile.alpha -= (!Main.dayTime || CalamityWorld.malice || BossRushEvent.BossRushActive) ? 10 : 5;
            if (Projectile.alpha <= 0)
            {
                Projectile.Kill();
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return (Main.dayTime && !CalamityWorld.malice) ? new Color(250, 150, 0, Projectile.alpha) : new Color(100, 200, 250, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Main.dayTime && !CalamityWorld.malice) ? ModContent.Request<Texture2D>(Texture).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyFireNight").Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int randomShot = Main.rand.Next(2);
            if (randomShot == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0.01f, 0f, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -0.01f, 0f, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0.05f, 0f, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -0.05f, 0f, ModContent.ProjectileType<HolyFire2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            int dustType = (Main.dayTime && !CalamityWorld.malice) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = (Main.dayTime && !CalamityWorld.malice) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
