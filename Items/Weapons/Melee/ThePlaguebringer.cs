using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ThePlaguebringer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pandemic");
            Tooltip.SetDefault("Fires plague seekers when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 66;
            Item.knockBack = 2.5f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<PandemicYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 80);
        }
    }
}
