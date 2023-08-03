using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator right click main projectile (invisible flare cluster bomb)
    public class ExoFlareCluster : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float MinDistanceFromTarget = 45f;
        public const float MaxDistanceFromTarget = 350f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            // localAI[0] is used by the sticky AI method, so use localAI[1] to spawn the flares.
            if (Projectile.localAI[1] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int projID = ModContent.ProjectileType<ExoFlare>();
                    int flareDamage = (int)(0.6f * Projectile.damage);
                    float flareKB = Projectile.knockBack;
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projID, flareDamage, flareKB, Projectile.owner);
                        p.localAI[1] = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);
                    }
                }
                Projectile.localAI[1] = 1f;
            }

            if (Projectile.ai[0] == 0f)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(MaxDistanceFromTarget, true, true);
                if (potentialTarget != null)
                {
                    if (Projectile.Distance(potentialTarget.Center) > MinDistanceFromTarget)
                    {
                        float angleOffset = Projectile.AngleTo(potentialTarget.Center) - Projectile.velocity.ToRotation();
                        angleOffset = MathHelper.WrapAngle(angleOffset);
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Clamp(angleOffset, -0.1f, 0.1f));
                    }
                }
            }
            Projectile.StickyProjAI(5);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(4);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.width == 50)
            {
                int width = (int)MathHelper.Min(target.Hitbox.Width, 60);
                int height = (int)MathHelper.Min(target.Hitbox.Height, 60);
                Projectile.ExpandHitboxBy(width, height);
            }
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);
        }
    }
}
