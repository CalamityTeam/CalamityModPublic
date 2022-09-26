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
            Tooltip.SetDefault("Summons a Black Hawk jet to fight for you\n" +
                               "Jets will fire bullets from your inventory\n" +
                               "50% chance to not consume ammo");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BlackHawkSummon>();
            Item.shootSpeed = 6f; // Affects bullet speed
            Item.DamageType = DamageClass.Summon;
            Item.rare = ItemRarityID.LightRed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 1f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Item.damage;
                }
            }
            return false;
        }
    }
}
