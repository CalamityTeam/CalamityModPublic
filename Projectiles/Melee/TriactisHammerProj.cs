using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TriactisHammerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/TriactisTruePaladinianMageHammerofMightMelee";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.75f);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 30f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.tileCollide = false;
                float projVelModifier = 5f;
                Vector2 projDirection = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float projXDist = Main.player[Projectile.owner].position.X + Main.player[Projectile.owner].width / 2 - projDirection.X;
                float projYDist = Main.player[Projectile.owner].position.Y + Main.player[Projectile.owner].height / 2 - projDirection.Y;
                float projDistance = (float)Math.Sqrt(projXDist * projXDist + projYDist * projYDist);
                if (projDistance > 3000f)
                    Projectile.Kill();
                projDistance = 20f / projDistance;
                projXDist *= projDistance;
                projYDist *= projDistance;
                if (Projectile.velocity.X < projXDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + projVelModifier;
                    if (Projectile.velocity.X < 0f && projXDist > 0f)
                        Projectile.velocity.X = Projectile.velocity.X + projVelModifier;
                }
                else if (Projectile.velocity.X > projXDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - projVelModifier;
                    if (Projectile.velocity.X > 0f && projXDist < 0f)
                        Projectile.velocity.X = Projectile.velocity.X - projVelModifier;
                }
                if (Projectile.velocity.Y < projYDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + projVelModifier;
                    if (Projectile.velocity.Y < 0f && projYDist > 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y + projVelModifier;
                }
                else if (Projectile.velocity.Y > projYDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - projVelModifier;
                    if (Projectile.velocity.Y > 0f && projYDist < 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y - projVelModifier;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                    Rectangle playerArea = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    if (rectangle.Intersects(playerArea))
                        Projectile.Kill();
                }
            }
            Projectile.rotation += 0.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 50);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MageHammerBoom>(), (int)(Projectile.damage * 0.25), Projectile.knockBack, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 40; i++)
            {
                int triactisDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool() ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[triactisDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[triactisDust].scale = 0.5f;
                    Main.dust[triactisDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 70; j++)
            {
                int triactisDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool() ? 89 : 229, 0f, 0f, 100, default, 3f);
                Main.dust[triactisDust2].noGravity = true;
                Main.dust[triactisDust2].velocity *= 5f;
                triactisDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool() ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[triactisDust2].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MageHammerBoom>(), (int)(Projectile.damage * 0.25), Projectile.knockBack, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 40; i++)
            {
                int triactisDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool() ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[triactisDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[triactisDust].scale = 0.5f;
                    Main.dust[triactisDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 70; j++)
            {
                int triactisDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool() ? 89 : 229, 0f, 0f, 100, default, 3f);
                Main.dust[triactisDust2].noGravity = true;
                Main.dust[triactisDust2].velocity *= 5f;
                triactisDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool() ? 89 : 229, 0f, 0f, 100, default, 2f);
                Main.dust[triactisDust2].velocity *= 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
