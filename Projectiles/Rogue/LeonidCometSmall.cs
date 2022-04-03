using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class LeonidCometSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Comet");
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.Calamity().rogue = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/LeonidProgenitorGlow");
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Main.rand.NextBool(4))
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });
                if (Main.rand.NextBool(2))
                {
                    int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.6f);
                    Main.dust[index].noGravity = true;
                }
                if (Main.rand.NextBool(2))
                {
                    int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.6f);
                    Main.dust[index].noGravity = true;
                }
                int index1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.2f);
                Main.dust[index1].noGravity = true;
                Main.dust[index1].velocity *= 2f;
                Main.dust[index1].velocity += Projectile.velocity;
                Main.dust[index1].fadeIn = Projectile.ai[1] != -1f ? 1.22f : 1.5f;
            }
            if (Projectile.ai[0] >= 0f)
            {
                ++Projectile.ai[0];
                if (Projectile.ai[0] >= 20f)
                {
                    Projectile.velocity.Y += 0.2f;
                    if (Projectile.velocity.Y > 0f)
                        Projectile.velocity.X *= 0.98f;
                    if (Projectile.velocity.Y > 12f)
                        Projectile.velocity.Y = 12f;
                }
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LeonidStar>(), Projectile.damage / 3, 0f, Projectile.owner);
            Projectile.ai[1] = -1f;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 40;
            Projectile.Center = Projectile.position;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.5f);
                Dust dust = Main.dust[index2];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * Projectile.width / 2f;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });
                int index2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 0, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 2.5f);
                Dust dust = Main.dust[index2];
                dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 0, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.5f);
                Dust dust = Main.dust[index2];
                dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }
    }
}
