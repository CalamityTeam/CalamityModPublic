using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class MicrowaveYoyo : ModProjectile
    {
        private const float Radius = 100f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Microwave");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 320f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 14f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
        	if (projectile.owner == Main.myPlayer)
        	{
            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MicrowaveAura>(), (int)((double)projectile.damage * 0.5f), projectile.knockBack, projectile.owner, 0f, 0f);

				//dust circle
				int numDust = (int)(0.2f * MathHelper.TwoPi * Radius);
				float angleIncrement = MathHelper.TwoPi / (float)numDust;
				Vector2 dustOffset = new Vector2(Radius, 0f);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, 20, 16);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/Yoyos/MicrowaveYoyoGlow"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }
    }
}
