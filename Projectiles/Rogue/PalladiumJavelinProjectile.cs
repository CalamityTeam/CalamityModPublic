using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class PalladiumJavelinProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/PalladiumJavelin";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Javelin");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;

            //Stealth strike behavior
            if (!projectile.Calamity().stealthStrike || projectile.owner != Main.myPlayer || projectile.Calamity().lineColor >= 2)
                return;
            projectile.localAI[0]++;
            if (projectile.localAI[0] >= 30f)
            {
                Vector2 vector2 = new Vector2(20f, 20f);
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 87, 0f, 0f, 100, new Color(), 1.5f);
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 1.4f;
                }
                for (int index1 = 0; index1 < 5; ++index1)
                {
                    Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 144, 0f, 0f, 0, default, 1f);
                }

                int javelin = Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(MathHelper.ToRadians(5f)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                int javelin2 = Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(MathHelper.ToRadians(-5f)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                if (javelin.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[javelin].Calamity().lineColor = projectile.Calamity().lineColor + 1;
                    Main.projectile[javelin].Calamity().stealthStrike = true;
                }
                if (javelin2.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[javelin2].Calamity().lineColor = projectile.Calamity().lineColor + 1;
                    Main.projectile[javelin2].Calamity().stealthStrike = true;
                }
                projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2) && !projectile.Calamity().stealthStrike)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<PalladiumJavelin>());
            }
        }
    }
}
