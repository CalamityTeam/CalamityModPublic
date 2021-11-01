using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class Metastasis : ModItem
    {
        public const string PoeticTooltipLine = "A contemplated possible future of the cosmic serpent,\n" +
            "A gruesome warning for those blinded by the hunger for power.";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Metastasis");
            Tooltip.SetDefault("Summons a sepulcher to fight for you\n" +
                "While the sepulcher is active brimstone hearts will begin to appear and orbit you\n" +
                "It will attempt to attack you more and more frequently depending on how many hearts are present\n" +
                "It takes up 4 minion slots\n" +
               CalamityUtils.ColorMessage(PoeticTooltipLine, CalamityGlobalItem.ExhumedTooltipColor));
        }

        public override void SetDefaults()
        {
            item.damage = 666;
            item.mana = 10;
            item.width = 58;
            item.height = 58;
            item.useTime = item.useAnimation = 10; // 9 because of useStyle 1
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.UseSound = SoundID.DD2_BetsySummon;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SepulcherMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(Main.MouseWorld, -Vector2.UnitY * 5f, type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
