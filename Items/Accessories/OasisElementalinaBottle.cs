using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("WifeinaBottlewithBoobs", "RareElementalinaBottle")] // Yes, that was the actual name.
    public class OasisElementalinaBottle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        // Hold on... isn't this the HEALER elemental??
        public static int ElementalDamage = 45;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
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
            modPlayer.oasisElemental = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<OasisElementalBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<OasisElementalBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(ElementalDamage);

                    Projectile sandy = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<SandElementalHealer>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    sandy.originalDamage = ElementalDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.oasisElementalVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<OasisElementalBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<OasisElementalBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] < 1)
                {
                    var source = player.GetSource_Accessory(Item);

                    // 08DEC2023: Ozzatron: Rare Sand Elementals spawned with... Hold on a second. Why the fuck are we doing damage calculations when the accessory is in vanity?!
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(ElementalDamage);

                    Projectile sandy = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<SandElementalHealer>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    sandy.originalDamage = ElementalDamage;
                }
            }
        }
    }
}
