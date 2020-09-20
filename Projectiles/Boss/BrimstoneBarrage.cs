using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
	public class BrimstoneBarrage : ModProjectile
    {
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Dart");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 690;
			cooldownSlot = 1;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
        {
			if (projectile.velocity.Length() < (projectile.ai[1] == 0f ? 14f : 10f))
				projectile.velocity *= 1.01f;

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

			if (projectile.timeLeft < 60)
				projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 60f, 0f, 1f);

			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				projectile.damage = projectile.GetProjectileDamage(ModContent.NPCType<SupremeCalamitas>());
			}

			Lighting.AddLight(projectile.Center, 0.75f, 0f, 0f);
        }

		public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.Opacity != 1f)
				return;

			target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);

            if (projectile.ai[0] == 0f)
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			lightColor.R = (byte)(255 * projectile.Opacity);
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
