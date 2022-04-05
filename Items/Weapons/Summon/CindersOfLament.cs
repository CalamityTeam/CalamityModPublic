using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CindersOfLament : ModItem
    {
        public const string PoeticTooltipLine = "The Witch, a sinner of her own making,\n" +
            "Within her mind her demon lies, ever patient, until the end of time.";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinders of Lament");
            Tooltip.SetDefault("Summons either Cataclysm or Catastrophe at the mouse position\n" +
                "They will look at you for a moment before charging at you\n" +
                "They can do damage to both you and enemies\n" +
               CalamityUtils.ColorMessage(PoeticTooltipLine, CalamityGlobalItem.ExhumedTooltipColor));
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 1666;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 86;
            Item.height = 92;
            Item.useTime = Item.useAnimation = 30; // 29 because of useStyle 1
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.shoot = ModContent.ProjectileType<CataclysmSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (Main.rand.NextBool(2))
                    type = ModContent.ProjectileType<CatastropheSummon>();
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
