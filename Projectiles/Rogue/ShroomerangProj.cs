using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShroomerangProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Shroomerang";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomerang");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 300;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0.5f / 255f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 15 == 0 && projectile.owner == Main.myPlayer)
                {
                    Vector2 vector62 = Main.player[projectile.owner].Center - projectile.Center;
                    Vector2 vector63 = vector62 * -1f;
                    vector63.Normalize();
                    vector63 *= (float)Main.rand.Next(45, 65) * 0.1f;
                    vector63 = vector63.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866, default);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector63.X, vector63.Y, ModContent.ProjectileType<ShroomerangSpore>(), (int)(projectile.damage * 0.1), projectile.knockBack * 0.2f, projectile.owner, -10f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.Calamity().stealthStrike)
                player.AddBuff(ModContent.BuffType<Mushy>(), 720);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.Calamity().stealthStrike)
                player.AddBuff(ModContent.BuffType<Mushy>(), 720);
        }
    }
}
