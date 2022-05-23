using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class YateveoBloomProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yateveo Bloom");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Vector2 vector62 = Main.player[Projectile.owner].MountedCenter - Projectile.Center;
            Projectile.rotation = vector62.ToRotation() - 1.57f;
            float distance = vector62.Length();

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

            if (Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = true;
                if (distance > 210f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                else if (!Main.player[Projectile.owner].channel)
                {
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y * 0.9f;

                    Projectile.velocity.Y = Projectile.velocity.Y + 1f;
                    Projectile.velocity.X = Projectile.velocity.X * 0.9f;
                }
            }
            else
            {
                float num211 = 14f / Main.player[Projectile.owner].GetAttackSpeed<MeleeDamageClass>() * 1.25f;
                float num212 = 0.9f / Main.player[Projectile.owner].GetAttackSpeed<MeleeDamageClass>() * 1.25f;

                if (Projectile.ai[1] == 1f)
                    Projectile.tileCollide = false;

                if (!Main.player[Projectile.owner].channel || distance > 375f || !Projectile.tileCollide)
                {
                    Projectile.ai[1] = 1f;

                    if (Projectile.tileCollide)
                        Projectile.netUpdate = true;

                    Projectile.tileCollide = false;

                    if (distance < 20f)
                        Projectile.Kill();
                }

                if (!Projectile.tileCollide)
                    num212 *= 2f;

                if (distance > 80f || !Projectile.tileCollide)
                {
                    distance = num211 / distance;
                    vector62.X *= distance;
                    vector62.Y *= distance;

                    Vector2 vector21 = new Vector2(Projectile.velocity.X, Projectile.velocity.Y);
                    float num217 = vector62.X - Projectile.velocity.X;
                    float num218 = vector62.Y - Projectile.velocity.Y;
                    float num219 = (float)Math.Sqrt((double)(num217 * num217 + num218 * num218));

                    num219 = num212 / num219;
                    num217 *= num219;
                    num218 *= num219;

                    Projectile.velocity.X = Projectile.velocity.X * 0.98f;
                    Projectile.velocity.Y = Projectile.velocity.Y * 0.98f;
                    Projectile.velocity.X = Projectile.velocity.X + num217;
                    Projectile.velocity.Y = Projectile.velocity.Y + num218;
                }
                else
                {
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 6f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X * 0.96f;
                        Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
                    }

                    if (Main.player[Projectile.owner].velocity.X == 0f)
                        Projectile.velocity.X = Projectile.velocity.X * 0.96f;
                }
            }

            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(5);
                switch (dustType)
                {
                    case 0:
                        dustType = 2;
                        break;
                    case 1:
                        dustType = 44;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        dustType = 136;
                        break;
                    default:
                        break;
                }
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType, 0f, 0f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Projectile.alpha == 255 && distance > 10f)
                Projectile.alpha = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool hitTile = false;
            if (oldVelocity.X != Projectile.velocity.X)
            {
                hitTile = Math.Abs(oldVelocity.X) > 4f;

                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.2f;
            }
            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                hitTile = Math.Abs(oldVelocity.Y) > 4f;

                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            Projectile.ai[0] = 1f;

            if (hitTile)
            {
                Projectile.netUpdate = true;
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/YateveoBloomChain").Value;
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
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 90);
        }
    }
}
