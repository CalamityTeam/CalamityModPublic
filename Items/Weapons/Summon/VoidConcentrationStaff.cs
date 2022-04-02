using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	class VoidConcentrationStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Concentration Staff");
            Tooltip.SetDefault("Summons a foreboding aura that attacks by firing void orbs\n" + //If you have flavour text ideas, feel free to implement, my brain is still like nonexistant as per usual yeah.
							   "Minion damage is increased by 5% while the aura is active\n" +
                               "Requires three minion slots to use\n" +
                               "Only one may exist\n" +
                               "Right click to launch a black hole that grows in size");
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.height = 72;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.DD2_EtherianPortalOpen;
            item.summon = true;
            item.mana = 10;
            item.damage = 150;
            item.knockBack = 4f;
            item.useTime = item.useAnimation = 15; // 14 because of useStyle 1
            item.shoot = ModContent.ProjectileType<VoidConcentrationAura>();
            item.shootSpeed = 10f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
                return player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationAura>()] == 0;
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                player.AddBuff(ModContent.BuffType<VoidConcentrationBuff>(), 120);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return base.AltFunctionUse(player);
        }
    }
}
