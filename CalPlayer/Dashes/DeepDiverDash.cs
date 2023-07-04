using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.CalPlayer.Dashes
{
    public class DeepDiverDash : PlayerDashEffect
    {
        public static new string ID => "Deep Diver";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 25.9f;

        public override void OnDashEffects(Player player)
        {
            for (int d = 0; d < 60; d++)
            {
                Dust iceDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 33, 0f, 0f, 100, default, 1.25f);
                iceDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                iceDashDust.velocity *= 0.2f;
                iceDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                iceDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                iceDashDust.noGravity = true;
                iceDashDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            for (int m = 0; m < 24; m++)
            {
                Dust iceDashDust = Dust.NewDustDirect(player.position + Vector2.UnitY * 4f, player.width, player.height - 8, 33, 0f, 0f, 100, default, 1.25f);
                iceDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                iceDashDust.velocity *= 0.2f;
                iceDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                iceDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                iceDashDust.noGravity = true;
                if (Main.rand.NextBool(2))
                    iceDashDust.fadeIn = 0.5f;
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 18f;
        }
    }
}
