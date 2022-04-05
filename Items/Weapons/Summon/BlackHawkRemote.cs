using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class BlackHawkRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Hawk Remote");
            Tooltip.SetDefault("Summons a Black Hawk jet to fight for you");
        }

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BlackHawkSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.rare = ItemRarityID.LightRed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    position = Main.MouseWorld;
                    velocity.X = 0;
                    velocity.Y = 0;
                    Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 1f);
                }
            }
            return false;
        }
    }
}
