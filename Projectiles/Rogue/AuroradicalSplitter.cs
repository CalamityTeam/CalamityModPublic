using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class AuroradicalSplitter : ModProjectile
    {
        public int[] dustTypes = new int[]
        {
            ModContent.DustType<AstralBlue>(),
            ModContent.DustType<AstralOrange>()
        };

        public override string Texture => "CalamityMod/Projectiles/Rogue/AuroradicalStar";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auroradical Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 100;
            projectile.penetrate = -1;
            projectile.timeLeft = 50;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            //Rotation
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;

            //Lighting
            Lighting.AddLight(projectile.Center, 0.3f, 0.5f, 0.1f);

            //sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 9);
                }
            }

            //Change the scale size a little bit to make it pulse in and out
            float scaleAmt = (float)Main.mouseTextColor / 200f - 0.35f;
            scaleAmt *= 0.2f;
            projectile.scale = scaleAmt + 0.95f;

            //Spawn dust
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 15f)
            {
                int astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 100, default, 0.8f);
                Main.dust[astral].noGravity = true;
                Main.dust[astral].velocity *= 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer != projectile.owner)
                return;
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 9, 1f, 0f);
            for (float i = 0; i < 5; i++)
            {
                float angle = MathHelper.TwoPi / 5f * i;
                int star = Projectile.NewProjectile(projectile.Center, angle.ToRotationVector2() * 5f, ModContent.ProjectileType<AuroradicalStar>(), (int)(projectile.damage * 0.87), projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[star].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
            }
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 96;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 50, default, 1f);
            }
            for (int d = 0; d < 20; d++)
            {
                int astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 0, default, 1.5f);
                Main.dust[astral].noGravity = true;
                Main.dust[astral].velocity *= 3f;
                astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 50, default, 1f);
                Main.dust[astral].velocity *= 2f;
                Main.dust[astral].noGravity = true;
            }
            for (int g = 0; g < 3; g++)
            {
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
        }
    }
}
