using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FantasyTalismanStealth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Rogue/FantasyTalismanProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 175, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            if (Projectile.ai[0] != 1f)
            {
                if (Projectile.timeLeft % 10 == 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        // Use a tiny velocity to ensure that rotation works correctly.
                        // The speed should be so low that it will make no meaningful mechanical difference.
                        Vector2 soulVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.0001f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, soulVelocity, ModContent.ProjectileType<LostSoulFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    }
                }
            }
            Projectile.StickyProjAI(4);
            if (Projectile.ai[0] == 1f)
            {
                if (Projectile.timeLeft % 4 == 0)
                {
                    if (Main.rand.NextBool(2))
                    {
                        int spiritDamage = Projectile.damage / 2;
                        Projectile ghost = CalamityUtils.SpawnOrb(Projectile, spiritDamage, ProjectileID.SpectreWrath, 800f, 4f);
                        if (ghost.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            ghost.DamageType = RogueDamageClass.Instance;
                            ghost.penetrate = 1;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(1);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }
    }
}
