using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SCalBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Some Edgy Word idk");
            Tooltip.SetDefault("yes");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.damage = 2444;
            item.magic = true;
            item.mana = 10;
            item.width = 38;
            item.height = 40;
            item.useTime = item.useAnimation = 120;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = SoundID.DD2_EtherianPortalDryadTouch;
            item.knockBack = 0f;

            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;

            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = ModContent.ProjectileType<SCalBookProj>();
            item.channel = true;
            item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}