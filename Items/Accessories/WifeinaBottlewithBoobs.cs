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
    public class WifeinaBottlewithBoobs : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
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
            modPlayer.sandBoobWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<SandyHealingWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SandyHealingWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);

                    // 08DEC2023: Ozzatron: Rare Sand Elementals spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(45);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    Projectile sandy = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<SandElementalHealer>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    sandy.originalDamage = baseDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sandBoobWaifuVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<SandyHealingWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<SandyHealingWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);

                    // 08DEC2023: Ozzatron: Rare Sand Elementals spawned with... Hold on a second. Why the fuck are we doing damage calculations when the accessory is in vanity?!
                    int baseDamage = player.ApplyArmorAccDamageBonusesTo(45);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDamage);

                    Projectile sandy = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<SandElementalHealer>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    sandy.originalDamage = baseDamage;
                }
            }
        }
    }
}
