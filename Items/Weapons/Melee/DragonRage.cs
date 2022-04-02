using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DragonRage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Rage");
            Tooltip.SetDefault("Twirls a baton that causes explosions on enemy hits\n" +
            "Every ten hits will summon a ring of fireballs");
        }

        public override void SetDefaults()
        {
            item.damage = 1275;
            item.knockBack = 7.5f;
            item.useAnimation = item.useTime = 25;
            item.melee = true;
            item.noMelee = true;
            item.channel = true;
            item.autoReuse = true;
            item.shootSpeed = 14f;
            item.shoot = ModContent.ProjectileType<DragonRageStaff>();

            item.width = 128;
            item.height = 140;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().trueMelee = true;
        }
    }
}
