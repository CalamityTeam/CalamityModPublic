using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("LureofEnthrallment")]
    public class PearlofEnthrallment : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int ElementalDamage = 65;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<WindChilled>(), BuffID.Confused];
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.waterElemental = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<WaterElemental>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<WaterElemental>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] < 1)
                {
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(ElementalDamage);

                    int anahita = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<WaterElementalMinion>(), damage, 2f, Main.myPlayer);
                    if (Main.projectile.IndexInRange(anahita))
                        Main.projectile[anahita].originalDamage = ElementalDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.waterElementalVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<WaterElemental>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<WaterElemental>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Anahitas spawned with... Hold on a second. Why the fuck are we doing damage calculations when the accessory is in vanity?!
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(ElementalDamage);

                    int anahita = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<WaterElementalMinion>(), damage, 2f, Main.myPlayer);
                    if (Main.projectile.IndexInRange(anahita))
                        Main.projectile[anahita].originalDamage = ElementalDamage;
                }
            }
        }
    }
}
