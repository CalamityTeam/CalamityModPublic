using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee
{
    public class CrescentMoonFlail : BaseWhipProjectile
    {
        public int moonCounter = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crescent Moon");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override Color SpecialDrawColor => default;
        public override int ExudeDustType => 176;
        public override int WhipDustType => 176;
        public override int HandleHeight => 56;
        public override int BodyType1StartY => 60;
        public override int BodyType1SectionHeight => 22;
        public override int BodyType2StartY => 86;
        public override int BodyType2SectionHeight => 18;
        public override int TailStartY => 108;
        public override int TailHeight => 50;

        public override void ExtraBehavior()
        {
            Player player = Main.player[projectile.owner];
            if (moonCounter > 0)
                moonCounter--;
            if (moonCounter <= 0 && projectile.owner == Main.myPlayer)
            {
                Vector2 vectorBruh = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
                if (player.direction != 1)
                {
                    vectorBruh.X = (float)player.bodyFrame.Width - vectorBruh.X;
                }
                if (player.gravDir != 1f)
                {
                    vectorBruh.Y = (float)player.bodyFrame.Height - vectorBruh.Y;
                }
                vectorBruh -= new Vector2((float)(player.bodyFrame.Width - player.width), (float)(player.bodyFrame.Height - 42)) / 2f;
                Vector2 newCenter = player.RotatedRelativePoint(Main.player[projectile.owner].position + vectorBruh, true) + projectile.velocity;
                int moonDamage = (int)(projectile.damage * 0.18f);
                Projectile.NewProjectile(newCenter.X, newCenter.Y, 0f, 0f, ModContent.ProjectileType<CrescentMoonProj>(), moonDamage, 0f, projectile.owner, 0f, 0f);
                moonCounter = 6;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
            projectile.localAI[1] = 4f;
        }
    }
}
