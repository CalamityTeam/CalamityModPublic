using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Cryophobia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryophobia");
            Tooltip.SetDefault("Chill\n" +
                "Fires an icy wave that splits multiple times and explodes into shards");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 56;
            Item.height = 34;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item117;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<CryoBlast>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}
