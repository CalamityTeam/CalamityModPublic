using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MythrilArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.MythrilHelmet;

        public override int? BodyPieceID => ItemID.MythrilChainmail;

        public override int? LegPieceID => ItemID.MythrilGreaves;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.MythrilHood, ItemID.MythrilHat };

        public override string ArmorSetName => "Mythril";

        public const int MaxManaBoost = 20;
        public const int FlareFrameSpawnDelay = 12;
        public const int FlareDamageSoftcap = 50;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetText($"Vanilla.Armor.SetBonus.{ArmorSetName}").Format(FlareFrameSpawnDelay)}";
        }

        public override void ApplyHeadPieceEffect(Player player)
        {
            if (player.armor[0].type == ItemID.MythrilHood)
                player.statManaMax2 += MaxManaBoost;
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().MythrilSet = true;
        }

        public static void OnHitEffects(NPC victim, int originalDamage, Player owner)
        {
            // Don't spawn anything if the player isn't actually wearing the set.
            if (!owner.Calamity().MythrilSet)
                return;

            // Don't spawn anything if the on-hit delay is still counting down.
            if (owner.Calamity().MythrilFlareSpawnCountdown > 0)
                return;

            // Reset the spawn delay.
            owner.Calamity().MythrilFlareSpawnCountdown = FlareFrameSpawnDelay;

            int flareDamage = CalamityUtils.DamageSoftCap(originalDamage * 0.4, FlareDamageSoftcap);
            flareDamage = owner.ApplyArmorAccDamageBonusesTo(flareDamage);

            Vector2 flareSpawnPosition = victim.Center + Main.rand.NextVector2Circular(10f, 10f);
            Projectile.NewProjectile(owner.GetSource_ItemUse(owner.ActiveItem()), flareSpawnPosition, Vector2.Zero, ModContent.ProjectileType<MythrilFlare>(), flareDamage, 0f, owner.whoAmI);
        }
    }
}
