using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicIceBurst : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1];
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= (float)(3 * Main.projFrames[Projectile.type]))
            {
                Projectile.Kill();
                return;
            }
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.hide = true;
                }
            }
            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, 0.517f, (float)Main.DiscoG / 300f, 0.85f);
            if (Projectile.ai[0] == 1f)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(52f * Projectile.scale);
                Projectile.Center = Projectile.position;
                Projectile.Damage();
                SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
                for (int i = 0; i < 2; i++)
                {
                    int cosmicDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 187, 0f, 0f, 100, new Color(150, 255, 255), 1f);
                    Main.dust[cosmicDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                }
                for (int j = 0; j < 3; j++)
                {
                    int iceDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 200, new Color(150, 255, 255), 1f);
                    Main.dust[iceDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    Main.dust[iceDust].noGravity = true;
                    Main.dust[iceDust].velocity *= 3f;
                    iceDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 100, new Color(150, 255, 255), 0.6f);
                    Main.dust[iceDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    Main.dust[iceDust].velocity *= 2f;
                    Main.dust[iceDust].noGravity = true;
                    Main.dust[iceDust].fadeIn = 2.5f;
                }
                for (int k = 0; k < 2; k++)
                {
                    int icyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 67, 0f, 0f, 0, new Color(150, 255, 255), 1f);
                    Main.dust[icyDust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    Main.dust[icyDust].noGravity = true;
                    Main.dust[icyDust].velocity *= 3f;
                }
                for (int l = 0; l < 3; l++)
                {
                    int freezeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 187, 0f, 0f, 0, new Color(150, 255, 255), 0.6f);
                    Main.dust[freezeDust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    Main.dust[freezeDust].noGravity = true;
                    Main.dust[freezeDust].velocity *= 3f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Color colorArea = Lighting.GetColor((int)((double)Projectile.position.X + (double)Projectile.width * 0.5) / 16, (int)(((double)Projectile.position.Y + (double)Projectile.height * 0.5) / 16.0));
            if (Projectile.hide && !ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type])
            {
                colorArea = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f));
            }
            Texture2D texture2D33 = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rectangl = texture2D33.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            return true;
        }

        /*public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, Main.DiscoG, 255, 127);
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 420);
            Projectile.direction = Main.player[Projectile.owner].direction;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 420);
            Projectile.direction = Main.player[Projectile.owner].direction;
        }
    }
}
