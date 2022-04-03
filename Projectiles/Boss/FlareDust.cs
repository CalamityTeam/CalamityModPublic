using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
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
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().canBreakPlayerDefense = true;
            Projectile.width = 64;
            Projectile.height = 66;
            Projectile.scale = 1.5f;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 680;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(start);
            writer.Write(startingPosX);
            writer.Write(startingPosY);
            writer.Write(distance);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            start = reader.ReadBoolean();
            startingPosX = reader.ReadSingle();
            startingPosY = reader.ReadSingle();
            distance = reader.ReadDouble();
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

            if (Projectile.ai[0] == 2f)
                return;

            if (start)
            {
                startingPosX = Projectile.Center.X;
                startingPosY = Projectile.Center.Y;
                start = false;
            }

            float amount = Projectile.localAI[0] / 120f;
            if (amount > 1f)
                amount = 1f;
            distance += MathHelper.Lerp(1f, 9f, amount);

            if (Projectile.timeLeft < 380)
            {
                float amount2 = (Projectile.localAI[0] - 300f) / 240f;
                if (amount2 > 1f)
                    amount2 = 1f;
                distance += MathHelper.Lerp(1f, 9f, amount2);
            }

            double rad = MathHelper.ToRadians(Projectile.ai[1]);
            if (Projectile.ai[0] == 0f)
            {
                Projectile.position.X = startingPosX - (int)(Math.Sin(rad) * distance) - Projectile.width / 2;
                Projectile.position.Y = startingPosY - (int)(Math.Cos(rad) * distance) - Projectile.height / 2;
            }
            else
            {
                Projectile.position.X = startingPosX - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
                Projectile.position.Y = startingPosY - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;
            }

            Projectile.ai[1] += (1.1f - amount) * 0.5f;
            Projectile.localAI[0] += 1f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 48);
            for (int d = 0; d < 2; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.NextFloat(0.1f, 1f);
                }
            }
            for (int d = 0; d < 4; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(Projectile.Center, 3);
            Projectile.Damage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 16f * Projectile.scale, targetHitbox);

        public override bool CanHitPlayer(Player target)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(Projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(Projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(Projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(Projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 16f * Projectile.scale;
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
