using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class MoltenBlob : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Blob");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 12;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.scale = 1.5f;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.95f;

            if (projectile.wet || projectile.lavaWet)
            {
                projectile.velocity.Y = 0f;
            }
            else
            {
                projectile.velocity.Y += 0.1f;
                if (projectile.velocity.Y > 5f)
                    projectile.velocity.Y = 5f;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 1)
                projectile.frame = 0;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override Color? GetAlpha(Color lightColor)
        {
            return (Main.dayTime && !CalamityWorld.malice) ? new Color(250, 150, 0, projectile.alpha) : new Color(100, 200, 250, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = (Main.dayTime && !CalamityWorld.malice) ? Main.projectileTexture[projectile.type] : ModContent.GetTexture("CalamityMod/Projectiles/Boss/MoltenBlobNight");
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item20, projectile.Center);
            projectile.position.X = projectile.position.X + (projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (projectile.height / 2);
            projectile.width = 10;
            projectile.height = 10;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            int dustType = (Main.dayTime && !CalamityWorld.malice) ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 5; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int buffType = (Main.dayTime && !CalamityWorld.malice) ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
            target.AddBuff(buffType, 90);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
