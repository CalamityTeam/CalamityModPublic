using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class YharimsCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Crystal");
            Tooltip.SetDefault("Fires draconic beams of total annihilation");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.magic = true;
            item.mana = 15;
            item.width = 16;
            item.height = 16;
            item.useTime = 10;
            item.useAnimation = 10;
            item.reuseDelay = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item13;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 0f;
            item.shoot = ModContent.ProjectileType<YharimsCrystalPrism>();
            item.shootSpeed = 30f;

            item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
