using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.CalPlayer.Dashes
{
    public class CounterScarfDash : PlayerDashEffect
    {
        public static new string ID => "Counter Scarf";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => player.Calamity().evasionScarf ? 16.3f : 15f;

        public override void OnDashEffects(Player player)
        {
            for (int d = 0; d < 20; d++)
            {
                Dust redDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 235, 0f, 0f, 100, default, 2f);
                redDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                redDashDust.velocity *= 0.2f;
                redDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                redDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cNeck, player);
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            for (int k = 0; k < 2; k++)
            {
                // Spawn dust at the player's feet if they are moving vertically. Otherwise, spawn it around their body.
                int dustSpawnHeight = 8;
                float dustSpawnTop = player.Bottom.Y - 4f;
                if (player.velocity.Y != 0f)
                {
                    dustSpawnHeight = 16;
                    dustSpawnTop = player.Center.Y - 8f;
                }
                Vector2 dustSpawnPosition = new Vector2(player.position.X, dustSpawnTop);

                Dust redDashDust = Dust.NewDustDirect(dustSpawnPosition, player.width, dustSpawnHeight, 235, 0f, 0f, 100, default, 1.4f);
                redDashDust.velocity *= 0.1f;
                redDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                redDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cNeck, player);
            }
        }
    }
}
