using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EidolicWail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolic Wail");
            Tooltip.SetDefault("Earrape\n" +
                "Fires a string of bouncing sound waves that become stronger as they travel");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 126;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 12;
            Item.reuseDelay = 30;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.UseSound = CommonCalamitySounds.WyrmScreamSound;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<EidolicWailSoundwave>();
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }
}
