using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityBook : ModProjectile
    {
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            projectile.Center = player.Center + 18f * player.direction * Vector2.UnitX;

            // If the player is no longer able to hold the book, kill it (and by extension the other projectiles).
            if (!player.channel || player.noItems || player.CCed)
            {
                projectile.Kill();
                return;
            }

            Time++;

            // Summon a bunch of cool things the moment the book is created, assuming an NPC is near the mouse position.
            if (Main.myPlayer == projectile.owner && Time == 1f)
            {
                NPC target = Main.MouseWorld.ClosestNPCAt(4400f, true, true);

                if (target != null)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyRay"));
                    SummonProjectilesOnTarget(target, player);
                }
            }

            // Switch frames at a linearly increasing rate to make it look like the player is flipping pages quickly.
            if (projectile.frameCounter++ >= (int)MathHelper.Lerp(10f, 1f, Utils.InverseLerp(0f, 200f, Time, true)))
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }

            AdjustPlayerValues(player);
        }
        public void AdjustPlayerValues(Player player)
        {
            projectile.spriteDirection = projectile.direction = player.direction;
            projectile.timeLeft = 2;
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (projectile.direction * projectile.velocity).ToRotation();
        }
        public void SummonProjectilesOnTarget(NPC target, Player owner)
        {
            Projectile hex = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<EternityHex>(), projectile.damage, 0f, owner.whoAmI, target.whoAmI);
            hex.localAI[1] = projectile.whoAmI;

            for (int i = 0; i < 5; i++)
            {
                float crystalAngleOffset = MathHelper.TwoPi / 5f * i;
                Projectile crystal = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCrystal>(), 0, 0f, owner.whoAmI, target.whoAmI, crystalAngleOffset);
                crystal.frame = i % 2;
                crystal.localAI[1] = projectile.whoAmI;
            }
            for (int i = 0; i < 10; i++)
            {
                float circleOffset = MathHelper.TwoPi / 10f * i;
                Projectile circleSpell = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<EternityCircle>(), 0, 0f, owner.whoAmI, target.whoAmI, circleOffset);
                circleSpell.localAI[1] = projectile.whoAmI;
            }
        }
        public override bool CanDamage() => false;
    }
}
