using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Items.Weapons.Melee;

namespace CalamityMod.Projectiles.Melee
{
    public class Exobeam : ModProjectile
    {
        public int TargetIndex = -1;

        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exobeam");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 106;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 12;
            Projectile.light = 1f;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item60, Projectile.position);
                Projectile.localAI[1] = 1f;
            }

            // Aim very, very quickly at targets.
            // This takes a small amount of time to happen, to allow the blade to go in its intended direction before immediately racing
            // towards the nearest target.
            if (Time >= Exoblade.BeamNoHomeTime)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f, false);
                if (TargetIndex == -1 && potentialTarget != null)
                    TargetIndex = potentialTarget.whoAmI;
                if (TargetIndex >= 0 && Main.npc[TargetIndex].active && Main.npc[TargetIndex].CanBeChasedBy())
                {
                    Vector2 idealVelocity = Projectile.SafeDirectionTo(Main.npc[TargetIndex].Center) * (Projectile.velocity.Length() + 3.5f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.08f);
                }
            }

            // Dissipate if colliding with tiles.
            if (Time >= Exoblade.BeamNoHomeTime + 16f && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.Opacity -= 0.08f;
                if (Projectile.Opacity <= 0f)
                    Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);

            if (Projectile.FinalExtraUpdate())
                Time++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(YanmeisKnife.HitSound, target.Center);
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity * 0.1f, ModContent.ProjectileType<ExobeamSlashCreator>(), Projectile.damage, 0f, Projectile.owner, target.whoAmI);

            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White with { A = 0 } * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 4);
            return false;
        }
    }
}
