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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 113;
            Projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            Projectile.Calamity().rogue = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;

            //Stealth strike behavior
            if (!Projectile.Calamity().stealthStrike || Projectile.owner != Main.myPlayer || Projectile.Calamity().lineColor >= 2)
                return;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 30f)
            {
                Vector2 vector2 = new Vector2(20f, 20f);
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 87, 0f, 0f, 100, new Color(), 1.5f);
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 1.4f;
                }
                for (int index1 = 0; index1 < 5; ++index1)
                {
                    Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 144, 0f, 0f, 0, default, 1f);
                }

                int javelin = Projectile.NewProjectile(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(5f)), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                int javelin2 = Projectile.NewProjectile(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(-5f)), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (javelin.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[javelin].Calamity().lineColor = Projectile.Calamity().lineColor + 1;
                    Main.projectile[javelin].Calamity().stealthStrike = true;
                }
                if (javelin2.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[javelin2].Calamity().lineColor = Projectile.Calamity().lineColor + 1;
                    Main.projectile[javelin2].Calamity().stealthStrike = true;
                }
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2) && !Projectile.Calamity().stealthStrike)
            {
                Item.NewItem((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<PalladiumJavelin>());
            }
        }
    }
}
