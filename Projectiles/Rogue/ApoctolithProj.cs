using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class ApoctolithProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Apoctolith";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apoctolith");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //Constant rotation and gravity
            projectile.rotation += 0.4f * projectile.direction;
            projectile.velocity.Y += 0.3f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
            //Dust trail
            if (Main.rand.NextBool(13))
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 180, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default, 0.9f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

            if (crit)
                target.Calamity().miscDefenseLoss = Math.Min(target.defense, 15);

            if (projectile.Calamity().stealthStrike)
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Tink, (int)projectile.position.X, (int)projectile.position.Y);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 180, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.15f, 120, default, 1.5f);
                dust_splash += 1;
            }
            // This only triggers if stealth is full
            if (projectile.Calamity().stealthStrike)
            {
                int split = 0;
                while (split < 5)
                {
                    //Calculate the velocity of the projectile
                    float shardspeedX = -projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                    float shardspeedY = -projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                    //Prevents the projectile speed from being too low
                    if (shardspeedX < 2f && shardspeedX > -2f)
                    {
                        shardspeedX += -projectile.velocity.X;
                    }
                    if (shardspeedY > 2f && shardspeedY < 2f)
                    {
                        shardspeedY += -projectile.velocity.Y;
                    }

                    //Spawn the projectile
                    int shard = Projectile.NewProjectile(projectile.position.X + shardspeedX, projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<ApoctolithShard>(), (int)(projectile.damage * 0.5), projectile.knockBack / 2f, projectile.owner);
                    Main.projectile[shard].frame = Main.rand.Next(3);
                    split += 1;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(32f, 33f);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ApoctolithGlow");
            Vector2 origin = new Vector2(32f, 33f);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
