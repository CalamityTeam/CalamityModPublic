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
    public class TarragonThrowingDartProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TarragonThrowingDart";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Throwing Dart");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 6;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
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
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<TarragonThrowingDart>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == projectile.owner)
            {
                if (projectile.Calamity().stealthStrike)
                {
                    float random = Main.rand.Next(30, 90);
                    float spread = random * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;

                    int projID = ModContent.ProjectileType<TarraThornRight>();
                    int splitDamage = (int)(projectile.damage * 0.25f);
                    float splitKB = 1f;
                    for (int i = 0; i < 4; i++)
                    {
                        double offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f) * 2f, (float)(Math.Cos(offsetAngle) * 5f) * 2f, projID, splitDamage, splitKB, projectile.owner);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f) * 2f, (float)(-Math.Cos(offsetAngle) * 5f) * 2f, projID, splitDamage, splitKB, projectile.owner);
                    }
                }
            }
        }
    }
}
