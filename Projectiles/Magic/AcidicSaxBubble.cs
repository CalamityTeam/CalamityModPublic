using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Magic;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class AcidicSaxBubble : ModProjectile
    {
        public float counter = 0f;
        public float counter2 = 0f;
        public int killCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (counter >= 120f)
                {
                    counter = 0f;
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(50, 401) * 0.01f;
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, vector15.X, vector15.Y, ModContent.ProjectileType<AcidicSaxMist>(), (int)(BelchingSaxophone.BaseDamage * Main.player[Projectile.owner].MagicDamage()), 1f, Projectile.owner, 0f, 0f);
                }
                else
                    counter += 1f;
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 6f)
                {
                    int num982 = 20;
                    if (Projectile.alpha > 0)
                    {
                        Projectile.alpha -= num982;
                    }
                    if (Projectile.alpha < 80)
                    {
                        Projectile.alpha = 80;
                    }
                }
                if (Projectile.ai[1] >= 45f)
                {
                    Projectile.ai[1] = 45f;
                    if (counter2 < 1f)
                    {
                        counter2 += 0.002f;
                        Projectile.scale += 0.002f;
                        Projectile.width = (int)(30f * Projectile.scale);
                        Projectile.height = (int)(30f * Projectile.scale);
                    }
                    else
                    {
                        Projectile.width = 60;
                        Projectile.height = 60;
                    }
                    if (Projectile.wet)
                    {
                        if (Projectile.velocity.Y > 0f)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y * 0.98f;
                        }
                        if (Projectile.velocity.Y > -1f)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y - 0.2f;
                        }
                    }
                    else if (Projectile.velocity.Y > -2f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
                    }
                }
                killCounter++;
                if (killCounter >= 200)
                {
                    Projectile.Kill();
                }
            }
            Projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Projectile.ModifyHitNPCSticky(3, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[Projectile.type];
            int num214 = Main.projectileTexture[Projectile.type].Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + Projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = Projectile.alpha;
                num3 = num246;
            }
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }
    }
}
