using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class BonebreakerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonebreaker");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.melee = true;
			projectile.timeLeft = 600;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -2;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.localAI[1] += 1f;
                if (projectile.localAI[1] >= 45f)
                {
                    projectile.velocity.X *= 0.98f;
                    projectile.velocity.Y += 0.35f;
                }
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            //Sticky Behaviour
            projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(6, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 3; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 78, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
            if (projectile.owner == Main.myPlayer)
            {
				for (int s = 0; s < Main.rand.Next(2,5); s++)
				{
					Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (velocity.X == 0f && velocity.Y == 0f)
					{
						velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					velocity.Normalize();
					velocity *= (float)Main.rand.Next(70, 101) * 0.1f;
					int shard = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<BonebreakerFragment1>(), (int)(projectile.damage * 0.5f), projectile.knockBack * 0.5f, projectile.owner, Main.rand.Next(0,4), 0f);
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.BoneJavelin, 240);
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BoneJavelin, 240);
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
        }

        public override bool? CanHitNPC(NPC target)
		{
			if (projectile.ai[0] == 1f)
			{
				return false;
			}
			return null;
		}

		public override bool CanHitPvp(Player target) => projectile.ai[0] != 1f;
    }
}
