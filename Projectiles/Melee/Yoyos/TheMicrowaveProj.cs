using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class TheMicrowaveProj : ModProjectile
    {
		private int radius = 100;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Bootleg Lacerator");
            DisplayName.SetDefault("Microwave");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
            projectile.width = 16;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            aiType = 555;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
        	if (projectile.owner == Main.myPlayer)
        	{
            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MicrowaveAura>(), (int)((double)projectile.damage * 0.5f), projectile.knockBack, projectile.owner, 0f, 0f);

				//dust circle
				int numDust = (int)(0.2f * MathHelper.TwoPi * radius);
				float angleIncrement = MathHelper.TwoPi / (float)numDust;
				Vector2 dustOffset = new Vector2(radius, 0f);
				dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
				for (int i = 0; i < numDust; i++)
				{
					dustOffset = dustOffset.RotatedBy(angleIncrement);
					int dustType = Utils.SelectRandom(Main.rand, new int[]
					{
						ModContent.DustType<AstralOrange>(),
						ModContent.DustType<AstralBlue>()
					});
					int dust = Dust.NewDust(projectile.Center, 1, 1, dustType);
					Main.dust[dust].position = projectile.Center + dustOffset;
					Main.dust[dust].fadeIn = 1f;
					Main.dust[dust].velocity *= 0.2f;
					Main.dust[dust].scale = 0.1599999999f;
				}
			}
		}

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, 20, 16);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/Yoyos/TheMicrowaveProjGlow"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }
    }
}
