using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class Flarenado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flarenado");
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().canBreakPlayerDefense = true;
            Projectile.width = 320;
            Projectile.height = 88;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            CooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            float scaleBase = 24f;
            float scaleMult = 1f;
            float baseWidth = 320f;
            float baseHeight = 88f;

            if (Projectile.velocity.X != 0f)
            {
                Projectile.direction = Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.scale = (scaleBase - Projectile.ai[1]) * scaleMult / scaleBase;
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)(baseWidth * Projectile.scale), (int)(baseHeight * Projectile.scale));
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] != -1f)
            {
                Projectile.scale = (scaleBase - Projectile.ai[1]) * scaleMult / scaleBase;
                Projectile.width = (int)(baseWidth * Projectile.scale);
                Projectile.height = (int)(baseHeight * Projectile.scale);
            }

            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 60)
                {
                    Projectile.alpha = 60;
                }
            }
            else
            {
                Projectile.alpha += 30;
                if (Projectile.alpha > 150)
                {
                    Projectile.alpha = 150;
                }
            }
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
            }

            if (Projectile.ai[0] == 1f && Projectile.ai[1] > 0f && Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
                Vector2 center = Projectile.Center;
                center.Y -= baseHeight * Projectile.scale / 2f;
                float num618 = (scaleBase - Projectile.ai[1] + 1f) * scaleMult / scaleBase;
                center.Y -= baseHeight * num618 / 2f;
                center.Y += 2f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 11f, Projectile.ai[1] - 1f);
            }
            if (Projectile.ai[0] <= 0f)
            {
                float num622 = 0.104719758f;
                float num623 = (float)Projectile.width / 5f;
                num623 *= 2f;
                float num624 = (float)(Math.Cos((double)(num622 * -(double)Projectile.ai[0])) - 0.5) * num623;
                Projectile.position.X -= num624 * -Projectile.direction;
                Projectile.ai[0] -= 1f;
                num624 = (float)(Math.Cos((double)(num622 * -(double)Projectile.ai[0])) - 0.5) * num623;
                Projectile.position.X += num624 * -Projectile.direction;
            }

            if (Projectile.timeLeft == 480)
                Projectile.damage = Projectile.GetProjectileDamage(ModContent.NPCType<Yharon>());
        }

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft <= 480;

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 53, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = texture.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.timeLeft <= 480)
                target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 300);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = Projectile;
        }
    }
}
