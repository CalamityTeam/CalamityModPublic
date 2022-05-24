using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.Providence;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityBook : ModProjectile
    {
        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center + 18f * player.direction * Vector2.UnitX;

            // If the player is no longer able to hold the book, kill it (and by extension the other projectiles).
            if (!player.channel || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            Time++;

            // Summon a bunch of cool things the moment the book is created, assuming an NPC is near the mouse position.
            if (Main.myPlayer == Projectile.owner && Time == 1f)
            {
                NPC target = Main.MouseWorld.ClosestNPCAt(4400f, true, true);

                if (target != null)
                {
                    SoundEngine.PlaySound(Providence.HolyRaySound);
                    SummonProjectilesOnTarget(target, player);
                }
            }

            // Switch frames at a linearly increasing rate to make it look like the player is flipping pages quickly.
            if (Projectile.frameCounter++ >= (int)MathHelper.Lerp(10f, 1f, Utils.GetLerpValue(0f, 200f, Time, true)))
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }

            AdjustPlayerValues(player);
        }
        public void AdjustPlayerValues(Player player)
        {
            Projectile.spriteDirection = Projectile.direction = player.direction;
            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.direction * Projectile.velocity).ToRotation();
        }
        public void SummonProjectilesOnTarget(NPC target, Player owner)
        {
            Projectile hex = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EternityHex>(), Projectile.damage, 0f, owner.whoAmI, target.whoAmI);
            hex.localAI[1] = Projectile.whoAmI;

            for (int i = 0; i < 5; i++)
            {
                float crystalAngleOffset = MathHelper.TwoPi / 5f * i;
                Projectile crystal = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCrystal>(), 0, 0f, owner.whoAmI, target.whoAmI, crystalAngleOffset);
                crystal.frame = i % 2;
                crystal.localAI[1] = Projectile.whoAmI;
            }
            for (int i = 0; i < 10; i++)
            {
                float circleOffset = MathHelper.TwoPi / 10f * i;
                Projectile circleSpell = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCircle>(), 0, 0f, owner.whoAmI, target.whoAmI, circleOffset);
                circleSpell.localAI[1] = Projectile.whoAmI;
            }
        }
        public override bool? CanDamage() => false;
    }
}
