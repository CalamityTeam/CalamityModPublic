using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SlimePuppetStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Puppet Staff");
            Tooltip.SetDefault("Summons a slime ball that follows you\n" +
                                "The ball flies toward nearby enemies and explodes into slime on enemy hits\n" +
                                "Does not consume minion slots"); // In other words, bootleg mage :TaxEvasion:
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.mana = 10;
            Item.width = 48;
            Item.height = 56;
            Item.useTime = Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.6f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SlimeGodPossession");
            Item.shoot = ModContent.ProjectileType<SlimePuppet>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(12f);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, Main.myPlayer);

            return false;
        }
    }
}
