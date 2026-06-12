using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class WulfrumController : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";

        public static float PartialRegenBoostPerBuffDroid = 0.5f;
        public static int DefenseBoostPerBuffDroid = 2;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PartialRegenBoostPerBuffDroid.ToRegenPerSecond(), DefenseBoostPerBuffDroid);
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.damage = 19;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect
            Item.autoReuse = true;
            Item.buffType = ModContent.BuffType<WulfrumDroidBuff>();
            Item.shoot = ModContent.ProjectileType<WulfrumDroid>();
            Item.DamageType = DamageClass.Summon;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 1f);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class WulfrumControllerPlayer : ModPlayer
    {
        public int buffingDrones = 0;

        public override void ResetEffects()
        {
        }

        public override void UpdateDead()
        {
            buffingDrones = 0;
        }

        // This needs to be a fair bit earlier for partial life regen to work
        public override void PostUpdateBuffs()
        {
            if (buffingDrones > 0)
            {
                Player.Calamity().partialLifeRegen += buffingDrones * WulfrumController.PartialRegenBoostPerBuffDroid;
                Player.statDefense += buffingDrones * WulfrumController.DefenseBoostPerBuffDroid;

                buffingDrones = 0;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (buffingDrones > 0 && Main.rand.NextBool(3))
            {
                Vector2 dustPos = Player.position + (Player.height * Main.rand.NextFloat(0.7f, 1f) + Player.gfxOffY) * Vector2.UnitY + Vector2.UnitX * Main.rand.NextFloat() * Player.width;

                Dust chust = Dust.NewDustPerfect(dustPos, DustID.ApprenticeStorm, -Vector2.UnitY * Main.rand.NextFloat(1.4f, 7f) + Player.velocity, Alpha: 100, Scale: Main.rand.NextFloat(1.2f, 1.8f));
                chust.noGravity = true;
                chust.noLight = true;
            }
        }
    }
}
