using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class DeepWounderProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DeepWounder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Wounder Projectile");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.2f * projectile.direction;

            if (projectile.Calamity().stealthStrike)
            {
                int spriteWidth = 52;
                int spriteHeight = 48;
                Vector2 spriteCenter = projectile.Center - new Vector2(spriteWidth / 2, spriteHeight / 2);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(spriteCenter, spriteWidth, spriteHeight, 33, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                }

                if (Main.rand.NextBool(5))
                {
                    Vector2 waterVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    waterVelocity.Normalize();
                    waterVelocity *= 3;
                    Projectile.NewProjectile(spriteCenter, waterVelocity, ModContent.ProjectileType<DeepWounderWater>(), 20, 1, projectile.owner, 0, 0);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            if (projectile.direction > 0)
            {
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }
    }
}
