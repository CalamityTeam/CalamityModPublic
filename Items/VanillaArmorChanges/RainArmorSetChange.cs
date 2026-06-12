using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class RainArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.RainHat;
        public override int? BodyPieceID => ItemID.RainCoat;
        public override int? LegPieceID => null;

        public override string ArmorSetName => "Rain";

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = $"{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().rainSet = true;
            player.autoJump = true;
            player.jumpSpeedBoost += 1.2f; // 24%
        }

        public static void SpawnRainArmorJump(Player Player)
        {
            bool rainBoost = Player.Center.Y < Main.worldSurface * 16.0 && Main.raining;
            int damage = (int)Player.GetBestClassDamage().ApplyTo(30 * (rainBoost ? 1.5f : 1));
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Vector2.UnitY * 26, Vector2.Zero, ModContent.ProjectileType<PuddleSplash>(), damage, 0, Main.myPlayer);
        }
    }
}
