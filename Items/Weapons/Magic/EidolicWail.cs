using CalamityMod.Projectiles.Magic;
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
            Item.value = Item.buyPrice(1, 40, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/WyrmScream");
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<EidolicWailSoundwave>();
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);
    }
}
