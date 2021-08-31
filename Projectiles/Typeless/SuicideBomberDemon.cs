using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SuicideBomberDemon : ModProjectile
    {
        public bool HasDamagedSomething
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value.ToInt();
        }
        public Player Owner => Main.player[projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.Opacity = 0f;
            projectile.timeLeft = 300;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.friendly);
            writer.Write(projectile.hostile);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.friendly = reader.ReadBoolean();
            projectile.hostile = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (projectile.owner == 255)
                projectile.owner = Player.FindClosest(projectile.Center, 1, 1);
            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.025f, 0f, 1f);

            projectile.frame = projectile.frameCounter++ / 5 % Main.projFrames[projectile.type];

            // Anti-clumping behavior.
            float pushForce = 0.08f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || otherProj.type != projectile.type || k == projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == projectile.type;
                float taxicabDist = MathHelper.Distance(projectile.position.X, otherProj.position.X) + MathHelper.Distance(projectile.position.Y, otherProj.position.Y);
                if (sameProjType && taxicabDist < 60f)
                {
                    if (projectile.position.X < otherProj.position.X)
                        projectile.velocity.X -= pushForce;
                    else
                        projectile.velocity.X += pushForce;

                    if (projectile.position.Y < otherProj.position.Y)
                        projectile.velocity.Y -= pushForce;
                    else
                        projectile.velocity.Y += pushForce;
                }
            }

            if (projectile.hostile)
            {
                projectile.velocity = projectile.velocity.MoveTowards(projectile.SafeDirectionTo(Owner.Center) * 16f, 1.4f);
                projectile.spriteDirection = (Owner.Center.X > projectile.Center.X).ToDirectionInt();

                if (HasDamagedSomething && projectile.WithinRange(Owner.Center, Owner.Size.Length() * 0.25f))
                    projectile.Kill();
            }
            else if (projectile.friendly)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1360f);
                if (potentialTarget is null)
                    projectile.velocity = Vector2.Lerp(projectile.velocity, -Vector2.UnitY * 10f, 0.06f);
                else
                {
                    projectile.spriteDirection = (potentialTarget.Center.X > projectile.Center.X).ToDirectionInt();
                    projectile.velocity = projectile.velocity.MoveTowards(projectile.SafeDirectionTo(potentialTarget.Center) * 18.5f, 3f);
                    projectile.friendly = true;
                }

                if (HasDamagedSomething && projectile.WithinRange(potentialTarget.Center, potentialTarget.Size.Length() * 0.25f))
                    projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Dust explosion = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 267);
                explosion.velocity = Main.rand.NextVector2Circular(4f, 4f);
                explosion.color = Color.Red;
                explosion.scale = 1.35f;
                explosion.fadeIn = 0.45f;
                explosion.noGravity = true;
            }
        }

        public override bool CanDamage() => projectile.Opacity >= 1f;

        // Ensure damage is not absolutely obscene when hitting players.
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage = 95;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            HasDamagedSomething = true;
            projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HasDamagedSomething = true;
            projectile.netUpdate = true;
        }
    }
}
