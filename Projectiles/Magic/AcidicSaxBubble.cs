using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AcidicSaxBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public float counter = 0f;
        public float counter2 = 0f;
        public int killCounter = 0;

        public override void SetStaticDefaults()
        {
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
                    Vector2 mistRandDirection = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    mistRandDirection.Normalize();
                    mistRandDirection *= (float)Main.rand.Next(50, 401) * 0.01f;
                    int damage = (int)Main.player[Projectile.owner].GetTotalDamage<MagicDamageClass>().ApplyTo(BelchingSaxophone.BaseDamage);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, mistRandDirection.X, mistRandDirection.Y, ModContent.ProjectileType<AcidicSaxMist>(), damage, 1f, Projectile.owner, 0f, 0f);
                }
                else
                    counter += 1f;
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 6f)
                {
                    if (Projectile.alpha > 0)
                    {
                        Projectile.alpha -= 20;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(3);
        public override bool? CanDamage() => Projectile.ai[0] == 1f ? false : base.CanDamage();

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
            int inc;
            for (int i = 0; i < 25; i = inc + 1)
            {
                int toxicDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 0, default, 1f);
                Main.dust[toxicDust].position = (Main.dust[toxicDust].position + Projectile.position) / 2f;
                Main.dust[toxicDust].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[toxicDust].velocity.Normalize();
                Dust dust = Main.dust[toxicDust];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[toxicDust].alpha = Projectile.alpha;
                inc = i;
            }
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }
    }
}
