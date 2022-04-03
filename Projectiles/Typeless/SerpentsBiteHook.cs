using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Typeless
{
    public class SerpentsBiteHook : ModProjectile
    {
        public const float PullSpeed = 12f;
        public const float ReelbackSpeed = 14f;
        public const float LaunchSpeed = 18f;
        public const float GrappleRangInTiles = 28.125f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Serpent's Bite");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
        }

        // Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook
        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < Main.maxProjectiles; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                {
                    hooksOut++;
                }
            }
            if (hooksOut > 2) // This hook can have 3 hooks out.
            {
                return false;
            }
            return true;
        }

        // Amethyst Hook is 300, Static Hook is 600, 16f = 1 tile
        public override float GrappleRange() => GrappleRangInTiles * 16f;

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 2;
        }

        // default is 11, Lunar is 24
        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = ReelbackSpeed;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = PullSpeed;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawHook(Projectile, GetTexture("CalamityMod/ExtraTextures/Chains/SerpentsBiteChain"));
            return true;
        }

        public override void AI()
        {
            Projectile.spriteDirection = -Projectile.direction;
        }
    }
}
