using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShinobiBladeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShinobiBlade";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 5;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.PiOver2 * Projectile.spriteDirection;

            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {            
            NPC firstTarget = Main.npc[(int)Projectile.ai[0]];
            
            // 7 hits total
            if (Projectile.Calamity().stealthStrike && Projectile.ai[1] <= 7f && (Projectile.ai[1] == 0f || firstTarget != null))
			{
				Vector2 targetPos = Projectile.ai[1] == 0f ? target.Center : firstTarget.Center;
				Vector2 offset = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(80f, 120f);
                Vector2 eVelocity = Vector2.UnitX.RotatedBy(offset.ToRotation() + MathHelper.Pi) * 4f;
                int realTarget = Projectile.ai[1] == 0f ? target.whoAmI : firstTarget.whoAmI;
				Projectile echo = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), targetPos + offset, eVelocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, realTarget, Projectile.ai[1] + 1);
				echo.Calamity().stealthStrike = true;
			}

            if (target.life <= 0)
                CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], 10, ModContent.ProjectileType<ShinobiHealOrb>(), 1200f, 0f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (target.statLife <= 0)
                CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], 10, ModContent.ProjectileType<ShinobiHealOrb>(), 1200f, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = 42;
                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
