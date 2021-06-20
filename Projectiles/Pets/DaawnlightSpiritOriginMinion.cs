using CalamityMod.Buffs.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class DaawnlightSpiritOriginMinion : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daawnlight");
            Main.projFrames[projectile.type] = 12;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 146;
            projectile.height = 164;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Owner.active)
            {
                projectile.active = false;
                return;
            }
            HandlePetStuff();
            DoMovement();
            HandleFrames();
        }

        public void HandlePetStuff()
        {
            if (Owner.dead || !Owner.Calamity().spiritOrigin)
                Owner.Calamity().spiritOriginPet = false;
            if (Owner.Calamity().spiritOriginPet)
                projectile.timeLeft = 2;
        }

        public void DoMovement()
        {
            if (projectile.WithinRange(Owner.Center, 100f) || Owner.Calamity().spiritOriginBullseyeShootCountdown > 0)
                projectile.velocity *= 0.975f;
            else
            {
                float flySpeed = MathHelper.Clamp(11f + projectile.Distance(Owner.Center) * 0.015f, 11f, 25f);
                projectile.velocity = projectile.velocity.MoveTowards(projectile.SafeDirectionTo(Owner.Center) * flySpeed, flySpeed * 0.02f);
                if (!projectile.WithinRange(Owner.Center, 2200f))
                {
                    projectile.Center = Owner.Center;
                    projectile.velocity = -Vector2.UnitY * 4f;
                    projectile.netUpdate = true;
                }
            }

            if (MathHelper.Distance(projectile.Center.X, Owner.Center.X) > 80f)
                projectile.spriteDirection = (projectile.Center.X > Owner.Center.X).ToDirectionInt();
        }

        public void HandleFrames()
        {
            if (Owner.Calamity().spiritOriginBullseyeShootCountdown > 0)
                projectile.frame = (int)MathHelper.Lerp(5f, Main.projFrames[projectile.type] - 0.1f, 1f - Owner.Calamity().spiritOriginBullseyeShootCountdown / 45f);
            else
            {
                projectile.frameCounter++;
                if (projectile.frameCounter % 7 == 6)
                    projectile.frame = (projectile.frame + 1) % 5;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Owner.FindBuffIndex(ModContent.BuffType<DaawnlightSpiritOriginBuff>()) != -1)
                Owner.ClearBuff(ModContent.BuffType<DaawnlightSpiritOriginBuff>());
        }
    }
}
