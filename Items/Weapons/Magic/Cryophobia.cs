using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
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
                "Fires an icy wave that splits multiple times and explodes into shards");
        }

        public override void SetDefaults()
        {
            item.damage = 55;
            item.magic = true;
            item.mana = 18;
            item.width = 56;
            item.height = 34;
            item.useTime = 28;
            item.useAnimation = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity6BuyPrice;
            item.rare = ItemRarityID.LightPurple;
            item.UseSound = SoundID.Item117;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<CryoBlast>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
