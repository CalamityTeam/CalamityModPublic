using Terraria.DataStructures;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class VoidConcentrationStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Concentration Staff");
            Tooltip.SetDefault("Summons a foreboding aura that attacks by firing void orbs\n" +
                               "Minion damage is increased by 5% while the aura is active\n" +
                               "Requires three minion slots to use\n" +
                               "Only one may exist\n" +
                               "Right click to launch a black hole that grows in size");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 72;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.DD2_EtherianPortalOpen;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.damage = 150;
            Item.knockBack = 4f;
            Item.useTime = Item.useAnimation = 15; // 14 because of useStyle 1
            Item.shoot = ModContent.ProjectileType<VoidConcentrationAura>();
            Item.shootSpeed = 10f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
                return player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationAura>()] == 0;
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
                player.AddBuff(ModContent.BuffType<VoidConcentrationBuff>(), 120);
            }
            return false;
        }

        // TODO -- should be strictly unnecessary as per API design
        public override bool AltFunctionUse(Player player) => base.AltFunctionUse(player);
    }
}
