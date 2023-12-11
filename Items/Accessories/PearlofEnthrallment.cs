using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Potions.Alcohol;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("LureofEnthrallment")]
    public class PearlofEnthrallment : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
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
            modPlayer.sirenWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<WaterWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<WaterWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Anahitas spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(65);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    int anahita = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<WaterElementalMinion>(), damage, 2f, Main.myPlayer);
                    if (Main.projectile.IndexInRange(anahita))
                        Main.projectile[anahita].originalDamage = baseDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sirenWaifuVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<WaterWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<WaterWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Anahitas spawned with... Hold on a second. Why the fuck are we doing damage calculations when the accessory is in vanity?!
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(65);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    int anahita = Projectile.NewProjectile(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<WaterElementalMinion>(), damage, 2f, Main.myPlayer);
                    if (Main.projectile.IndexInRange(anahita))
                        Main.projectile[anahita].originalDamage = baseDamage;
                }
            }
        }
    }
}
