using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SkeletalDragonChild : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 48;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
            Projectile.minionSlots = 0f;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] < 0 || Projectile.ai[0] >= Main.projectile.Length)
            {
                Projectile.Kill();
                return;
            }

            Projectile mother = Main.projectile[(int)Projectile.ai[0]];

            if (!mother.active || mother.type != ModContent.ProjectileType<SkeletalDragonMother>())
            {
                Projectile.Kill();
                return;
            }

            NPC target = Projectile.Center.MinionHoming(SkeletalDragonMother.DistanceToCheck * 0.666f, player);

            if (target != null)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] % 55f == 54f && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 19f, ModContent.ProjectileType<BloodSpit>(), mother.damage, mother.knockBack, mother.owner);
                }
            }

            // If the mother is dive-bombing
            if (mother.ai[1] == 1f)
            {
                if (target != null)
                {
                    if (Projectile.Distance(mother.Center) > 620f)
                    {
                        Projectile.velocity = (mother.Center - Projectile.Center) / 45f;
                    }
                    else
                    {
                        Projectile.velocity = (Projectile.velocity * 19f + Projectile.SafeDirectionTo(mother.Center) * 18f) / 20f;
                    }
                }
                else
                {
                    if (Projectile.Distance(mother.Center) > 250f)
                    {
                        Projectile.velocity = (mother.Center - Projectile.Center) / 28f;
                    }
                    else
                    {
                        Projectile.velocity += new Vector2(Math.Sign(mother.Center.X - Projectile.Center.X), Math.Sign(mother.Center.Y - Projectile.Center.Y)) * new Vector2(0.04f, 0.0225f);
                        if (Projectile.velocity.Length() > 8f)
                        {
                            Projectile.velocity *= 8f / Projectile.velocity.Length();
                        }
                    }
                }
            }
            else
            {
                if (Projectile.Distance(mother.Center) > 250f)
                {
                    Projectile.velocity = (mother.Center - Projectile.Center) / 28f;
                }
                else
                {
                    Projectile.velocity += new Vector2(Math.Sign(mother.Center.X - Projectile.Center.X), Math.Sign(mother.Center.Y - Projectile.Center.Y)) * new Vector2(0.04f, 0.0225f);
                    if (Projectile.velocity.Length() > 8f)
                    {
                        Projectile.velocity *= 8f / Projectile.velocity.Length();
                    }
                }
            }

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }
    }
}
