using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class FlareDust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/FlareBomb";

        private bool start = true;
        private float startingPosX = 0f;
        private float startingPosY = 0f;
        private double distance = 0D;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Dust");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 64;
            projectile.height = 66;
            projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 680;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(start);
            writer.Write(startingPosX);
            writer.Write(startingPosY);
            writer.Write(distance);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            start = reader.ReadBoolean();
            startingPosX = reader.ReadSingle();
            startingPosY = reader.ReadSingle();
            distance = reader.ReadDouble();
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);

            if (projectile.ai[0] == 2f)
                return;

            if (start)
            {
                startingPosX = projectile.Center.X;
                startingPosY = projectile.Center.Y;
                start = false;
            }

            float amount = projectile.localAI[0] / 120f;
            if (amount > 1f)
                amount = 1f;
            distance += MathHelper.Lerp(1f, 9f, amount);

            if (projectile.timeLeft < 380)
            {
                float amount2 = (projectile.localAI[0] - 300f) / 240f;
                if (amount2 > 1f)
                    amount2 = 1f;
                distance += MathHelper.Lerp(1f, 9f, amount2);
            }

            double rad = MathHelper.ToRadians(projectile.ai[1]);
            if (projectile.ai[0] == 0f)
            {
                projectile.position.X = startingPosX - (int)(Math.Sin(rad) * distance) - projectile.width / 2;
                projectile.position.Y = startingPosY - (int)(Math.Cos(rad) * distance) - projectile.height / 2;
            }
            else
            {
                projectile.position.X = startingPosX - (int)(Math.Cos(rad) * distance) - projectile.width / 2;
                projectile.position.Y = startingPosY - (int)(Math.Sin(rad) * distance) - projectile.height / 2;
            }

            projectile.ai[1] += (1.1f - amount) * 0.5f;
            projectile.localAI[0] += 1f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 48);
            for (int d = 0; d < 2; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.NextFloat(0.1f, 1f);
                }
            }
            for (int d = 0; d < 4; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
            projectile.Damage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 16f * projectile.scale, targetHitbox);

        public override bool CanHitPlayer(Player target)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 16f * projectile.scale;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
