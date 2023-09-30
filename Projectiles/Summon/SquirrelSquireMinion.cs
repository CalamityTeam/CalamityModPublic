using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SquirrelSquireMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float AttackTimer => ref Projectile.ai[1];
        public bool Attacking
        {
            get => Projectile.localAI[1] == 1f;
            set => Projectile.localAI[1] = value.ToInt();
        }
        public bool OnSolidGround
        {
            get
            {
                bool groundSolid = false;
                for (int i = (int)Projectile.Left.X / 16 - 1; i < (int)Projectile.Right.X / 16 + 1; i++)
                {
                    bool bottomTileSolid = CalamityUtils.ParanoidTileRetrieval(i, (int)Projectile.Bottom.Y / 16).IsTileSolidGround();
                    bool firstTileDownSolid = CalamityUtils.ParanoidTileRetrieval(i, (int)Projectile.Bottom.Y / 16 + 1).IsTileSolidGround();
                    bool secondTileDownSolid = CalamityUtils.ParanoidTileRetrieval(i, (int)Projectile.Bottom.Y / 16 + 2).IsTileSolidGround();
                    groundSolid |= bottomTileSolid || firstTileDownSolid || secondTileDownSolid;
                }
                return groundSolid;
            }
        }
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 64;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                DoInitializationEffects();
                Projectile.localAI[0] = 1f;
            }

            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.004f, -12f, 12f);
            Projectile.frameCounter++;

            Attacking = false;
            NPC potentialTarget = Projectile.Center.MinionHoming(800f, Owner, false);
            if (potentialTarget is null)
            {
                if (OnSolidGround)
                {
                    if (Projectile.frameCounter > 3)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame > 3)
                    {
                        Projectile.frame = 0;
                    }
                }
                else
                {
                    if (Projectile.frameCounter > 5)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame < 8 || Projectile.frame > 11)
                    {
                        Projectile.frame = 9;
                    }
                }
            }
            else
            {
                Attacking = true;
                AttackTarget(potentialTarget);
            }

            Projectile.rotation = 0f;
            Projectile.tileCollide = true;
            Projectile.StickToTiles(false, false);
        }

        public void DoInitializationEffects()
        {
            int dustQuantity = 36;
            for (int d = 0; d < dustQuantity; d++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 7);
                dust.scale = 1.4f;
                dust.velocity = (MathHelper.TwoPi * d / dustQuantity).ToRotationVector2() * 4f;
                dust.noGravity = true;
                dust.noLight = true;
            }
        }

        public void AttackTarget(NPC target)
        {
            AttackTimer++;

            // Pelt the target with acorns.
            if (!OnSolidGround)
                Projectile.frame = 12 + (int)(AttackTimer / 6.4) % 4;
            else
            {
                Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.2f, -12f, 12f);
                Projectile.frame = 4 + (int)(AttackTimer / 6.4) % 4;
            }
            if (Main.myPlayer == Projectile.owner && AttackTimer % 30f == 27f)
            {
                Projectile.spriteDirection = (target.Center.X > Projectile.Center.X).ToDirectionInt();
                Vector2 acornSpawnPosition = Projectile.Center + new Vector2(Projectile.spriteDirection * 6f, 10f);
                float acornShootSpeed = MathHelper.Lerp(15f, 32f, Projectile.Distance(target.Center) / 800f);
                Vector2 acornShootVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(acornSpawnPosition, target.Top + target.velocity * 25f, SquirrelSquireAcorn.Gravity, acornShootSpeed);

                if (Projectile.WithinRange(target.Center, 200f))
                    acornShootVelocity = (target.Center - acornSpawnPosition).SafeNormalize(-Vector2.UnitY) * acornShootSpeed;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), acornSpawnPosition, acornShootVelocity, ModContent.ProjectileType<SquirrelSquireAcorn>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                int index = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Main.rand.Next(61, 64), Projectile.scale);
                Main.gore[index].velocity *= 0.1f;
            }
        }
    }
}
