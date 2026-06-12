using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Hematemesis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>()];
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 54;
            Item.damage = 75;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.75f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodBlast>();
            Item.shootSpeed = 10f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = player.ClampedMouseWorld();
            for (int x = 0; x < 10; x++)
            {
                Projectile.NewProjectile(source, position.X + (float)Main.rand.Next(-150, 150), position.Y + 600f, 0f, -10f, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
