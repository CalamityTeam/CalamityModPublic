using CalamityMod.Buffs.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class DaawnlightSpiritOriginMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 146;
            Projectile.height = 164;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!Owner.active)
            {
                Projectile.active = false;
                return;
            }
            HandlePetStuff();
            DoMovement();
            HandleFrames();
        }

        public void HandlePetStuff()
        {
            if (Owner.dead || !Owner.Calamity().spiritOrigin && !Owner.Calamity().spiritOriginVanity)
                Owner.Calamity().spiritOriginPet = false;
            if (Owner.Calamity().spiritOriginPet)
                Projectile.timeLeft = 2;
        }

        public void DoMovement()
        {
            if (Projectile.WithinRange(Owner.Center, 100f))
                Projectile.velocity *= 0.975f;
            else
            {
                float flySpeed = MathHelper.Clamp(11f + Projectile.Distance(Owner.Center) * 0.015f, 11f, 25f);
                Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.SafeDirectionTo(Owner.Center) * flySpeed, flySpeed * 0.02f);
                if (!Projectile.WithinRange(Owner.Center, 2200f))
                {
                    Projectile.Center = Owner.Center;
                    Projectile.velocity = -Vector2.UnitY * 4f;
                    Projectile.netUpdate = true;
                }
            }

            if (MathHelper.Distance(Projectile.Center.X, Owner.Center.X) > 80f)
                Projectile.spriteDirection = (Projectile.Center.X > Owner.Center.X).ToDirectionInt();
        }

        public void HandleFrames()
        {
            if (Owner.Calamity().spiritOriginBullseyeShootCountdown > 0)
                Projectile.frame = (int)MathHelper.Lerp(5f, Main.projFrames[Projectile.type] - 0.1f, 1f - Owner.Calamity().spiritOriginBullseyeShootCountdown / 45f);
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter % 7 == 6)
                    Projectile.frame = (Projectile.frame + 1) % 5;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Owner.FindBuffIndex(ModContent.BuffType<ArcherofLunamoon>()) != -1)
                Owner.ClearBuff(ModContent.BuffType<ArcherofLunamoon>());
        }
    }
}
