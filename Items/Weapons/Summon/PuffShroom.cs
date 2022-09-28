using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PuffShroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Puff Shroom");
            Tooltip.SetDefault("Summons a cute mushroom warrior to fight for you");
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item42;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PuffWarrior>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int puffGuy = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(puffGuy))
                    Main.projectile[puffGuy].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
