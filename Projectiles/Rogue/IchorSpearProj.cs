using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class IchorSpearProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IchorSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = ProjectileID.BoneJavelin;
            projectile.Calamity().rogue = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }

            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 8 == 0)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-14f, 14f), Main.rand.NextFloat(-14f, 14f));
                        int ichor = Projectile.NewProjectile(projectile.Center, velocity, Main.rand.NextBool(2) ? ProjectileID.GoldenShowerFriendly : ProjectileID.IchorSplash, (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner);
                        if (ichor.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[ichor].Calamity().forceRogue = true;
                            Main.projectile[ichor].usesLocalNPCImmunity = true;
                            Main.projectile[ichor].localNPCHitCooldown = 10;
                            Main.projectile[ichor].extraUpdates = 2;
                        }
                    }
                }
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
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, projectile.Calamity().stealthStrike ? 600 : 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, projectile.Calamity().stealthStrike ? 600 : 120);
        }
    }
}
