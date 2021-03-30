using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Cryophobia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryophobia");
            Tooltip.SetDefault("Chill\n" +
				"Rare Item Variant");
        }

        public override void SetDefaults()
        {
            item.damage = 56;
            item.magic = true;
            item.mana = 18;
            item.width = 56;
            item.height = 34;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
			item.value = CalamityGlobalItem.Rarity5BuyPrice;
			item.rare = ItemRarityID.Pink;
			item.UseSound = SoundID.Item117;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<CryoBlast>();
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
