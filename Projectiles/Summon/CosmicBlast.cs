using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class CosmicBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int rainbowDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                Main.dust[rainbowDust].noGravity = true;
                Main.dust[rainbowDust].velocity = value3 * 0.66f;
                Main.dust[rainbowDust].position = Projectile.Center + value3 * 12f;
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(potentialTarget.Center) * 40f) / 21f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 144;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int j = 0; j < 30; j++)
            {
                int rainbowDusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 2.5f);
                Main.dust[rainbowDusty].noGravity = true;
                Main.dust[rainbowDusty].velocity *= 3f;
                rainbowDusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.5f);
                Main.dust[rainbowDusty].velocity *= 2f;
                Main.dust[rainbowDusty].noGravity = true;
            }
        }
    }
}
