using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("AngryChickenStaff")]
    public class YharonsKindleStaff : ModItem
    {
        public const int Damage = 300;
        public const float ReboundRamDamageFactor = 2f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharon's Kindle Staff");
            Tooltip.SetDefault("Summons a fiery draconid to fight for you\n" +
                               "Requires 5 minion slots to use");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = Damage;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 80;
            Item.height = 74;
            Item.useTime = Item.useAnimation = 10;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.UseSound = CommonCalamitySounds.FlareSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FieryDraconid>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                int dragon = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(dragon))
                    Main.projectile[dragon].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
