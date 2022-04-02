using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/LeonidProgenitorGlow");
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            projectile.alpha -= 25;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (Main.rand.NextBool(4))
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });
                if (Main.rand.NextBool(2))
                {
                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, randomDust, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.6f);
                    Main.dust[index].noGravity = true;
                }
                if (Main.rand.NextBool(2))
                {
                    int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.6f);
                    Main.dust[index].noGravity = true;
                }
                int index1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, randomDust, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.2f);
                Main.dust[index1].noGravity = true;
                Main.dust[index1].velocity *= 2f;
                Main.dust[index1].velocity += projectile.velocity;
                Main.dust[index1].fadeIn = projectile.ai[1] != -1f ? 1.22f : 1.5f;
            }
            if (projectile.ai[0] >= 0f)
            {
                ++projectile.ai[0];
                if (projectile.ai[0] >= 20f)
                {
                    projectile.velocity.Y += 0.2f;
                    if (projectile.velocity.Y > 0f)
                        projectile.velocity.X *= 0.98f;
                    if (projectile.velocity.Y > 12f)
                        projectile.velocity.Y = 12f;
                }
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LeonidStar>(), projectile.damage / 3, 0f, projectile.owner);
            projectile.ai[1] = -1f;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 40;
            projectile.Center = projectile.position;
            projectile.Damage();
            Main.PlaySound(SoundID.Item89, projectile.position);
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.5f);
                Dust dust = Main.dust[index2];
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * projectile.width / 2f;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDust, 0f, 0f, 0, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 2.5f);
                Dust dust = Main.dust[index2];
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 2f;
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 0, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.5f);
                Dust dust = Main.dust[index2];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2()) * projectile.width / 2f;
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
