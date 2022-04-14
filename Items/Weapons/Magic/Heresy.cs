using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Heresy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heresy");
            Tooltip.SetDefault("Releases fire and souls upward from a held book\n" +
                "As the book is held the power of both increases");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 888;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 38;
            Item.height = 40;
            Item.useTime = Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.DD2_EtherianPortalDryadTouch;
            Item.knockBack = 0f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<HeresyProj>();
            Item.channel = true;
            Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
