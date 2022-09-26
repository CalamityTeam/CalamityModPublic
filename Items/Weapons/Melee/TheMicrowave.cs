using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheMicrowave : ModItem
    {
        public static readonly SoundStyle BeepSound = new("CalamityMod/Sounds/Custom/MicrowaveBeep");
        public static readonly SoundStyle MMMSound = new("CalamityMod/Sounds/Custom/MMMMMMMMMMMMM") { IsLooped = true };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Microwave");
            Tooltip.SetDefault("Fries nearby enemies with radiation\n" +
            "A very agile yoyo\n" +
            "Cooking, Astral Infection style");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 65;
            Item.knockBack = 3f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<MicrowaveYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Cyan;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
        }
    }
}
