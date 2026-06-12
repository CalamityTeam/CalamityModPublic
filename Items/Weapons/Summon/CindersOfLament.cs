using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CindersOfLament : ExhumedItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<EntropysVigil>();
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<VulnerabilityHex>()];
        }

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 92;
            Item.damage = 1666;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 80; // 79 because of useStyle 1
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<CataclysmSummon>();
            Item.shootSpeed = 10f;

            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (Main.rand.NextBool())
                    type = ModContent.ProjectileType<CatastropheSummon>();
                int p = Projectile.NewProjectile(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
