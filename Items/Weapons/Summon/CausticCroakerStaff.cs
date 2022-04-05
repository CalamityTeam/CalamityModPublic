using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CausticCroakerStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caustic Croaker Staff");
            Tooltip.SetDefault("Summons a toad that explodes if enemies are nearby");
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.mana = 10;
            Item.width = 36;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EXPLODINGFROG>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                player.UpdateMaxTurrets();
            }
            return false;
        }
    }
}
