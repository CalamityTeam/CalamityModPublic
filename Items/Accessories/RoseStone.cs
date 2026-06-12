using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RoseStone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int ElementalDamage = 60;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
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
            modPlayer.brimElemental = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<BrimstoneElemental>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneElemental>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Brimstone Elementals spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(ElementalDamage);

                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = ElementalDamage;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.brimElementalVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_Accessory(Item);
                if (player.FindBuffIndex(ModContent.BuffType<BrimstoneElemental>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneElemental>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    // 08DEC2023: Ozzatron: Brimstone Elementals spawned with... Hold on a second. Why the fuck are we doing damage calculations when the accessory is in vanity?!
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(ElementalDamage);

                    int p = Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), damage, 2f, Main.myPlayer, 0f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = ElementalDamage;
                }
            }
        }
    }
}
