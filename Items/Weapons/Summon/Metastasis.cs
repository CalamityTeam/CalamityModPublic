using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
                "It takes up 4 minion slots and it can only be summoned once\n" +
               CalamityUtils.ColorMessage(PoeticTooltipLine, CalamityGlobalItem.ExhumedTooltipColor));
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 78;
            Item.useTime = Item.useAnimation = 10; // 9 because of useStyle 1
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.shoot = ModContent.ProjectileType<SepulcherMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, Main.MouseWorld, -Vector2.UnitY * 5f, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}
