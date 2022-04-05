using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ShellfishStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shellfish Staff");
            Tooltip.SetDefault("Summons a shellfish to fight for you\n" +
                               "Takes up 2 minion slots");
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 10;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Shellfish>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
