using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.SunkenSea;

namespace CalamityMod.Projectiles.Melee
{
    public class ClamCrusherFlail : ModProjectile
    {
        public int finalDamage;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clam Crusher");
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] == 1f)
            {
                finalDamage = Projectile.damage * 4;
            }
            if (Projectile.ai[1] >= 5f && (Projectile.ai[1] <= 10f))
            {
                for (int num105 = 0; num105 < 10; num105++)
                {
                    float num99 = Projectile.velocity.X / 3f * (float)num105;
                    float num100 = Projectile.velocity.Y / 3f * (float)num105;
                    int num101 = 4;
                    int waterDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)num101, Projectile.position.Y + (float)num101), Projectile.width - num101 * 2, Projectile.height - num101 * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.5f);
                    Dust waterdust = Main.dust[waterDust];
                    waterdust.noGravity = true;
                    waterdust.velocity *= 0.1f;
                    waterdust.velocity += Projectile.velocity * 0.1f;
                    waterdust.position.X -= num99;
                    waterdust.position.Y -= num100;
                }
            }
            if (Projectile.ai[1] == 5f)
            {
                Projectile.tileCollide = true;
            }
            Vector2 vector62 = Main.player[Projectile.owner].Center - Projectile.Center;
            Projectile.rotation = vector62.ToRotation() - 1.57f;
            if (Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Main.player[Projectile.owner].itemAnimation = 10;
            Main.player[Projectile.owner].itemTime = 10;
            if (vector62.X < 0f)
            {
                Main.player[Projectile.owner].ChangeDir(1);
                Projectile.direction = 1;
            }
            else
            {
                Main.player[Projectile.owner].ChangeDir(-1);
                Projectile.direction = -1;
            }
            Main.player[Projectile.owner].itemRotation = (vector62 * -1f * (float)Projectile.direction).ToRotation();
            Projectile.spriteDirection = (vector62.X > 0f) ? -1 : 1;
            if (Projectile.ai[1] >= 45f && (Projectile.ai[0] != 1f || Projectile.ai[0] != 2f))
            {
                Projectile.velocity.Y += 1f;
                Projectile.velocity.X *= 0.995f;
                Projectile.damage = finalDamage;
            }
            if (Projectile.ai[0] == 0f && vector62.Length() > 1000f)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 1f || Projectile.ai[0] == 2f)
            {
                float num693 = vector62.Length();
                if (num693 > 1500f)
                {
                    Projectile.Kill();
                    return;
                }
                if (num693 > 600f)
                {
                    Projectile.ai[0] = 2f;
                }
                Projectile.tileCollide = false;
                float num694 = 20f;
                if (Projectile.ai[0] == 2f)
                {
                    num694 = 40f;
                }
                Projectile.velocity = Vector2.Normalize(vector62) * num694;
                if (vector62.Length() < num694)
                {
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 5f)
            {
                Projectile.alpha = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] >= 5f)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(GiantClam.SlamSound, Projectile.position);
                for (int num105 = 0; num105 < 50; num105++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int waterDust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width / 2, Projectile.height / 2, 33, velocity.X, velocity.Y, 0, new Color(0, 142, 255), 1.5f);
                    Main.dust[waterDust].velocity *= 2f;
                }
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/ClamCrusherChain").Value;
            Vector2 vector17 = Projectile.Center;
            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2((float)texture2D2.Width * 0.5f, (float)texture2D2.Height * 0.5f);
            float num91 = (float)texture2D2.Height;
            Vector2 vector18 = mountedCenter - vector17;
            float rotation15 = (float)Math.Atan2((double)vector18.Y, (double)vector18.X) - 1.57f;
            bool flag13 = true;
            if (float.IsNaN(vector17.X) && float.IsNaN(vector17.Y))
            {
                flag13 = false;
            }
            if (float.IsNaN(vector18.X) && float.IsNaN(vector18.Y))
            {
                flag13 = false;
            }
            while (flag13)
            {
                if (vector18.Length() < num91 + 1f)
                {
                    flag13 = false;
                }
                else
                {
                    Vector2 value2 = vector18;
                    value2.Normalize();
                    vector17 += value2 * num91;
                    vector18 = mountedCenter - vector17;
                    Color color17 = Lighting.GetColor((int)vector17.X / 16, (int)(vector17.Y / 16f));
                    Main.spriteBatch.Draw(texture2D2, vector17 - Main.screenPosition, sourceRectangle, color17, rotation15, origin, 1f, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[1] >= 45f && (Projectile.ai[0] != 1f || Projectile.ai[0] != 2f))
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
            else
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 60);

            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(GiantClam.SlamSound, Projectile.position);
        }
    }
}
