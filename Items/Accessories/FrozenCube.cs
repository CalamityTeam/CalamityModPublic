using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.CustomRecipes;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class FrozenCube : ModItem, ILocalizedModType
    {
        public static int mistBaseDamage = 1;
        public static int slamBaseDamage = 20;
        public static int baseAttackSpeed = 90;
        public static int baseAttackCooldown = 180;

        public static int usedDefenseDivide = 3;
        public static int debuff = ModContent.BuffType<WindChilled>();

        public static readonly SoundStyle noise = new("CalamityMod/Sounds/Item/ElumphantSound") { Volume = 0.6f };
        public static readonly SoundStyle cry = new("CalamityMod/Sounds/Item/ElumphantCry") { Volume = 0.6f };
        public static readonly SoundStyle hit = new("CalamityMod/Sounds/Item/ElumphantBop") { Volume = 0.6f };
        public static readonly SoundStyle jokeBonk = new("CalamityMod/Sounds/Item/Bonk") { Volume = 0.6f };

        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.value = Item.buyPrice(gold: 15); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundFrozenCube)
            {
                RecipeUnlockHandler.HasFoundFrozenCube = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.frozenCube = true;
            modPlayer.frozenCubeVisuals = !hideVisual;

            int projectile = ProjectileType<Elumphant>();
            if (player.ownedProjectileCounts[projectile] < 1 && !player.dead)
            {
                int damage = (int)player.GetTotalDamage<GenericDamageClass>().ApplyTo(slamBaseDamage);
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, projectile, damage, 0f, player.whoAmI);
            }
        }
        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.frozenCubeVanity = true;
            modPlayer.frozenCubeVisuals = true;

            int projectile = ProjectileType<Elumphant>();
            if (player.ownedProjectileCounts[projectile] < 1 && !player.dead)
            {
                int damage = 0;
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, projectile, damage, 0f, player.whoAmI);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Main.LocalPlayer != null)
                list.FindAndReplace("[DAMAGELINE]", Main.LocalPlayer.Calamity().frozenCube ? 
                    this.GetLocalization("EquippedDebuff").Format((Main.LocalPlayer.Calamity().frozenCubeDebuffBoost.ToPercent("N0")).ToString()) + "\n" +
                    this.GetLocalization("EquippedElumphant").Format((Main.LocalPlayer.Calamity().frozenCubeElumphantBoost.ToPercent("N1")).ToString())
                : this.GetLocalizedValue("Unequipped"));
        }
    }
}
