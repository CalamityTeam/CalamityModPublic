using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
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
            projectile.width = 58;
            projectile.height = 74;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.ai[1] += 1f;
            if (projectile.ai[1] == 1f)
            {
                finalDamage = projectile.damage * 4;
            }
            if (projectile.ai[1] >= 5f && (projectile.ai[1] <= 10f))
            {
                for (int num105 = 0; num105 < 10; num105++)
                {
                    float num99 = projectile.velocity.X / 3f * (float)num105;
                    float num100 = projectile.velocity.Y / 3f * (float)num105;
                    int num101 = 4;
                    int waterDust = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.5f);
                    Dust waterdust = Main.dust[waterDust];
                    waterdust.noGravity = true;
                    waterdust.velocity *= 0.1f;
                    waterdust.velocity += projectile.velocity * 0.1f;
                    waterdust.position.X -= num99;
                    waterdust.position.Y -= num100;
                }
            }
            if (projectile.ai[1] == 5f)
            {
                projectile.tileCollide = true;
            }
            Vector2 vector62 = Main.player[projectile.owner].Center - projectile.Center;
            projectile.rotation = vector62.ToRotation() - 1.57f;
            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }
            Main.player[projectile.owner].itemAnimation = 10;
            Main.player[projectile.owner].itemTime = 10;
            if (vector62.X < 0f)
            {
                Main.player[projectile.owner].ChangeDir(1);
                projectile.direction = 1;
            }
            else
            {
                Main.player[projectile.owner].ChangeDir(-1);
                projectile.direction = -1;
            }
            Main.player[projectile.owner].itemRotation = (vector62 * -1f * (float)projectile.direction).ToRotation();
            projectile.spriteDirection = (vector62.X > 0f) ? -1 : 1;
            if (projectile.ai[1] >= 45f && (projectile.ai[0] != 1f || projectile.ai[0] != 2f))
            {
                projectile.velocity.Y += 1f;
                projectile.velocity.X *= 0.995f;
                projectile.damage = finalDamage;
            }
            if (projectile.ai[0] == 0f && vector62.Length() > 1000f)
            {
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[0] == 1f || projectile.ai[0] == 2f)
            {
                float num693 = vector62.Length();
                if (num693 > 1500f)
                {
                    projectile.Kill();
                    return;
                }
                if (num693 > 600f)
                {
                    projectile.ai[0] = 2f;
                }
                projectile.tileCollide = false;
                float num694 = 20f;
                if (projectile.ai[0] == 2f)
                {
                    num694 = 40f;
                }
                projectile.velocity = Vector2.Normalize(vector62) * num694;
                if (vector62.Length() < num694)
                {
                    projectile.Kill();
                    return;
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 5f)
            {
                projectile.alpha = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[1] >= 5f)
            {
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ClamImpact"), (int)projectile.position.X, (int)projectile.position.Y);
                for (int num105 = 0; num105 < 50; num105++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int waterDust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width / 2, projectile.height / 2, 33, velocity.X, velocity.Y, 0, new Color(0, 142, 255), 1.5f);
                    Main.dust[waterDust].velocity *= 2f;
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D texture2D2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/ClamCrusherChain");
            Vector2 vector17 = projectile.Center;
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
                    Main.spriteBatch.Draw(texture2D2, vector17 - Main.screenPosition, sourceRectangle, color17, rotation15, origin, 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[1] >= 45f && (projectile.ai[0] != 1f || projectile.ai[0] != 2f))
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
            else
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 60);

            projectile.ai[0] = 1f;
            projectile.netUpdate = true;
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ClamImpact"), (int)projectile.position.X, (int)projectile.position.Y);
        }
    }
}
