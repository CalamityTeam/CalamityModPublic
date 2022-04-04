using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Magic
{
    public class Thunderstorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thunderstorm");
            Tooltip.SetDefault("Make it rain");
        }

        public override void SetDefaults()
        {
            Item.damage = 132;
            Item.mana = 50;
            Item.DamageType = DamageClass.Magic;
            Item.width = 48;
            Item.height = 22;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/PlasmaBlast");
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ModContent.ProjectileType<ThunderstormShot>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
    }
}
